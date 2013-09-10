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
			catch(NullReferenceException nre)
			{
				Debug.LogWarning("Tried to access player colour, but playerchar was null!");
			}

			return _playerColour;
		}
	}	

	LevelStateController levelStateController;
	
	public bool isStoryMode = false;

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
		if(Application.loadedLevelName == "UserCreatedLevel")
			isStoryMode = false;
		else 
			isStoryMode = true;

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

		Camera.main.GetComponent<CameraFollow>().target = playerObj.transform;

		if(levelStateController != null)
		{
			levelStateController.SetInitialState();
		}
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
		levelStateController.LoadInitialState();
	}

	public void SetCheckpoint()
	{
		hasCheckpoint = true;
		levelStateController.SetCheckPoint();
	}

	public void LoadCheckpoint()
	{
		levelStateController.LoadCheckpoint();
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
