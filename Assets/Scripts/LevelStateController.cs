using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class LevelStateController : MonoSingleton<LevelStateController> 
{
	public static string currentLevelName;

	public void SetCheckPoint()
	{
		//LevelSerializer.Checkpoint();
		var saves = LevelSerializer.SavedGames[LevelSerializer.PlayerName];
		var data = saves.Where(e => e.Name == "Checkpoint").FirstOrDefault();
		if (data != null)
		{
			saves.RemoveAt(saves.IndexOf(data));
		}
		LevelSerializer.SaveGame("Checkpoint");
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
			});
		}
	}

	public void LoadCheckpoint(Action<GameObject, List<GameObject>> onComplete = null)
	{
		var smoke = GameObject.Find("SmokeTrail");
		if(smoke)
			Destroy(smoke);

		//LevelSerializer.Resume(onComplete);
		var saves = LevelSerializer.SavedGames[LevelSerializer.PlayerName];
		var data = saves.Where(e => e.Name == "Checkpoint").FirstOrDefault();
		LevelSerializer.LoadSavedLevel(data.Data, onComplete);

		StateMachine<LevelState, LevelStateMessage>.ChangeState(LevelState.InGame);
	}
}
