using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour 
{
	public GameObject rootPanel;

	public UISprite colourChangeSprite;

	void Awake()
	{
		Messenger<StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData>.AddListener(LevelCreatorStateMessage.TestingMapEnter.ToString(), TestingMapEnter);
		Messenger<StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData>.AddListener(LevelCreatorStateMessage.TestingMapExit.ToString(), TestingMapExit);

		Messenger<Colour>.AddListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);
		Messenger.AddListener(LevelStateMessage.LevelStarted.ToString(), LevelStarted);
	}

	void OnDestroy()
	{
		Messenger<StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData>.RemoveListener(LevelCreatorStateMessage.TestingMapEnter.ToString(), TestingMapEnter);
		Messenger<StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData>.RemoveListener(LevelCreatorStateMessage.TestingMapExit.ToString(), TestingMapExit);

		Messenger<Colour>.RemoveListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);
		Messenger.RemoveListener(LevelStateMessage.LevelStarted.ToString(), LevelStarted);
	}

	void LevelStarted()
	{
		EnableHud();
	}

	void TestingMapEnter(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
	{
		EnableHud();
	}

	void TestingMapExit(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
	{
		DisableHud();
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
