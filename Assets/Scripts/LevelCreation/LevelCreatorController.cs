using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StateMachine = StateMachine<LevelCreatorStates, LevelCreatorStateNotification>;

public enum LevelCreatorStates
{
	FrontMenu, LevelCreation, SavingMap, LoadingMap, TestingMap
};

public enum LevelCreatorStateNotification
{
	FrontMenuEnter, FrontMenuExit,
	LevelCreationEnter, LevelCreationExit,
	SavingMapEnter, SavingMapExit,
	LoadingMapEnter, LoadingMapExit,
	TestingMapEnter, TestingMapExit
};

public class LevelCreatorController : MonoBehaviour 
{
	public static string mapFilesFilePath = "/MapFiles/";
	GameObject mainCam;
	List<DraggableMapObject> mapObjects = new List<DraggableMapObject>();
	LevelCreator levelCreator;

	void Awake()
	{
		EnsureMapDirectoryExists();
		RegisterStates();
		mainCam = Camera.main.gameObject;
		levelCreator = GameObject.Find("LevelCreator").GetComponent<LevelCreator>();
	}

	void Start()
	{
		if (!LevelCreatorUIController.cameFromPreview)
			StateMachine.SetInitialState(LevelCreatorStates.FrontMenu);
		else
			StateMachine.SetInitialState(LevelCreatorStates.LevelCreation);
	}

	void EnsureMapDirectoryExists()
	{
		if(!System.IO.Directory.Exists(mapFilesFilePath))
			System.IO.Directory.CreateDirectory(mapFilesFilePath);
	}
	
	void RegisterStates()
	{
		StateMachine.RegisterState(LevelCreatorStates.FrontMenu, LevelCreatorStateNotification.FrontMenuEnter, LevelCreatorStateNotification.FrontMenuExit);
		StateMachine.RegisterState(LevelCreatorStates.LevelCreation, LevelCreatorStateNotification.LevelCreationEnter, LevelCreatorStateNotification.LevelCreationExit);
		StateMachine.RegisterState(LevelCreatorStates.SavingMap, LevelCreatorStateNotification.SavingMapEnter, LevelCreatorStateNotification.SavingMapExit);
		StateMachine.RegisterState(LevelCreatorStates.LoadingMap, LevelCreatorStateNotification.LoadingMapEnter, LevelCreatorStateNotification.LoadingMapExit);
		StateMachine.RegisterState(LevelCreatorStates.TestingMap, LevelCreatorStateNotification.TestingMapEnter, LevelCreatorStateNotification.TestingMapExit);

		StateMachine.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.FrontMenuEnter);

		StateMachine.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.LevelCreationEnter);
		StateMachine.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.LevelCreationExit);

		StateMachine.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.SavingMapEnter);

		StateMachine.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.LoadingMapEnter);

		StateMachine.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.TestingMapEnter);
		StateMachine.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.TestingMapExit);

		NotificationCenter<DragAndDropNotification>.DefaultCenter.AddObserver(this, DragAndDropNotification.ObjectPlaced);
		NotificationCenter<DragAndDropNotification>.DefaultCenter.AddObserver(this, DragAndDropNotification.MapObjectRemoved);

	}

	void ObjectPlaced(NotificationCenter<DragAndDropNotification>.Notification notiData)
	{
		var objectThatWasPlaced = (GameObject)notiData.data;

		var mapObj = objectThatWasPlaced.GetComponent<DraggableMapObject>();

		if(mapObj != null)
			mapObjects.Add(mapObj);
	}

	void MapObjectRemoved(NotificationCenter<DragAndDropNotification>.Notification notiData)
	{
		var objDestroyed = (DraggableMapObject)notiData.data;

		mapObjects.Remove(objDestroyed);
	}

	#region State Responses

	void FrontMenuEnter()
	{
		mapObjects.Clear();
	}

	void LevelCreationEnter()
	{
		GetDraggableMapObjects();
		mainCam.GetComponent<CameraControls>().enabled = true;
		TurnOnDraggingMapObjects();
		TurnOffMapObjectColliders();
	}

	void LevelCreationExit()
	{
		mainCam.GetComponent<CameraControls>().enabled = false;
	}

	void SavingMapEnter()
	{
		TurnOffDraggingMapObjects();
	}

	void LoadingMapEnter()
	{
		TurnOffDraggingMapObjects();
	}

	void TestingMapEnter()
	{
		InitMapForPreview();
	}

	void TestingMapExit()
	{
		var saves = LevelSerializer.SavedGames[LevelSerializer.PlayerName];
		var restoreData = saves.Where(e => e.Name == "BeforePreviewSave").FirstOrDefault().Data;
		LevelSerializer.LoadSavedLevel(restoreData);
		Time.timeScale = 1;
	}

#endregion

	public void TurnOffDraggingMapObjects()
	{
		mapObjects.ForEach(e => e.enabled = false);
	}

	public void TurnOnDraggingMapObjects()
	{
		mapObjects.ForEach(e => e.enabled = true);
	}

	public void TurnOnMapObjectColliders()
	{
		foreach(var mapObj in mapObjects)
		{
			if(mapObj.collider)
				mapObj.collider.enabled = true;

			if(mapObj.transform.childCount > 0)
			{
				foreach(Transform child in mapObj.transform)
				{
					if(child.collider)
						child.collider.enabled = true;
				}
			}
		}
	}

	public void TurnOffMapObjectColliders()
	{
		foreach(var mapObj in mapObjects)
		{
			if(mapObj.collider)
				mapObj.collider.enabled = true;

			if(mapObj.transform.childCount > 0)
			{
				foreach(Transform child in mapObj.transform)
				{
					if(child.collider)
					{
						child.collider.enabled = false;
					}
				}
			}
		}
	}

	public void GetDraggableMapObjects()
	{
		mapObjects.Clear();
		mapObjects = ((DraggableMapObject[])FindObjectsOfType(typeof(DraggableMapObject))).ToList();
	}

	public void InitMapForPreview()
	{
		GetDraggableMapObjects();
		TurnOnMapObjectColliders();

		TurnOffDraggingMapObjects();

		var playerObj = GameObject.FindWithTag("Player");

		playerObj.transform.parent = null;

		playerObj.GetComponent<PlayerCharacter>().playerMovement.canMove = true;
		playerObj.rigidbody.useGravity = true;
		playerObj.collider.enabled = true;
		mainCam.AddComponent<CameraFollow>();

		new GameObject("LevelControllerSingleton").AddComponent<LevelController>();

		levelCreator.CheckEdgeCubeNeighbours();

		var nullCubes = GameObject.FindGameObjectsWithTag("NullCube");

		foreach(var nullCube in nullCubes)
		{
			nullCube.renderer.enabled = false;
			nullCube.collider.enabled = true;
			nullCube.GetComponent<BoxCollider>().size = new Vector3(1, 10, 1);
		}

		LevelController.Instance.InitLevel();
	}

}
