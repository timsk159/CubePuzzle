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
				DisplayText("Use the WASD or arrows keys to move. Try and find the entrance (You can't miss it!)\n\nTap any key to close this window.");
				StartCoroutine(SetIsShowing(hudZoomTime * 2.0f));
				break;
			case 2:
				tutorialPanel.SetActive(true);
				DisablePlayer();
				DisplayText("You can't move over a cube that is the same colour as your ball.\n\nThe image here will highlight which colour cubes are currently blocked\n" +
					"Rolling over the button with the floating cube above it will rotate each cubes colour by one" +
					"\n\nTap any key to close this");
				ZoomHUDUp();
				DisplayAdditonalElement(extraElements.GetElement("ButtonCubeWidget"));
				StartCoroutine(SetIsShowing(hudZoomTime * 2.0f));
				break;
			case 6:
				tutorialPanel.SetActive(true);
				DisablePlayer();
				DisplayText("Buttons with a sphere in them will change the colour of your ball. The sphere above the button shows you what colour you will change to.\n" +
					"The highlighted cube on the image shows you which cubes are currently raised");
				ZoomHUDUp();
				DisplayAdditonalElement(extraElements.GetElement("PlayerButtonWidget"));
				StartCoroutine(SetIsShowing(hudZoomTime * 2.0f));
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

	//Simple delay for setting our isShowing flag (which allows dismissal)
	IEnumerator SetIsShowing(float delay)
	{
		yield return new WaitForSeconds(delay);
		isShowingTutorial = true;
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

	void DisplayText(string text)
	{
		//Set the text and fade the label up, with an optional timer
		tutorialLabel.text = text;
		FadeTutorialPanelUp();
		AddFadeMainCamera();
	}

	void DisplayAdditonalElement(GameObject gameObjectToUse)
	{
		//Need to keep track of the object we turned on, to turn it off later.
		additionalObject = gameObjectToUse;

		//Simply turn the given object on
		EnableGO();
	}

	public void DismissTutorial()
	{
		isShowingTutorial = false;
		DisableGO();
		FadeTutorialPanelDown();
		ZoomHUDDown();
		RemoveFadeMainCamera();
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

	void AddFadeMainCamera()
	{
		var fadeGO = iTween.CameraFadeAdd();
		iTween.CameraFadeTo(0.39f, 0.5f);
	}

	void RemoveFadeMainCamera()
	{
		iTween.CameraFadeTo(iTween.Hash("amount", 0, "time", 0.5f, "oncomplete", "CamFadeComplete", "oncompletetarget", gameObject));
	}

	void CamFadeComplete()
	{
		iTween.CameraFadeDestroy();
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
