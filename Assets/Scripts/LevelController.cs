using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum LevelState
{
	InGame, Pause, EndGame
};

public enum LevelStateNotification
{
	InGameEnter, InGameExit, PauseEnter, PauseExit, EndGameEnter, EndGameExit
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
			_playerColour = playerChar.currentColor;

			return _playerColour;
		}
	}	

	LevelStateController levelStateController;
	
	public bool isStoryMode = false;
	
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
		
		StateMachine<LevelState, LevelStateNotification>.StateNotificationCenter.AddObserver(this, LevelStateNotification.InGameEnter);
		StateMachine<LevelState, LevelStateNotification>.StateNotificationCenter.AddObserver(this, LevelStateNotification.InGameExit);
		
		StateMachine<LevelState, LevelStateNotification>.StateNotificationCenter.AddObserver(this, LevelStateNotification.PauseEnter);
		StateMachine<LevelState, LevelStateNotification>.StateNotificationCenter.AddObserver(this, LevelStateNotification.PauseExit);
		
		StateMachine<LevelState, LevelStateNotification>.StateNotificationCenter.AddObserver(this, LevelStateNotification.EndGameEnter);
		StateMachine<LevelState, LevelStateNotification>.StateNotificationCenter.AddObserver(this, LevelStateNotification.EndGameExit);
		
		StateMachine<LevelState, LevelStateNotification>.SetInitialState(LevelState.InGame);
	}
	
	void Start()
	{
		if(Application.loadedLevelName == "UserCreatedLevel")
			isStoryMode = false;

		levelStateController = GetComponent<LevelStateController>();
	}
	
	public void InitLevel()
	{
		var playerObj = GameObject.FindWithTag ("Player");

		if(playerObj != null)
		{
			playerChar = playerObj.GetComponent<PlayerCharacter>();
		}

		SetInitialFloorColliders();

		Camera.main.GetComponent<CameraFollow>().target = playerObj.transform;

		if(levelStateController != null)
			levelStateController.SetInitialState();
	}

	public void SetInitialFloorColliders()
	{
		NotificationCenter<ColourCollisionNotification>.DefaultCenter.PostNotification(ColourCollisionNotification.PlayerChangedColour, PlayerColour);



		/*
		FloorPiece[] floorPieces = GameObject.FindGameObjectsWithTag("FloorPiece").ToList().Where(e => e.GetComponent<FloorPiece>() != null).Select(
			e => e.GetComponent<FloorPiece>()).ToArray();

		ButtonPiece[] buttonPieces = GameObject.FindGameObjectsWithTag("ButtonPiece").ToList().Where(e => e.GetComponent<ButtonPiece>() != null).Select(
			e => e.GetComponent<ButtonPiece>()).ToArray();

		DoorPiece[] doorPieces = GameObject.FindGameObjectsWithTag("DoorPiece").ToList().Where(e => e.GetComponent<DoorPiece>() != null).Select(
			e => e.GetComponent<DoorPiece>()).ToArray();

		var checkPoints = GameObject.FindGameObjectsWithTag("FloorPiece").Where(e => e.name == "CheckpointCube").ToArray();

		foreach(var floorPiece in floorPieces)
		{
			var thisCollider = floorPiece.collider as BoxCollider;
			thisCollider.enabled = true;
			if(floorPiece.objColour == PlayerColour)
			{
				var newColliderSize = new Vector3(thisCollider.size.x, 10, thisCollider.size.z);
				
				thisCollider.size = newColliderSize;		
			}
			foreach(Transform child in floorPiece.transform)
			{
				if(child.collider)
					child.collider.enabled = true;
			}
		}
		foreach(var buttonPiece in buttonPieces)
		{
			var thisCollider = buttonPiece.collider as BoxCollider;
			thisCollider.enabled = true;
			foreach(Transform child in buttonPiece.transform)
			{
				if(child.collider)
					child.collider.enabled = true;
			}
		}
		foreach(var doorPiece in doorPieces)
		{
			var thisCollider = doorPiece.collider as BoxCollider;
			thisCollider.enabled = true;
			foreach(Transform child in doorPiece.transform)
			{
				if(child.collider)
					child.collider.enabled = true;
			}
		}
		foreach(var checkpointCube in checkPoints)
		{
			var thisCollider = checkpointCube.collider as BoxCollider;
			thisCollider.enabled = true;
			foreach(Transform child in checkpointCube.transform)
			{
				if(child.collider)
					child.collider.enabled = true;
			}
		}
*/
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
	
	#endregion
}
