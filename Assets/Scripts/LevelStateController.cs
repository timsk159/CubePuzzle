using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class LevelStateController : MonoSingleton<LevelStateController> 
{
	public static string currentLevelName;

	public string checkpointSave;

	public void SetCheckPoint()
	{
		//LevelSerializer.Checkpoint();
		if (!Application.isWebPlayer)
		{
			var saves = LevelSerializer.SavedGames[LevelSerializer.PlayerName];
			var data = saves.Where(e => e.Name == "Checkpoint").FirstOrDefault();
			if (data != null)
			{
				saves.RemoveAt(saves.IndexOf(data));
			}

			LevelSerializer.SaveGame("Checkpoint");
		}
		else
			checkpointSave = LevelSerializer.SerializeLevel();
	}


	public void LoadInitialState()
	{
		if(LevelController.Instance.isStoryMode)
		{
			//LoadCheckpoint(delegate
			//{
			//	LevelController.Instance.InitLevel(false);
			//});
		
			SceneLoader.Instance.LoadLevel(Application.loadedLevelName, delegate {
				LevelController.Instance.InitLevel(false);
				LevelController.Instance.playerChar.canReset = true;
			});
		}
		else
		{
			var smoke = GameObject.Find("SmokeTrail");
			if(smoke)
				Destroy(smoke);

			LevelSerializer.LoadObjectTreeFromFile(currentLevelName, delegate(LevelLoader obj)
			{
				LevelController.Instance.InitLevel(false);
				LevelController.Instance.playerChar.canReset = true;
				
			});
		}
	}

	public void LoadCheckpoint(Action<GameObject, List<GameObject>> onComplete = null)
	{
		var smoke = GameObject.Find("SmokeTrail");
		if(smoke)
			Destroy(smoke);

		if (!Application.isWebPlayer)
		{
			//LevelSerializer.Resume(onComplete);
			var saves = LevelSerializer.SavedGames[LevelSerializer.PlayerName];
			var data = saves.Where(e => e.Name == "Checkpoint").FirstOrDefault();
			LevelSerializer.LoadSavedLevel(data.Data, onComplete);
		}
		else
		{
			LevelSerializer.LoadSavedLevel(checkpointSave, onComplete);
		}

		StateMachine<LevelState, LevelStateMessage>.ChangeState(LevelState.InGame);
	}
}
