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
		StateMachine<LevelState, LevelStateMessage>.ChangeState(LevelState.InGame);
	}
}
