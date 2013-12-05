using UnityEngine;
using System.Collections;

using StateMachineMessenger = Messenger<StateMachine<LevelState, LevelStateMessage>.StateChangeData>;

public class PauseMenuController : MonoBehaviour 
{
	public GameObject pauseMenuPanel;
	
	void Start()
	{
		StateMachineMessenger.AddListener(LevelStateMessage.PauseEnter.ToString(), PauseEnter);
		StateMachineMessenger.AddListener(LevelStateMessage.PauseExit.ToString(), PauseExit);

		Messenger.AddListener(PauseMenuMessage.ResumeButtonClicked.ToString(), ResumeButtonClicked);
		Messenger.AddListener(PauseMenuMessage.QuitButtonClicked.ToString(), QuitButtonClicked);
		Messenger.AddListener(PauseMenuMessage.RestartButtonClicked.ToString(), RestartButtonClicked);
		Messenger.AddListener(PauseMenuMessage.ReloadCheckpointClicked.ToString(), ReloadCheckpointClicked);
	}

	void OnDestroy()
	{
		StateMachineMessenger.RemoveListener(LevelStateMessage.PauseEnter.ToString(), PauseEnter);
		StateMachineMessenger.RemoveListener(LevelStateMessage.PauseExit.ToString(), PauseExit);

		Messenger.RemoveListener(PauseMenuMessage.ResumeButtonClicked.ToString(), ResumeButtonClicked);
		Messenger.RemoveListener(PauseMenuMessage.QuitButtonClicked.ToString(), QuitButtonClicked);
		Messenger.RemoveListener(PauseMenuMessage.RestartButtonClicked.ToString(), RestartButtonClicked);
		Messenger.RemoveListener(PauseMenuMessage.ReloadCheckpointClicked.ToString(), ReloadCheckpointClicked);
	}
	
	void PauseEnter(StateMachine<LevelState, LevelStateMessage>.StateChangeData stateChangeData)
	{
		NGUITools.SetActive(pauseMenuPanel, true);
	}
	
	void PauseExit(StateMachine<LevelState, LevelStateMessage>.StateChangeData stateChangeData)
	{
		NGUITools.SetActive(pauseMenuPanel, false);
	}
	
	void ResumeButtonClicked()
	{
		StateMachine<LevelState, LevelStateMessage>.ChangeState(LevelState.InGame);
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
