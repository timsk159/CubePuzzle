using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class LevelStateController : MonoBehaviour 
{
	public void SetInitialState()
	{
		Debug.Log("Set initial state");
		LevelSerializer.SaveGame("InitialState");
	}

	public void SetCheckPoint()
	{
		LevelSerializer.Checkpoint();
	}


	public void LoadInitialState(Action<GameObject, List<GameObject>> onComplete = null)
	{
		var savedGames = LevelSerializer.SavedGames[LevelSerializer.PlayerName];
		var initialSaveData = savedGames.Where(e => e.Name == "InitialState").FirstOrDefault().Data;
		if(onComplete != null)
		{
			LevelSerializer.LoadSavedLevel(initialSaveData, onComplete);
		}
		else
		{
			LevelSerializer.LoadSavedLevel(initialSaveData);
		}
		StateMachine<LevelState, LevelStateNotification>.ChangeState(LevelState.InGame) ;
	}

	public void LoadCheckpoint(Action<GameObject, List<GameObject>> onComplete = null)
	{
		LevelSerializer.Resume(onComplete);
		StateMachine<LevelState, LevelStateNotification>.ChangeState(LevelState.InGame);
	}
}
