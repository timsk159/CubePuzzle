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
		LevelSerializer.Checkpoint();
	}


	public void LoadInitialState()
	{
		if(LevelController.Instance.isStoryMode)
		{
			var smoke = GameObject.Find("SmokeTrail");
			if(smoke)
				Destroy(smoke);

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

		LevelSerializer.Resume(onComplete);
		StateMachine<LevelState, LevelStateMessage>.ChangeState(LevelState.InGame);
	}
}
