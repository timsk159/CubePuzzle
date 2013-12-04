using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using StateM = StateMachine<LevelState, LevelStateNotification>;

public enum LevelState
{
	InGame, Pause, EndGame, CutScene, ExitingLevel
};

public enum LevelStateNotification
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
				Debug.LogWarning("Tried to access player colour, but playerchar was null!");
			}

			return _playerColour;
		}
	}	

	private bool _isStoryMode;

	public bool isStoryMode
	{
		get
		{
			if(Application.loadedLevelName == "UserLevelScene")
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
	
	void Awake()
	{
		RegisterStates();
		levelIntro = GetComponent<LevelIntro>();
		if(levelIntro == null)
		{
			levelIntro = gameObject.AddComponent<LevelIntro>();
		}
	}
	
	void RegisterStates()
	{
		StateMachine<LevelState, LevelStateNotification>.RegisterState(LevelState.InGame, LevelStateNotification.InGameEnter, LevelStateNotification.InGameExit); 
		StateMachine<LevelState, LevelStateNotification>.RegisterState(LevelState.Pause, LevelStateNotification.PauseEnter, LevelStateNotification.PauseExit); 
		StateMachine<LevelState, LevelStateNotification>.RegisterState(LevelState.EndGame, LevelStateNotification.EndGameEnter, LevelStateNotification.EndGameExit);
		StateMachine<LevelState, LevelStateNotification>.RegisterState(LevelState.CutScene, LevelStateNotification.CutSceneEnter, LevelStateNotification.CutSceneExit);

		Messenger<StateM.StateChangeData>.AddListener(LevelStateNotification.InGameEnter.ToString(), InGameEnter);
		Messenger<StateM.StateChangeData>.AddListener(LevelStateNotification.InGameExit.ToString(), InGameExit);

		Messenger<StateM.StateChangeData>.AddListener(LevelStateNotification.PauseEnter.ToString(), PauseEnter);
		Messenger<StateM.StateChangeData>.AddListener(LevelStateNotification.PauseExit.ToString(), PauseExit);

		Messenger<StateM.StateChangeData>.AddListener(LevelStateNotification.EndGameEnter.ToString(), EndGameEnter);
		Messenger<StateM.StateChangeData>.AddListener(LevelStateNotification.EndGameExit.ToString(), EndGameExit);

		Messenger<StateM.StateChangeData>.AddListener(LevelStateNotification.CutSceneEnter.ToString(), CutSceneEnter);
		Messenger<StateM.StateChangeData>.AddListener(LevelStateNotification.CutSceneExit.ToString(), CutSceneExit);
		
		StateMachine<LevelState, LevelStateNotification>.SetInitialState(LevelState.InGame);
	}

	void Start()
	{
		if(isStoryMode)
		{
			Messenger.AddListener(CutSceneNotification.CutSceneStarted.ToString(), CutSceneStarted);
			Messenger.AddListener(CutSceneNotification.CutSceneFinished.ToString(), CutSceneFinished);
		}
	}

	void OnDestroy()
	{
		Messenger<StateM.StateChangeData>.RemoveListener(LevelStateNotification.InGameEnter.ToString(), InGameEnter);
		Messenger<StateM.StateChangeData>.RemoveListener(LevelStateNotification.InGameExit.ToString(), InGameExit);

		Messenger<StateM.StateChangeData>.RemoveListener(LevelStateNotification.PauseEnter.ToString(), PauseEnter);
		Messenger<StateM.StateChangeData>.RemoveListener(LevelStateNotification.PauseExit.ToString(), PauseExit);

		Messenger<StateM.StateChangeData>.RemoveListener(LevelStateNotification.EndGameEnter.ToString(), EndGameEnter);
		Messenger<StateM.StateChangeData>.RemoveListener(LevelStateNotification.EndGameExit.ToString(), EndGameExit);

		Messenger<StateM.StateChangeData>.RemoveListener(LevelStateNotification.CutSceneEnter.ToString(), CutSceneEnter);
		Messenger<StateM.StateChangeData>.RemoveListener(LevelStateNotification.CutSceneExit.ToString(), CutSceneExit);

		Messenger.RemoveListener(CutSceneNotification.CutSceneStarted.ToString(), CutSceneStarted);
		Messenger.RemoveListener(CutSceneNotification.CutSceneFinished.ToString(), CutSceneFinished);

		Messenger.RemoveListener(LevelIntroNotification.IntroFinished.ToString(), IntroFinished);
		Messenger.RemoveListener(LevelIntroNotification.IntroInterrupted.ToString(), IntroInterrupted);
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

		mapRoot = GameObject.Find("MapRoot");
		SetupNullCubes();

		Camera.main.GetComponent<CameraFollow>().target = playerObj.transform;

		combinedMeshes = OptimiseLevelMesh();
		foreach(var go in combinedMeshes)
		{
			go.renderer.enabled = false;
		}

		Messenger.Invoke(LevelStateNotification.LevelInitialized.ToString());

		if(playIntro)
		{
			StartGameAfterIntro(introCutsceneObj);
		}
		else
			IntroFinished();
	}

	//Just used to save us having to find the combined meshes after the intro animation.
	GameObject[] combinedMeshes;

	void StartGameAfterIntro(CutSceneObj introCutsceneObj = null)
	{
		Messenger.AddListener(LevelIntroNotification.IntroFinished.ToString(), IntroFinished);
		Messenger.AddListener(LevelIntroNotification.IntroInterrupted.ToString(), IntroInterrupted);
		Camera.main.GetComponent<CameraFollow>().PutCameraBehindPlayer();
		playerChar.playerMovement.canMove = false;
		playerChar.rigidbody.useGravity = false;
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

		Messenger.Invoke(LevelStateNotification.LevelStarted.ToString());

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
		Messenger<Colour>.Invoke(ColourCollisionNotification.PlayerChangedColour.ToString(), PlayerColour);
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

	GameObject[] OptimiseLevelMesh()
	{
		List<GameObject> combinedMeshes = new List<GameObject>();
		var mapObjects = GameObject.Find("MapRoot").GetComponentsInChildren<ColorCollisionObject>().ToList();

		mapObjects = (List<ColorCollisionObject>)mapObjects.Where(e => e.meshCanBeOptimized).ToList();
		List<MeshFilter> meshFilters = new List<MeshFilter>();
		mapObjects.ForEach(e => meshFilters.AddRange(e.GetComponentsInChildren<MeshFilter>()));

		var uniqueMaterials = meshFilters.Select(e => e.renderer.sharedMaterial).Distinct();
		foreach(var uniqueMat in uniqueMaterials)
		{
			var meshFiltersForMat = meshFilters.Where(e => e.renderer.sharedMaterial == uniqueMat).ToArray();

			var combine = new CombineInstance[meshFiltersForMat.Length];

			int layerForThisMesh = 1;

			for(int i = 0; i < meshFiltersForMat.Length; i++)
			{
				combine[i].mesh = meshFiltersForMat[i].sharedMesh;
				combine[i].transform = meshFiltersForMat[i].transform.localToWorldMatrix;
				if(layerForThisMesh != meshFilters[i].gameObject.layer)
					layerForThisMesh = meshFilters[i].gameObject.layer;
			}

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
			combinedMeshes.Add(newMeshObject);
		}

		return combinedMeshes.ToArray();
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
					StateMachine<LevelState, LevelStateNotification>.ChangeState(LevelState.InGame);
				else
					StateMachine<LevelState, LevelStateNotification>.ChangeState(LevelState.Pause);
			}
		}
	}
	
	public void FinishLevel()
	{
		StateMachine<LevelState, LevelStateNotification>.ChangeState(LevelState.EndGame);
	}

	public void ResetLevel()
	{
		//LevelSerializer has some odd behavior when your trying to load in objects that already exist
		//We have to do some tidy up before we reset the level.
		DestroyCombinedMeshes();
		Destroy(playerChar.gameObject);
		Destroy(mapRoot);
		LevelStateController.Instance.LoadInitialState();
		StateMachine<LevelState, LevelStateNotification>.ChangeState(LevelState.InGame);
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
		LevelStateController.Instance.LoadCheckpoint(delegate(GameObject arg1, List<GameObject> arg2) {
			InitLevel(false);
			hasCheckpoint = true;
	});
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
		
	}

	void CutSceneEnter(StateM.StateChangeData changeData)
	{

	}

	void CutSceneExit(StateM.StateChangeData changeData)
	{

	}
	
	#endregion
}
