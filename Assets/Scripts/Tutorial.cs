using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour 
{
	HUD hud;
	public GameObject tutorialElementsRoot;
	public UILabel tutorialLabel;

	float textFadeTime = 0.5f;
	float hudZoomTime = 0.5f;

	GameObject additionalObject;

	void Start()
	{
		//Tutorial uses the HUD a lot, we grab an instance of it for ease of use
		hud = GetComponent<HUD>();
		Messenger.AddListener(LevelStateMessage.LevelStarted.ToString(), LevelStarted);
	}

	void LevelStarted()
	{
		if(!LevelController.Instance.isStoryMode)
			return;

		var currentLevelNumber = StoryProgressController.Instance.CurrentLevel.levelNumber;

		//All levels after the 10th have no tutorial.
		if(currentLevelNumber > 10)
			return;

		//Launch tutorials for different levels.
		switch(currentLevelNumber)
		{
			case 1:

				break;
			case 2:

				break;
		}
	}

	void DisplayTutorial(string text, bool fadeAfterZoom, GameObject additionalObject)
	{
		DisplayText(text, fadeAfterZoom);
		if(additionalObject != null)
			DisplayAdditionalImage(fadeAfterZoom, additionalObject);
	}

	void DisplayText(string text, bool fadeAfterZoom)
	{
		//Set the text and fade the label up, with an optional timer
		tutorialLabel.text = text;
		if(fadeAfterZoom)
			Invoke("FadeTextUp", hudZoomTime);
		else
			FadeTextUp();
	}

	void DisplayAdditionalImage(bool afterZoom, GameObject gameObjectToUse)
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
		DisableGO();
		FadeTextDown();
		ZoomHUDDown();
	}

	//Has to be a coroutine, to support the delay (Invoke can't have parameters)
	void EnableGO()
	{
		additionalObject.SetActive(true);
	}

	void DisableGO()
	{
		additionalObject.SetActive(false);
		additionalObject = null;
	}

	void ZoomHUDUp()
	{
		hud.colourChangeSprite.animation.Play("ZoomHuDUp");
	}

	void ZoomHUDDown()
	{
		hud.colourChangeSprite.animation.Play("ZoomHuDDown");
	}

	void FadeTextUp()
	{
		TweenAlpha.Begin(tutorialElementsRoot, textFadeTime, 1);
	}

	void FadeTextDown()
	{
		TweenAlpha.Begin(tutorialElementsRoot, textFadeTime, 0);
	}
}
