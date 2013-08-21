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

	void Awake()
	{
		EnsureMapDirectoryExists();
		RegisterStates();
		mainCam = Camera.main.gameObject;
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
		StateMachine.SetInitialState(LevelCreatorStates.FrontMenu);

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

	void LevelCreationEnter()
	{
		mainCam.GetComponent<CameraControls>().enabled = true;
		mapObjects.ForEach(e => e.enabled = true);
	}

	void LevelCreationExit()
	{
		mainCam.GetComponent<CameraControls>().enabled = false;
	}

	void SavingMapEnter()
	{
		mapObjects.ForEach(e => e.enabled = false);
	}

	void LoadingMapEnter()
	{
		mapObjects.ForEach(e => e.enabled = false);
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


	public void InitMapForPreview()
	{
		mapObjects.ForEach(e => e.enabled = false);

		var playerObj = GameObject.FindWithTag("Player");

		playerObj.transform.parent = null;

		playerObj.GetComponent<PlayerCharacter>().playerMovement.canMove = true;
		playerObj.rigidbody.useGravity = true;
		playerObj.collider.enabled = true;
		mainCam.AddComponent<CameraFollow>();

		var levelController = new GameObject("LevelControllerSingleton").AddComponent<LevelController>();

		var nullCubes = GameObject.FindGameObjectsWithTag("NullCube");

		foreach(var cube in nullCubes)
		{
			cube.renderer.enabled = false;
			cube.GetComponent<BoxCollider>().size = new Vector3(1, 10, 1);
		}

		levelController.InitLevel();
	}

}
