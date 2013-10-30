using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class LevelStateController : MonoBehaviour 
{
	public static string currentLevelName;

	public void SetInitialState()
	{
	//	LevelSerializer.SaveGame("InitialState");
	}

	public void SetCheckPoint()
	{
		LevelSerializer.Checkpoint();
	}


	public void LoadInitialState()
	{
		/*
		var savedGames = LevelSerializer.SavedGames[LevelSerializer.PlayerName];
		var initialSaveData = savedGames.Where(e => e.Name == "InitialState").FirstOrDefault().Data;

		LevelSerializer.LoadSavedLevelIfSameScene(initialSaveData, onComplete);
		StateMachine<LevelState, LevelStateNotification>.ChangeState(LevelState.InGame) ;
		*/
		if(LevelController.Instance.isStoryMode)
		{
			SceneLoader.Instance.LoadLevel(Application.loadedLevelName, delegate {
				LevelController.Instance.InitLevel(true);
			});
		}
		else
		{
			LevelSerializer.LoadObjectTreeFromFile(currentLevelName, delegate(LevelLoader obj)
			{
				LevelController.Instance.InitLevel(true);
			});
		}
	}

	public void LoadCheckpoint(Action<GameObject, List<GameObject>> onComplete = null)
	{
		LevelSerializer.Resume(onComplete);
		StateMachine<LevelState, LevelStateNotification>.ChangeState(LevelState.InGame);
	}
}
