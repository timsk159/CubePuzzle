using UnityEngine;
using System.Collections;

using StateMachineMessenger = Messenger<StateMachine<LevelState, LevelStateMessage>.StateChangeData>;

public class PauseMenuController : MonoBehaviour 
{
	public GameObject pauseMenuPanel;
	public GameObject controlsPanel;

	GameObject quitbutton;
	GameObject stopTestingButton;

	void Start()
	{
		quitbutton = pauseMenuPanel.transform.Find("QuitAndSaveButton").gameObject;
		stopTestingButton = pauseMenuPanel.transform.Find("StopTestingButton").gameObject;

		StateMachineMessenger.AddListener(LevelStateMessage.PauseEnter.ToString(), PauseEnter);
		StateMachineMessenger.AddListener(LevelStateMessage.PauseExit.ToString(), PauseExit);

		Messenger.AddListener(PauseMenuMessage.ResumeButtonClicked.ToString(), ResumeButtonClicked);
		Messenger.AddListener(PauseMenuMessage.QuitButtonClicked.ToString(), QuitButtonClicked);
		Messenger.AddListener(PauseMenuMessage.RestartButtonClicked.ToString(), RestartButtonClicked);
		Messenger.AddListener(PauseMenuMessage.ReloadCheckpointClicked.ToString(), ReloadCheckpointClicked);
		Messenger.AddListener(PauseMenuMessage.ControlsPressed.ToString(), ControlsPressed);
		Messenger.AddListener(FrontMenuUIMessage.ControlsBackPressed.ToString(), ControlsBackPressed);

		if (Application.loadedLevelName == "LevelCreator")
			SetForLevelTesting();
		else
			SetForNonLevelTesting();
	}

	void OnDestroy()
	{
		StateMachineMessenger.RemoveListener(LevelStateMessage.PauseEnter.ToString(), PauseEnter);
		StateMachineMessenger.RemoveListener(LevelStateMessage.PauseExit.ToString(), PauseExit);

		Messenger.RemoveListener(PauseMenuMessage.ResumeButtonClicked.ToString(), ResumeButtonClicked);
		Messenger.RemoveListener(PauseMenuMessage.QuitButtonClicked.ToString(), QuitButtonClicked);
		Messenger.RemoveListener(PauseMenuMessage.RestartButtonClicked.ToString(), RestartButtonClicked);
		Messenger.RemoveListener(PauseMenuMessage.ReloadCheckpointClicked.ToString(), ReloadCheckpointClicked);
		Messenger.RemoveListener(PauseMenuMessage.ControlsPressed.ToString(), ControlsPressed);
		Messenger.RemoveListener(FrontMenuUIMessage.ControlsBackPressed.ToString(), ControlsBackPressed);
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
		StateMachine<LevelState, LevelStateMessage>.ChangeState(LevelState.InGame);
		//LevelController.Instance.ResetLevel();
		StartCoroutine(ResetLevelAfterFrame());
	}

	IEnumerator ResetLevelAfterFrame()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		LevelController.Instance.ResetLevel();
	}

	IEnumerator LoadCheckpointAfterFrame()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		LevelController.Instance.LoadCheckpoint();
	}

	void ReloadCheckpointClicked()
	{
		if(LevelController.Instance.hasCheckpoint)
			StartCoroutine(LoadCheckpointAfterFrame());
		else
			StartCoroutine(ResetLevelAfterFrame());
	}

	void ControlsPressed()
	{
		NGUITools.SetActive(pauseMenuPanel, false);
		NGUITools.SetActive(controlsPanel, true);
	}

	void ControlsBackPressed()
	{
		NGUITools.SetActive(pauseMenuPanel, true);
		NGUITools.SetActive(controlsPanel, false);
	}
	
	void QuitButtonClicked()
	{
		SceneLoader.Instance.LoadLevel("FrontMenu");
		if(Time.timeScale != 1)
			Time.timeScale = 1;
	}

	void SetForLevelTesting()
	{
		quitbutton.SetActive(false);
		stopTestingButton.SetActive(true);
	}

	void SetForNonLevelTesting()
	{
		quitbutton.SetActive(true);
		stopTestingButton.SetActive(false);
	}
}
