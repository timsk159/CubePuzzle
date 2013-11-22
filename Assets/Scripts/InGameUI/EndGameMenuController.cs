using UnityEngine;
using System.Collections;

using StateMachineMessenger = Messenger<StateMachine<LevelState, LevelStateNotification>.StateChangeData>;

public class EndGameMenuController : MonoBehaviour 
{
	public GameObject endGameMenuPanel;
	
	void Start()
	{ 
		StateMachineMessenger.AddListener(LevelStateNotification.EndGameEnter.ToString(), EndGameEnter);

		Messenger.AddListener(EndGameMenuNotification.NextLevelPressed.ToString(), NextLevelPressed);
		Messenger.AddListener(EndGameMenuNotification.QuitPressed.ToString(), QuitPressed);
	}

	void OnDestroy()
	{
		StateMachineMessenger.RemoveListener(LevelStateNotification.EndGameEnter.ToString(), EndGameEnter);

		Messenger.RemoveListener(EndGameMenuNotification.NextLevelPressed.ToString(), NextLevelPressed);
		Messenger.RemoveListener(EndGameMenuNotification.QuitPressed.ToString(), QuitPressed);
	}
	
	void EndGameEnter(StateMachine<LevelState, LevelStateNotification>.StateChangeData stateChangeData)
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
