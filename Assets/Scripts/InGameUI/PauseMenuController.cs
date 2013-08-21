using UnityEngine;
using System.Collections;

public class PauseMenuController : MonoBehaviour 
{
	public GameObject pauseMenuPanel;
	
	void Start()
	{
		StateMachine<LevelState, LevelStateNotification>.StateNotificationCenter.AddObserver(this, LevelStateNotification.PauseEnter);
		StateMachine<LevelState, LevelStateNotification>.StateNotificationCenter.AddObserver(this, LevelStateNotification.PauseExit);
		
		NotificationCenter<PauseMenuNotification>.DefaultCenter.AddObserver(this, PauseMenuNotification.ResumeButtonClicked);
		NotificationCenter<PauseMenuNotification>.DefaultCenter.AddObserver(this, PauseMenuNotification.QuitButtonClicked);
		NotificationCenter<PauseMenuNotification>.DefaultCenter.AddObserver(this, PauseMenuNotification.RestartButtonClicked);
	}
	
	void PauseEnter()
	{
		NGUITools.SetActive(pauseMenuPanel, true);
	}
	
	void PauseExit()
	{
		NGUITools.SetActive(pauseMenuPanel, false);
	}
	
	void ResumeButtonClicked()
	{
		StateMachine<LevelState, LevelStateNotification>.ChangeState(LevelState.InGame);
	}

	void RestartButtonClicked()
	{
		LevelController.Instance.LoadCheckpoint ();

		//LevelController.Instance.ResetLevel();
	}
	
	void QuitButtonClicked()
	{
		Application.LoadLevel("FrontMenu");
		if(Time.timeScale != 1)
			Time.timeScale = 1;
	}
}
