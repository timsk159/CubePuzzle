using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum LevelState
{
	InGame, Pause, EndGame, CutScene
};

public enum LevelStateNotification
{
	InGameEnter, InGameExit, PauseEnter, PauseExit, EndGameEnter, EndGameExit, CutSceneEnter, CutSceneExit
};

public class LevelController : MonoBehaviour 
{
	private static LevelController _instance;

	public static LevelController Instance 
	{
		get
		{
			return _instance;
		}
	}
	
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

	LevelStateController levelStateController;

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
	
	void Awake()
	{
		_instance = this;
		RegisterStates();
	}
	
	void RegisterStates()
	{
		StateMachine<LevelState, LevelStateNotification>.RegisterState(LevelState.InGame, LevelStateNotification.InGameEnter, LevelStateNotification.InGameExit); 
		StateMachine<LevelState, LevelStateNotification>.RegisterState(LevelState.Pause, LevelStateNotification.PauseEnter, LevelStateNotification.PauseExit); 
		StateMachine<LevelState, LevelStateNotification>.RegisterState(LevelState.EndGame, LevelStateNotification.EndGameEnter, LevelStateNotification.EndGameExit);
		StateMachine<LevelState, LevelStateNotification>.RegisterState(LevelState.CutScene, LevelStateNotification.CutSceneEnter, LevelStateNotification.CutSceneExit);
		
		StateMachine<LevelState, LevelStateNotification>.StateNotificationCenter.AddObserver(this, LevelStateNotification.InGameEnter);
		StateMachine<LevelState, LevelStateNotification>.StateNotificationCenter.AddObserver(this, LevelStateNotification.InGameExit);
		
		StateMachine<LevelState, LevelStateNotification>.StateNotificationCenter.AddObserver(this, LevelStateNotification.PauseEnter);
		StateMachine<LevelState, LevelStateNotification>.StateNotificationCenter.AddObserver(this, LevelStateNotification.PauseExit);
		
		StateMachine<LevelState, LevelStateNotification>.StateNotificationCenter.AddObserver(this, LevelStateNotification.EndGameEnter);
		StateMachine<LevelState, LevelStateNotification>.StateNotificationCenter.AddObserver(this, LevelStateNotification.EndGameExit);

		StateMachine<LevelState, LevelStateNotification>.StateNotificationCenter.AddObserver(this, LevelStateNotification.CutSceneEnter);
		StateMachine<LevelState, LevelStateNotification>.StateNotificationCenter.AddObserver(this, LevelStateNotification.CutSceneExit);
		
		StateMachine<LevelState, LevelStateNotification>.SetInitialState(LevelState.InGame);
	}

	void Start()
	{
		levelStateController = GetComponent<LevelStateController>();

		if(isStoryMode)
		{
			NotificationCenter<CutSceneNotification>.DefaultCenter.AddObserver(this, CutSceneNotification.CutSceneStarted);
			NotificationCenter<CutSceneNotification>.DefaultCenter.AddObserver(this, CutSceneNotification.CutSceneFinished);
		}
	}
	
	public void InitLevel()
	{
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

		SetupNullCubes();
		SetInitialFloorColliders();
		OptimiseLevelMesh();

		Camera.main.GetComponent<CameraFollow>().target = playerObj.transform;
		if(levelStateController != null)
		{
			levelStateController.SetInitialState();
		}
	//	StartCoroutine(PlayIntroAnimation());
	}

	IEnumerator PlayIntroAnimation()
	{
		playerChar.playerMovement.canMove = false;
		Camera.main.GetComponent<CameraFollow>().enabled = false;

		Camera.main.animation.Play();

		yield return new WaitForSeconds(Camera.main.animation.clip.length);

		playerChar.playerMovement.canMove = true;
		Camera.main.GetComponent<CameraFollow>().enabled = true;
	}

	public void LoadedSaveComplete(GameObject rootObj, List<GameObject> mapObjects)
	{
		var playerObj = GameObject.FindWithTag ("Player");

		if(playerObj != null)
		{
			playerChar = playerObj.GetComponent<PlayerCharacter>();
		}

		SetupNullCubes();
		SetInitialFloorColliders();

		Camera.main.GetComponent<CameraFollow>().target = playerObj.transform;

		if(levelStateController != null)
		{
			levelStateController.SetInitialState();
		}
	}

	public void SetInitialFloorColliders()
	{
		NotificationCenter<ColourCollisionNotification>.DefaultCenter.PostNotification(ColourCollisionNotification.PlayerChangedColour, PlayerColour);
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

	void OptimiseLevelMesh()
	{
		var meshFilters = GameObject.Find("MapRoot").GetComponentsInChildren<MeshFilter>();

		var uniqueMaterials = meshFilters.Select(e => e.renderer.sharedMaterial).Distinct();
		foreach(var uniqueMat in uniqueMaterials)
		{
			if(!uniqueMat.name.Contains("Door"))
			{
				var meshFiltersForMat = meshFilters.Where(e => e.renderer.sharedMaterial == uniqueMat).ToArray();

				var combine = new CombineInstance[meshFiltersForMat.Length];

				int layerForThisMesh = 1;

				for(int i = 0; i < meshFiltersForMat.Length; i++)
				{
					combine[i].mesh = meshFiltersForMat[i].sharedMesh;
					combine[i].transform = meshFiltersForMat[i].transform.localToWorldMatrix;
					meshFiltersForMat[i].renderer.enabled = false;
					if(layerForThisMesh != meshFilters[i].gameObject.layer)
						layerForThisMesh = meshFilters[i].gameObject.layer;
				}

				var newMeshObject = new GameObject("CombinedMesh: " + uniqueMat.name.Replace("(Instance)", ""));
				newMeshObject.transform.position = Vector3.zero;
				newMeshObject.layer = layerForThisMesh;

				var newMeshFilter = newMeshObject.AddComponent<MeshFilter>();
				newMeshFilter.mesh = new Mesh();
				newMeshFilter.mesh.CombineMeshes(combine);

				var newMeshRenderer = newMeshObject.AddComponent<MeshRenderer>();
				newMeshRenderer.material = uniqueMat;
				if(uniqueMat.name == "NullCubeMat")
				{
					newMeshRenderer.enabled = false;
				}

			}
		}
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

		playerChar.ChangeColour(playerStart.GetComponent<PlayerStartPiece>().objColour);
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
		levelStateController.LoadInitialState(delegate(GameObject arg1, List<GameObject> arg2) {
			OptimiseLevelMesh();
	});
	}

	public void SetCheckpoint()
	{
		hasCheckpoint = true;
		levelStateController.SetCheckPoint();
	}

	public void LoadCheckpoint()
	{
		levelStateController.LoadCheckpoint(delegate(GameObject arg1, List<GameObject> arg2) {
			OptimiseLevelMesh();
	});
	}

	#region State Changes
	
	void InGameEnter()
	{
		if(playerChar != null)
			playerChar.playerMovement.canMove = true;
		isPaused = false;
	}
	
	void InGameExit()
	{
		
	}
	
	void PauseEnter()
	{
		isPaused = true;
		playerChar.playerMovement.canMove = false;
		Time.timeScale = 0;
	}
	
	void PauseExit()
	{
		Time.timeScale = 1;
	}
	
	void EndGameEnter()
	{
		Time.timeScale = 0;
		playerChar.playerMovement.canMove = false;
	}
	
	void EndGameExit()
	{
		
	}

	void CutSceneEnter()
	{

	}

	void CutSceneExit()
	{

	}
	
	#endregion
}
