using UnityEngine;
using System.Collections;

using StateMachineMessenger = Messenger<StateMachine<LevelState, LevelStateMessage>.StateChangeData>;

public class EndGameMenuController : MonoBehaviour 
{
	public GameObject endGameMenuPanel;
	public GameObject gameCompletePanel;

	public GameObject demoLabel;
	public GameObject gameCompleteLabel;
	public EndGameMenuUINotifier websiteButton;

	GameObject nextLevelButton;
	GameObject quitButton;
	GameObject stopTestingButton;
	
	void Start()
	{ 
		StateMachineMessenger.AddListener(LevelStateMessage.EndGameEnter.ToString(), EndGameEnter);

		Messenger.AddListener(EndGameMenuMessage.NextLevelPressed.ToString(), NextLevelPressed);
		Messenger.AddListener(EndGameMenuMessage.QuitPressed.ToString(), QuitPressed);
		Messenger.AddListener(EndGameMenuMessage.WebsiteButton.ToString(), WebsiteButton);
		nextLevelButton = endGameMenuPanel.transform.Find("NextLevelButton").gameObject;
		quitButton = endGameMenuPanel.transform.Find("QuitButton").gameObject;
		stopTestingButton = endGameMenuPanel.transform.Find("StopTestingButton").gameObject;
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

		if (Application.loadedLevelName == "LevelCreator")
			SetForLevelTesting();
		else
			SetForNonLevelTesting();

		if (!LevelController.Instance.isStoryMode)
		{
			NGUITools.SetActive(nextLevelButton, false);
		}
		else
		{
			if (Application.loadedLevelName != "LevelCreator")
			{
				if (IsGameComplete())
				{
					GameComplete();
				}
			}
		}
	}

	void WebsiteButton()
	{
		if (Application.isWebPlayer)
		{
			Application.ExternalEval("window.open('/cubaze.html')");
		}
		else
		{
			Application.OpenURL("www.smirkstudio.co.uk/cubaze.html");
		}
		SceneLoader.Instance.LoadLevel("FrontMenu");
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

	void SetForLevelTesting()
	{
		nextLevelButton.SetActive(false);
		quitButton.SetActive(false);
		stopTestingButton.SetActive(true);
	}

	void SetForNonLevelTesting()
	{
		nextLevelButton.SetActive(true);
		quitButton.SetActive(true);
		stopTestingButton.SetActive(false);
	}

	void QuitPressed()
	{
		SceneLoader.Instance.LoadLevel("FrontMenu");
		if(Time.timeScale != 1)
			Time.timeScale = 1;
	}

	bool IsGameComplete()
	{
		if (StoryProgressController.Instance.CurrentLevel.levelNumber == 10)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	void GameComplete()
	{
		gameCompletePanel.SetActive(true);
		nextLevelButton.SetActive(false);
		stopTestingButton.SetActive(false);

		if (Application.isWebPlayer)
		{
			demoLabel.SetActive(true);
			gameCompleteLabel.SetActive(false);
			websiteButton.url = "http://www.smirkstudio.co.uk/cubaze.html";
		}
		else
		{
			demoLabel.SetActive(false);
			gameCompleteLabel.SetActive(true);
			websiteButton.url = "http://www.smirkstudio.co.uk/";
		}
	}
}
