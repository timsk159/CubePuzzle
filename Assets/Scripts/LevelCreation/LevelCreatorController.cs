using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StateM = StateMachine<LevelCreatorStates, LevelCreatorStateMessage>;

public enum LevelCreatorStates
{
	FrontMenu, LevelCreation, SavingMap, LoadingMap, TestingMap
};

public enum LevelCreatorStateMessage
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
		if(!LevelCreatorUIController.cameFromPreview)
			StateM.SetInitialState(LevelCreatorStates.FrontMenu);
		else
		{
			StateM.SetInitialState(LevelCreatorStates.LevelCreation);
			LevelCreatorUIController.cameFromPreview = false;
		}
	}

	void EnsureMapDirectoryExists()
	{
		if(!System.IO.Directory.Exists(mapFilesFilePath))
			System.IO.Directory.CreateDirectory(mapFilesFilePath);
	}
	
	void RegisterStates()
	{
		StateM.RegisterState(LevelCreatorStates.FrontMenu, LevelCreatorStateMessage.FrontMenuEnter, LevelCreatorStateMessage.FrontMenuExit);
		StateM.RegisterState(LevelCreatorStates.LevelCreation, LevelCreatorStateMessage.LevelCreationEnter, LevelCreatorStateMessage.LevelCreationExit);
		StateM.RegisterState(LevelCreatorStates.SavingMap, LevelCreatorStateMessage.SavingMapEnter, LevelCreatorStateMessage.SavingMapExit);
		StateM.RegisterState(LevelCreatorStates.LoadingMap, LevelCreatorStateMessage.LoadingMapEnter, LevelCreatorStateMessage.LoadingMapExit);
		StateM.RegisterState(LevelCreatorStates.TestingMap, LevelCreatorStateMessage.TestingMapEnter, LevelCreatorStateMessage.TestingMapExit);

		Messenger<StateM.StateChangeData>.AddListener(LevelCreatorStateMessage.FrontMenuEnter.ToString(), FrontMenuEnter);
		Messenger<StateM.StateChangeData>.AddListener(LevelCreatorStateMessage.LevelCreationEnter.ToString(), LevelCreationEnter);
		Messenger<StateM.StateChangeData>.AddListener(LevelCreatorStateMessage.LevelCreationExit.ToString(), LevelCreationExit);
		Messenger<StateM.StateChangeData>.AddListener(LevelCreatorStateMessage.SavingMapEnter.ToString(), SavingMapEnter);
		Messenger<StateM.StateChangeData>.AddListener(LevelCreatorStateMessage.LoadingMapEnter.ToString(), LoadingMapEnter);
		Messenger<StateM.StateChangeData>.AddListener(LevelCreatorStateMessage.TestingMapEnter.ToString(), TestingMapEnter);
		Messenger<StateM.StateChangeData>.AddListener(LevelCreatorStateMessage.TestingMapExit.ToString(), TestingMapExit);

		Messenger<GameObject>.AddListener(DragAndDropMessage.ObjectPlaced.ToString(), ObjectPlaced);
		Messenger<DraggableMapObject>.AddListener(DragAndDropMessage.MapObjectRemoved.ToString(), MapObjectRemoved);

	}

	void DeRegisterStateListeners()
	{
		Messenger<StateM.StateChangeData>.RemoveListener(LevelCreatorStateMessage.FrontMenuEnter.ToString(), FrontMenuEnter);
		Messenger<StateM.StateChangeData>.RemoveListener(LevelCreatorStateMessage.LevelCreationEnter.ToString(), LevelCreationEnter);
		Messenger<StateM.StateChangeData>.RemoveListener(LevelCreatorStateMessage.LevelCreationExit.ToString(), LevelCreationExit);
		Messenger<StateM.StateChangeData>.RemoveListener(LevelCreatorStateMessage.SavingMapEnter.ToString(), SavingMapEnter);
		Messenger<StateM.StateChangeData>.RemoveListener(LevelCreatorStateMessage.LoadingMapEnter.ToString(), LoadingMapEnter);
		Messenger<StateM.StateChangeData>.RemoveListener(LevelCreatorStateMessage.TestingMapEnter.ToString(), TestingMapEnter);
		Messenger<StateM.StateChangeData>.RemoveListener(LevelCreatorStateMessage.TestingMapExit.ToString(), TestingMapExit);

		Messenger<GameObject>.RemoveListener(DragAndDropMessage.ObjectPlaced.ToString(), ObjectPlaced);
		Messenger<DraggableMapObject>.RemoveListener(DragAndDropMessage.MapObjectRemoved.ToString(), MapObjectRemoved);
	}

	void OnDestroy()
	{
		DeRegisterStateListeners();
	}

	void ObjectPlaced(GameObject objectThatWasPlaced)
	{
		var mapObj = objectThatWasPlaced.GetComponent<DraggableMapObject>();

		if(mapObj != null)
			mapObjects.Add(mapObj);
	}

	void MapObjectRemoved(DraggableMapObject objDestroyed)
	{
		mapObjects.Remove(objDestroyed);
	}

	#region State Responses

	void FrontMenuEnter(StateM.StateChangeData changeData)
	{
		mapObjects.Clear();
	}

	void LevelCreationEnter(StateM.StateChangeData changeData)
	{
		GetDraggableMapObjects();
		if(mainCam == null)
			mainCam = Camera.main.gameObject;
		mainCam.GetComponent<CameraControls>().enabled = true;
		TurnOnDraggingMapObjects();
		TurnOffMapObjectColliders();
	}

	void LevelCreationExit(StateM.StateChangeData changeData)
	{
		mainCam.GetComponent<CameraControls>().enabled = false;
	}

	void SavingMapEnter(StateM.StateChangeData changeData)
	{
		TurnOffDraggingMapObjects();
	}

	void LoadingMapEnter(StateM.StateChangeData changeData)
	{
		TurnOffDraggingMapObjects();
	}

	void TestingMapEnter(StateM.StateChangeData changeData)
	{
		InitMapForPreview();
		StartCoroutine(TestRoutine());
	}

	IEnumerator TestRoutine()
	{
		yield return new WaitForEndOfFrame();
		LevelTestStateController.Instance.SetCheckpoint();

	}

	void TestingMapExit(StateM.StateChangeData changeData)
	{
		var saves = LevelSerializer.SavedGames[LevelSerializer.PlayerName];
		var restoreData = saves.Where(e => e.Name == "BeforePreviewSave").FirstOrDefault().Data;
		LevelSerializer.LoadSavedLevel(restoreData);
		Time.timeScale = 1;
	}

#endregion

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Delete))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			RaycastHit hit;

			if(Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				if(hit.collider.GetComponent<DraggableMapObject>())
				{
					Messenger<Vector3>.Invoke(DragAndDropMessage.DoubleClicked.ToString(), hit.collider.transform.position);

					Destroy(hit.collider.gameObject);
				}
			}
		}
	}

	public void TurnOffDraggingMapObjects()
	{
		mapObjects = mapObjects.Where(e => e != null).ToList();
		mapObjects.ForEach(e => e.enabled = false);
	}

	public void TurnOnDraggingMapObjects()
	{
		mapObjects = mapObjects.Where(e => e != null).ToList();
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
		levelCreator.CheckEdgeCubeNeighbours();

		var nullCubes = GameObject.FindGameObjectsWithTag("NullCube");

		foreach(var nullCube in nullCubes)
		{
			nullCube.renderer.enabled = false;
			nullCube.collider.enabled = true;
			nullCube.GetComponent<BoxCollider>().size = new Vector3(1, 10, 1);
		}

		LevelController.Instance.InitLevel(false);
	}

}
