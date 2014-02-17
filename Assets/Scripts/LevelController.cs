using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using StateM = StateMachine<LevelState, LevelStateMessage>;

public enum LevelState
{
	InGame, Pause, EndGame, CutScene, ExitingLevel
};

public enum LevelStateMessage
{
	InGameEnter, InGameExit, PauseEnter, PauseExit, EndGameEnter, EndGameExit, CutSceneEnter, CutSceneExit, ExitingLevelEnter, ExitingLevelExit, LevelInitialized, LevelStarted
};

public class LevelController : MonoSingleton<LevelController> 
{
	public PlayerCharacter playerChar;
	
	private Colour _playerColour;

	public Colour PlayerColour 
	{
		get
		{
			try
			{
				_playerColour = playerChar.currentColor;
			}
			catch(NullReferenceException)
			{
				playerChar = GameObject.FindWithTag("Player").GetComponent<PlayerCharacter>();
				_playerColour = playerChar.currentColor;
				Debug.LogWarning("Tried to access player colour, but playerchar was null!");
				return _playerColour;
			}

			return _playerColour;
		}
	}	

	private bool _isStoryMode;

	public bool isStoryMode
	{
		get
		{
			if(Application.loadedLevelName == "UserLevelScene" || Application.loadedLevelName == "LevelCreator")
				_isStoryMode = false;
			else
				_isStoryMode = true;

			return _isStoryMode;
		}
	}

	public bool hasCheckpoint;
	
	bool canPause = true;
	bool isPaused;
	GameObject mapRoot;
	LevelIntro levelIntro;

	PhysicMaterial floorPhysicMat;

	Material[] skyboxes;
	
	void Awake()
	{
		RegisterStates();
		levelIntro = GetComponent<LevelIntro>();
		if(levelIntro == null)
		{
			levelIntro = gameObject.AddComponent<LevelIntro>();
		}
		if(floorPhysicMat == null)
			floorPhysicMat = (PhysicMaterial)Resources.Load("PassablePMat");
	}
	
	void RegisterStates()
	{
		StateMachine<LevelState, LevelStateMessage>.RegisterState(LevelState.InGame, LevelStateMessage.InGameEnter, LevelStateMessage.InGameExit); 
		StateMachine<LevelState, LevelStateMessage>.RegisterState(LevelState.Pause, LevelStateMessage.PauseEnter, LevelStateMessage.PauseExit); 
		StateMachine<LevelState, LevelStateMessage>.RegisterState(LevelState.EndGame, LevelStateMessage.EndGameEnter, LevelStateMessage.EndGameExit);
		StateMachine<LevelState, LevelStateMessage>.RegisterState(LevelState.CutScene, LevelStateMessage.CutSceneEnter, LevelStateMessage.CutSceneExit);

		Messenger<StateM.StateChangeData>.AddListener(LevelStateMessage.InGameEnter.ToString(), InGameEnter);
		Messenger<StateM.StateChangeData>.AddListener(LevelStateMessage.InGameExit.ToString(), InGameExit);

		Messenger<StateM.StateChangeData>.AddListener(LevelStateMessage.PauseEnter.ToString(), PauseEnter);
		Messenger<StateM.StateChangeData>.AddListener(LevelStateMessage.PauseExit.ToString(), PauseExit);

		Messenger<StateM.StateChangeData>.AddListener(LevelStateMessage.EndGameEnter.ToString(), EndGameEnter);
		Messenger<StateM.StateChangeData>.AddListener(LevelStateMessage.EndGameExit.ToString(), EndGameExit);

		Messenger<StateM.StateChangeData>.AddListener(LevelStateMessage.CutSceneEnter.ToString(), CutSceneEnter);
		Messenger<StateM.StateChangeData>.AddListener(LevelStateMessage.CutSceneExit.ToString(), CutSceneExit);

		Messenger<Colour>.AddListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);
		StateMachine<LevelState, LevelStateMessage>.SetInitialState(LevelState.InGame);
	}

	void Start()
	{
		skyboxes = new Material[3];
		skyboxes[0] = (Material)Resources.Load("Skyboxes/SkyboxRed");
		skyboxes[1] = (Material)Resources.Load("Skyboxes/SkyboxGreen");
		skyboxes[2] = (Material)Resources.Load("Skyboxes/SkyboxBlue");

		if(isStoryMode)
		{
			Messenger.AddListener(CutSceneMessage.CutSceneStarted.ToString(), CutSceneStarted);
			Messenger.AddListener(CutSceneMessage.CutSceneFinished.ToString(), CutSceneFinished);
		}
	}

	void OnDestroy()
	{
		Messenger<StateM.StateChangeData>.RemoveListener(LevelStateMessage.InGameEnter.ToString(), InGameEnter);
		Messenger<StateM.StateChangeData>.RemoveListener(LevelStateMessage.InGameExit.ToString(), InGameExit);

		Messenger<StateM.StateChangeData>.RemoveListener(LevelStateMessage.PauseEnter.ToString(), PauseEnter);
		Messenger<StateM.StateChangeData>.RemoveListener(LevelStateMessage.PauseExit.ToString(), PauseExit);

		Messenger<StateM.StateChangeData>.RemoveListener(LevelStateMessage.EndGameEnter.ToString(), EndGameEnter);
		Messenger<StateM.StateChangeData>.RemoveListener(LevelStateMessage.EndGameExit.ToString(), EndGameExit);

		Messenger<StateM.StateChangeData>.RemoveListener(LevelStateMessage.CutSceneEnter.ToString(), CutSceneEnter);
		Messenger<StateM.StateChangeData>.RemoveListener(LevelStateMessage.CutSceneExit.ToString(), CutSceneExit);

		Messenger<Colour>.RemoveListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);

		Messenger.RemoveListener(CutSceneMessage.CutSceneStarted.ToString(), CutSceneStarted);
		Messenger.RemoveListener(CutSceneMessage.CutSceneFinished.ToString(), CutSceneFinished);

		Messenger.RemoveListener(LevelIntroMessage.IntroFinished.ToString(), IntroFinished);
		Messenger.RemoveListener(LevelIntroMessage.IntroInterrupted.ToString(), IntroInterrupted);
	}
	
	public void InitLevel(bool playIntro, CutSceneObj introCutsceneObj = null)
	{
		hasCheckpoint = false;
		DestroyCombinedMeshes();
		var playerObj = GameObject.FindWithTag ("Player");
		if(playerObj == null)
		{
			CreatePlayer();
			playerObj = GameObject.FindWithTag("Player");
		}

		if(playerObj != null)
		{
			playerChar = playerObj.GetComponent<PlayerCharacter>();
		}
		playerChar.DisablePhysics();

		//Create floor and make sure player is on it
		mapRoot = GameObject.Find("MapRoot");

		CreateFloor();
		playerObj.transform.position = new Vector3(playerObj.transform.position.x, (playerObj.transform.position.y + 0.6f), playerObj.transform.position.z);

		SetupNullCubes();

		Camera.main.GetComponent<CameraFollow>().target = playerObj.transform;

		combinedMeshes = OptimiseLevelMesh();
		foreach(var go in combinedMeshes)
		{
			go.renderer.enabled = false;
		}
		var walls = GameObject.FindGameObjectsWithTag("WallCube");
		foreach(var wall in walls)
		{
			wall.renderer.enabled = false;
		}

		Messenger.Invoke(LevelStateMessage.LevelInitialized.ToString());

		if(playIntro)
		{
			levelIntro.InitIntro();
			StartGameAfterIntro(introCutsceneObj);
		}
		else
			IntroFinished();
	}

	//Just used to save us having to find the combined meshes after the intro animation.
	GameObject[] combinedMeshes;

	void StartGameAfterIntro(CutSceneObj introCutsceneObj = null)
	{
		playerChar.playerMovement.canMove = false;
		playerChar.rigidbody.useGravity = false;

		Messenger.AddListener(LevelIntroMessage.IntroFinished.ToString(), IntroFinished);
		Messenger.AddListener(LevelIntroMessage.IntroInterrupted.ToString(), IntroInterrupted);
		Camera.main.GetComponent<CameraFollow>().PutCameraBehindPlayer();
		Camera.main.GetComponent<CameraFollow>().enabled = false;
		
		StartCoroutine(levelIntro.PlayIntroAnimation(playerChar.gameObject, introCutsceneObj)); 
	}

	void IntroFinished()
	{
		Camera.main.GetComponent<CameraFollow>().enabled = true;

		SetInitialFloorColliders();
		foreach(var go in combinedMeshes)
		{
			if(!go.name.Contains("Null"))
				go.renderer.enabled = true;
		}

		var allFloorPieces = GameObject.FindGameObjectsWithTag("FloorPiece").Select(e => e.GetComponent<ColorCollisionObject>()).ToArray();
		foreach(var piece in allFloorPieces)
		{
			if(piece.meshCanBeOptimized)
				piece.renderer.enabled = false;
		}

		Messenger.Invoke(LevelStateMessage.LevelStarted.ToString());
		canPause = true;

		Messenger.RemoveListener(LevelIntroMessage.IntroFinished.ToString(), IntroFinished);
		Messenger.RemoveListener(LevelIntroMessage.IntroInterrupted.ToString(), IntroInterrupted);
		StateM.ChangeState(LevelState.InGame);
		playerChar.playerMovement.canMove = true;
		playerChar.EnablePhysics();
	}

	void IntroInterrupted()
	{
		Camera.main.GetComponent<CameraFollow>().enabled = true;
	}

	void OnDeserialized()
	{
		mapRoot = GameObject.Find("MapRoot");
	}

	public void SetInitialFloorColliders()
	{
		//Make sure all the triggers and such are turned on, then tell all the cubes to setup their colliders based on the players colour.
		Messenger<Colour>.Invoke(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerColour);
	}

	void SetupNullCube(GameObject nullCube)
	{
		nullCube.collider.enabled = true;
		nullCube.renderer.enabled = false;
		nullCube.GetComponent<BoxCollider>().size = new Vector3(1, 10, 1);
	}

	void SetupNullCubes()
	{
		var nullCubes = GameObject.FindGameObjectsWithTag("NullCube");

		foreach(var cube in nullCubes)
		{
			SetupNullCube(cube);
		}
	}

	public GameObject[] OptimiseLevelMesh()
	{
		List<GameObject> combinedMeshList = new List<GameObject>();
		var mapObjects = GameObject.Find("MapRoot").GetComponentsInChildren<ColorCollisionObject>().ToList();

		mapObjects = mapObjects.Where(e => e.meshCanBeOptimized).ToList();
		List<MeshFilter> meshFilters = new List<MeshFilter>();

		mapObjects.ForEach(e => meshFilters.AddRange(e.GetComponentsInChildren<MeshFilter>()));

		var uniqueMaterials = meshFilters.Select(e => e.renderer.sharedMaterial).Distinct();
		meshFilters.RemoveAll(m => m.name == "ImpassableCollider");

		foreach(var uniqueMat in uniqueMaterials)
		{
			var meshFiltersForMat = meshFilters.Where(meshFilter => meshFilter.renderer.sharedMaterial == uniqueMat).ToArray();

			var combine = new CombineInstance[meshFiltersForMat.Length];

			int layerForThisMesh = 1;

			for(int i = 0; i < meshFiltersForMat.Length; i++)
			{
				combine[i].mesh = meshFiltersForMat[i].sharedMesh;
				combine[i].transform = meshFiltersForMat[i].transform.localToWorldMatrix;
			}
			layerForThisMesh = meshFiltersForMat[0].gameObject.layer;

			var newMeshObject = new GameObject("CombinedMesh: " + uniqueMat.name.Replace("(Instance)", ""));
			newMeshObject.transform.position = Vector3.zero;
			newMeshObject.layer = layerForThisMesh; 
			newMeshObject.tag = "CombinedMesh";

			var newMeshFilter = newMeshObject.AddComponent<MeshFilter>();
			newMeshFilter.mesh = new Mesh();
			newMeshFilter.mesh.CombineMeshes(combine);

			var newMeshRenderer = newMeshObject.AddComponent<MeshRenderer>();
			newMeshRenderer.material = uniqueMat;
			if(uniqueMat.name == "NullCubeMat")
			{
				newMeshRenderer.enabled = false;
			}
			combinedMeshList.Add(newMeshObject);
		}

		return combinedMeshList.ToArray();
	}

	void CreateFloor()
	{
		GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
		floor.transform.parent = mapRoot.transform;

		floor.transform.localPosition = new Vector3(0, 0.55f, 0);

		floor.transform.localScale = new Vector3(10000, 0.01f, 10000);
		floor.transform.localEulerAngles = Vector3.zero;

		
		//var info = floor.AddComponent<StoreInformation>();

		floor.renderer.enabled = false;
		floor.collider.material = floorPhysicMat;
	}

	void CreatePlayer()
	{
		var playerStart = GameObject.Find("PlayerStartCube");

		var playerPrefab = (GameObject)Resources.Load("Player");

		var playerCube = (GameObject)Instantiate(playerPrefab);
		playerCube.name = playerCube.name.Replace ("(Clone)", string.Empty);
		var playerPos = playerStart.transform.position;

		playerPos.y += 1.01f;

		playerCube.transform.position = playerPos;

		Resources.UnloadUnusedAssets();

		playerChar = playerCube.GetComponent<PlayerCharacter> ();

		playerChar.SilentlyChangeColour(playerStart.GetComponent<PlayerStartPiece>().objColour);
	}

	void DestroyCombinedMeshes()
	{
		if(combinedMeshes == null)
		{
			combinedMeshes = GameObject.FindGameObjectsWithTag("CombinedMesh");
		}
		if(combinedMeshes != null)
		{
			if(combinedMeshes.Length > 0)
			{
				foreach(var cMesh in combinedMeshes)
				{
					Destroy(cMesh);
				}
			}
		}
	}

	void CutSceneStarted()
	{
		canPause = false;
		playerChar.playerMovement.canMove = false;
		Camera.main.GetComponent<CameraFollow>().enabled = false;
	}

	void CutSceneFinished()
	{
		canPause = true;
		playerChar.playerMovement.canMove = true;
		Camera.main.GetComponent<CameraFollow>().enabled = true;
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(canPause)
			{
				if(isPaused)
					StateMachine<LevelState, LevelStateMessage>.ChangeState(LevelState.InGame);
				else
					StateMachine<LevelState, LevelStateMessage>.ChangeState(LevelState.Pause);
			}
		}
	}
	
	public void FinishLevel()
	{
		StateMachine<LevelState, LevelStateMessage>.ChangeState(LevelState.EndGame);
	}

	public void ResetLevel()
	{
		Debug.Log("RESETTING LEVEL!");

		if(Application.loadedLevelName == "LevelCreator")
		{
			LevelTestStateController.Instance.LoadCheckpoint();
		}
		else
		{
			//LevelSerializer has some odd behavior when your trying to load in objects that already exist
			//We have to do some tidy up before we reset the level.
			DestroyCombinedMeshes();
			Destroy(playerChar.gameObject);
			Destroy(mapRoot);
			LevelStateController.Instance.LoadInitialState();
			StateMachine<LevelState, LevelStateMessage>.ChangeState(LevelState.InGame);
		}
	}

	public void SetCheckpoint()
	{
		hasCheckpoint = true;
		LevelStateController.Instance.SetCheckPoint();

		if(isStoryMode)
		{
			StoryProgressController.Instance.SetStoryProgressSave();
		}
	}

	public void LoadCheckpoint()
	{
		Debug.Log("LOADING CHECKPOINT!");
		if(Application.loadedLevelName == "LevelCreator")
		{
			LevelTestStateController.Instance.LoadCheckpoint();
		}
		else
		{
			LevelStateController.Instance.LoadCheckpoint(delegate(GameObject arg1, List<GameObject> arg2)
			{
				InitLevel(false);
				hasCheckpoint = true;
			});
		}
	}

	public void SetCheckpointLevelTest()
	{

	}

	void PlayerChangedColour(Colour colour)
	{
		var colourIndex = (int)colour;

		colourIndex++;

		if (colourIndex > ColorManager.cachedColourValues.Length)
			colourIndex = 1;

		if (colourIndex == 0)
			colourIndex = 1;

		RenderSettings.skybox = skyboxes[colourIndex - 2];
	}

	void PlayerKilled()
	{
		if(hasCheckpoint)
			LoadCheckpoint();
		else
			ResetLevel();
	}

	#region State Changes
	
	void InGameEnter(StateM.StateChangeData changeData)
	{
		if(playerChar != null)
			playerChar.playerMovement.canMove = true;
		isPaused = false;
	}
	
	void InGameExit(StateM.StateChangeData changeData)
	{
		
	}
	
	void PauseEnter(StateM.StateChangeData changeData)
	{
		isPaused = true;
		playerChar.playerMovement.canMove = false;
		Time.timeScale = 0;
	}
	
	void PauseExit(StateM.StateChangeData changeData)
	{
		Time.timeScale = 1;
	}
	
	void EndGameEnter(StateM.StateChangeData changeData)
	{
		canPause = false;
		Time.timeScale = 0;
		playerChar.playerMovement.canMove = false;
	}
	
	void EndGameExit(StateM.StateChangeData changeData)
	{
		canPause = true;
	}

	void CutSceneEnter(StateM.StateChangeData changeData)
	{

	}

	void CutSceneExit(StateM.StateChangeData changeData)
	{

	}
	
	#endregion
}
