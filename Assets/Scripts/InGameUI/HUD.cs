using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour 
{
	public GameObject rootPanel;

	public UISprite colourChangeSprite;

	void Awake()
	{
		Messenger<Colour>.AddListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);
		Messenger.AddListener(LevelStateMessage.LevelStarted.ToString(), LevelStarted);
	}

	void OnDestroy()
	{
		Messenger<Colour>.RemoveListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);
	}

	void LevelStarted()
	{
		EnableHud();
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
