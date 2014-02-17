using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour 
{
	public GameObject rootPanel;
	public GameObject introPanel;

	public UISprite colourChangeSprite;
	public UILabel introLabel;

	void Awake()
	{
		Messenger<StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData>.AddListener(LevelCreatorStateMessage.TestingMapEnter.ToString(), TestingMapEnter);
		Messenger<StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData>.AddListener(LevelCreatorStateMessage.TestingMapExit.ToString(), TestingMapExit);

		Messenger.AddListener(LevelIntroMessage.IntroStarted.ToString(), IntroStarted);
		Messenger<Colour>.AddListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);
		Messenger.AddListener(LevelStateMessage.LevelStarted.ToString(), LevelStarted);
	}

	void OnDestroy()
	{
		Messenger<StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData>.RemoveListener(LevelCreatorStateMessage.TestingMapEnter.ToString(), TestingMapEnter);
		Messenger<StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData>.RemoveListener(LevelCreatorStateMessage.TestingMapExit.ToString(), TestingMapExit);

		Messenger.RemoveListener(LevelIntroMessage.IntroStarted.ToString(), IntroStarted);
		Messenger<Colour>.RemoveListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);
		Messenger.RemoveListener(LevelStateMessage.LevelStarted.ToString(), LevelStarted);
	}

	void LevelStarted()
	{
		EnableHud();
		introPanel.SetActive(false);
	}

	void TestingMapEnter(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
	{
		EnableHud();
	}

	void TestingMapExit(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
	{
		DisableHud();
	}

	void IntroStarted()
	{
		if(LevelController.Instance.isStoryMode)
		{
			introPanel.SetActive(true);
			introLabel.gameObject.SetActive(true);
			introLabel.text = StoryProgressController.Instance.CurrentLevel.displayName;
			introLabel.SetDirty();
			introLabel.color = new Color(introLabel.color.r, introLabel.color.g, introLabel.color.b, 0);
			var tween = TweenColor.Begin(introLabel.gameObject, 1.9f, new Color(introLabel.color.r, introLabel.color.g, introLabel.color.b, 1));
			EventDelegate.Add(tween.onFinished, TweenBack);
		}
	}

	void TweenBack()
	{
		EventDelegate.Remove(TweenColor.current.onFinished, TweenBack);
		TweenColor.Begin(introLabel.gameObject, 1.9f, new Color(introLabel.color.r, introLabel.color.g, introLabel.color.b, 0));
	}

	void PlayerChangedColour(Colour colourToChangeTo)
	{
		switch(colourToChangeTo)
		{
			case Colour.Blue:
				colourChangeSprite.spriteName = "CubeColour-Blue";
				break;
			case Colour.Green:
				colourChangeSprite.spriteName = "CubeColour-Green";
				break;
			case Colour.Red:
				colourChangeSprite.spriteName = "CubeColour-Red";
				break;
			default:
				Debug.LogError("Unknown colour");
				break;
		}
	}



	void EnableHud()
	{
		rootPanel.SetActive(true);
	}

	void DisableHud()
	{
		rootPanel.SetActive(false);
	}
}
