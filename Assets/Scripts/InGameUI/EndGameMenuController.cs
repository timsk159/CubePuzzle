using UnityEngine;
using System.Collections;

using StateMachineMessenger = Messenger<StateMachine<LevelState, LevelStateMessage>.StateChangeData>;

public class EndGameMenuController : MonoBehaviour 
{
	public GameObject endGameMenuPanel;
	
	void Start()
	{ 
		StateMachineMessenger.AddListener(LevelStateMessage.EndGameEnter.ToString(), EndGameEnter);

		Messenger.AddListener(EndGameMenuMessage.NextLevelPressed.ToString(), NextLevelPressed);
		Messenger.AddListener(EndGameMenuMessage.QuitPressed.ToString(), QuitPressed);
	}

	void OnDestroy()
	{
		StateMachineMessenger.RemoveListener(LevelStateMessage.EndGameEnter.ToString(), EndGameEnter);

		Messenger.RemoveListener(EndGameMenuMessage.NextLevelPressed.ToString(), NextLevelPressed);
		Messenger.RemoveListener(EndGameMenuMessage.QuitPressed.ToString(), QuitPressed);
	}
	
	void EndGameEnter(StateMachine<LevelState, LevelStateMessage>.StateChangeData stateChangeData)
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
