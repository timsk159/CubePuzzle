using UnityEngine;
using System.Collections;

using StateMachineMessenger = Messenger<StateMachine<LevelState, LevelStateNotification>.StateChangeData>;

public class PauseMenuController : MonoBehaviour 
{
	public GameObject pauseMenuPanel;
	
	void Start()
	{
		StateMachineMessenger.AddListener(LevelStateNotification.PauseEnter.ToString(), PauseEnter);
		StateMachineMessenger.AddListener(LevelStateNotification.PauseExit.ToString(), PauseExit);

		Messenger.AddListener(PauseMenuNotification.ResumeButtonClicked.ToString(), ResumeButtonClicked);
		Messenger.AddListener(PauseMenuNotification.QuitButtonClicked.ToString(), QuitButtonClicked);
		Messenger.AddListener(PauseMenuNotification.RestartButtonClicked.ToString(), RestartButtonClicked);
		Messenger.AddListener(PauseMenuNotification.ReloadCheckpointClicked.ToString(), ReloadCheckpointClicked);
	}

	void OnDestroy()
	{
		StateMachineMessenger.RemoveListener(LevelStateNotification.PauseEnter.ToString(), PauseEnter);
		StateMachineMessenger.RemoveListener(LevelStateNotification.PauseExit.ToString(), PauseExit);

		Messenger.RemoveListener(PauseMenuNotification.ResumeButtonClicked.ToString(), ResumeButtonClicked);
		Messenger.RemoveListener(PauseMenuNotification.QuitButtonClicked.ToString(), QuitButtonClicked);
		Messenger.RemoveListener(PauseMenuNotification.RestartButtonClicked.ToString(), RestartButtonClicked);
		Messenger.RemoveListener(PauseMenuNotification.ReloadCheckpointClicked.ToString(), ReloadCheckpointClicked);
	}
	
	void PauseEnter(StateMachine<LevelState, LevelStateNotification>.StateChangeData stateChangeData)
	{
		NGUITools.SetActive(pauseMenuPanel, true);
	}
	
	void PauseExit(StateMachine<LevelState, LevelStateNotification>.StateChangeData stateChangeData)
	{
		NGUITools.SetActive(pauseMenuPanel, false);
	}
	
	void ResumeButtonClicked()
	{
		StateMachine<LevelState, LevelStateNotification>.ChangeState(LevelState.InGame);
	}

	void RestartButtonClicked()
	{
		LevelController.Instance.ResetLevel();
	}

	void ReloadCheckpointClicked()
	{
		if(LevelController.Instance.hasCheckpoint)
			LevelController.Instance.LoadCheckpoint();
		else
			LevelController.Instance.ResetLevel();
	}
	
	void QuitButtonClicked()
	{
		SceneLoader.Instance.LoadLevel("FrontMenu");
		if(Time.timeScale != 1)
			Time.timeScale = 1;
	}
}
