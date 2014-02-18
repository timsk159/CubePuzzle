using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour 
{
	HUD hud;
	public TutorialExtraElements extraElements;
	public UILabel tutorialLabel;
	public GameObject tutorialPanel;

	float textFadeTime = 0.5f;
	float hudZoomTime = 0.5f;
	bool isShowingTutorial;
	bool hudIsZoomed;

	GameObject additionalObject;

	void Start()
	{
		//Tutorial uses the HUD a lot, we grab an instance of it for ease of use
		hud = GetComponent<HUD>();
		Messenger.AddListener(LevelStateMessage.LevelStarted.ToString(), LevelStarted);
	}

	void OnDestroy()
	{
		Messenger.RemoveListener(LevelStateMessage.LevelStarted.ToString(), LevelStarted);
	}

	void LevelStarted()
	{
		if(!LevelController.Instance.isStoryMode)
			return;

		var currentLevelNumber = StoryProgressController.Instance.CurrentLevel.levelNumber;
		Debug.Log("+++--- loading tutorial for level: " + currentLevelNumber);
		//All levels after the 10th have no tutorial.
		if(currentLevelNumber > 10)
			return;

		//Launch tutorials for different levels.
		switch(currentLevelNumber)
		{
			case 1:
				tutorialPanel.SetActive(true);
				DisablePlayer();
				DisplayText("Use the WASD or arrows keys to move. Try and find the entrance (You can't miss it!)\n\nTap any key to close this window.", false);
				isShowingTutorial = true;
				break;
			case 2:
				tutorialPanel.SetActive(true);
				DisablePlayer();
				DisplayText("You can't move over a cube that is the same colour as your ball.\n\nThe image here will highlight which colour cubes are currently blocked\n" +
					"Rolling over the button with the floating cube above it will rotate each cubes colour by one" +
					"\n\nTap any key to close this", false);
				ZoomHUDUp();
				DisplayAdditonalElement(false, extraElements.GetElement("ButtonCube"));
				isShowingTutorial = true;
				break;
		}
	}

	void Update()
	{
		if(!isShowingTutorial)
			return;

		if(LevelController.Instance.playerChar.playerMovement.canMove)
			DisablePlayer();

		if(Input.anyKeyDown)
		{
			DismissTutorial();
		}
	}

	void DisablePlayer()
	{
		LevelController.Instance.playerChar.playerMovement.canMove = false;
		LevelController.Instance.playerChar.canReset = false;
		LevelController.Instance.canPause = false;
	}

	void EnablePlayer()
	{
		LevelController.Instance.playerChar.playerMovement.canMove = true;
		LevelController.Instance.playerChar.canReset = true;
		LevelController.Instance.canPause = true;
	}

	void DisplayText(string text, bool fadeAfterZoom)
	{
		//Set the text and fade the label up, with an optional timer
		tutorialLabel.text = text;
		if(fadeAfterZoom)
			Invoke("FadeTutorialPanelUp", hudZoomTime);
		else
			FadeTutorialPanelUp();
	}

	void DisplayAdditonalElement(bool afterZoom, GameObject gameObjectToUse)
	{
		//Need to keep track of the object we turned on, to turn it off later.
		additionalObject = gameObjectToUse;

		//Simply turn the given object on, before or after the hud zoom
		if(afterZoom)
			Invoke("EnableGO", hudZoomTime);
		else
			EnableGO();
	}

	public void DismissTutorial()
	{
		isShowingTutorial = false;
		DisableGO();
		FadeTutorialPanelDown();
		ZoomHUDDown();
		Invoke("EnablePlayer", hudZoomTime);
		Invoke("DisablePanel", hudZoomTime);
	}

	//Has to be a coroutine, to support the delay (Invoke can't have parameters)
	void EnableGO()
	{
		additionalObject.SetActive(true);
	}

	void DisableGO()
	{
		if(additionalObject != null)
		{
			additionalObject.SetActive(false);
			additionalObject = null;
		}
	}

	void EnablePanel()
	{
		tutorialPanel.SetActive(true);
	}

	void DisablePanel()
	{
		tutorialPanel.SetActive(false);
	}

	void ZoomHUDUp()
	{
		PlayHuDUpAnimation();
		hudIsZoomed = true;
	}

	void ZoomHUDDown()
	{
		if(hudIsZoomed)
		{
			PlayHuDDownAnimation();
		}
	}

	void PlayHuDUpAnimation()
	{
		if(!hudIsZoomed)
		{
			hud.colourChangeSprite.animation.Play("ZoomHuDUp");
		}
	}

	void PlayHuDDownAnimation()
	{
		if(hudIsZoomed)
		{
			hud.colourChangeSprite.animation.Play("ZoomHuDDown");
		}
	}
	void FadeTutorialPanelUp()
	{
		tutorialPanel.GetComponent<UIPanel>().alpha = 0;
		TweenAlpha.Begin(tutorialPanel, textFadeTime, 1);
	}

	void FadeTutorialPanelDown()
	{
		tutorialPanel.GetComponent<UIPanel>().alpha = 1;
		TweenAlpha.Begin(tutorialPanel, textFadeTime, 0);
	}
}
