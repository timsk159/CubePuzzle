using UnityEngine;
using System.Collections;
using System.Linq;

public class LevelStateController : MonoBehaviour 
{
	public void SetInitialState()
	{
		LevelSerializer.SaveGame("InitialState");
	}

	public void SetCheckPoint()
	{
		LevelSerializer.Checkpoint();
	}


	public void LoadInitialState()
	{
		var savedGames = LevelSerializer.SavedGames[LevelSerializer.PlayerName];
		var initialSaveData = savedGames.Where(e => e.Name == "InitialState").FirstOrDefault().Data;
	//	LevelSerializer.LoadSavedLevel(initialSaveData, LevelController.Instance.LoadedSaveComplete);
		LevelSerializer.LoadSavedLevel(initialSaveData);
		StateMachine<LevelState, LevelStateNotification>.ChangeState(LevelState.InGame) ;
	}

	public void LoadCheckpoint()
	{
		LevelSerializer.Resume();
		StateMachine<LevelState, LevelStateNotification>.ChangeState(LevelState.InGame);
	}
}
