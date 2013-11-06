using UnityEngine;
using System.Collections;



public class EndGameMenuController : MonoBehaviour 
{
	public GameObject endGameMenuPanel;
	
	void Start()
	{
		StateMachine<LevelState, LevelStateNotification>.StateNotificationCenter.AddObserver(this, LevelStateNotification.EndGameEnter);
		
		NotificationCenter<EndGameMenuNotification>.DefaultCenter.AddObserver(this, EndGameMenuNotification.NextLevelPressed);
		NotificationCenter<EndGameMenuNotification>.DefaultCenter.AddObserver(this, EndGameMenuNotification.QuitPressed); 
	}
	
	void EndGameEnter()
	{
		NGUITools.SetActive(endGameMenuPanel, true);
		
		if(!LevelController.Instance.isStoryMode)
		{
			var nextLevelButton = endGameMenuPanel.transform.Find("NextLevelButton").gameObject;
			
			NGUITools.SetActive(nextLevelButton, false);
		}
	}
	
	void NextLevelPressed()
	{
		var nextLevel = StoryProgressController.Instance.NextLevel.levelName;
		SceneLoader.Instance.LoadLevel(nextLevel, delegate {
			if(StoryProgressController.Instance.SavedLevel == null)
				StoryProgressController.Instance.SetStoryProgressSave();
			else if(StoryProgressController.Instance.CurrentLevel.levelNumber >= StoryProgressController.Instance.SavedLevel.levelNumber)
				StoryProgressController.Instance.SetStoryProgressSave();
			Time.timeScale = 1;
			LevelController.Instance.InitLevel(true,  StoryProgressController.Instance.SavedLevel.cutSceneObj);
	});
	}
	
	void QuitPressed()
	{
		SceneLoader.Instance.LoadLevel("FrontMenu");
		if(Time.timeScale != 1)
			Time.timeScale = 1;
	}
}
