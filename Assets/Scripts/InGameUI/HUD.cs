using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour 
{
	GameObject rootPanel;

	UISprite colourChangeSprite;



	void Start()
	{
		Messenger<Colour>.AddListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);

		rootPanel = GameObject.Find("HudPanel");

		colourChangeSprite = rootPanel.transform.Find("HudBottomRightAnchor/ColourCycleRoot/Texture").GetComponent<UISprite>();
	}

	void OnDestroy()
	{
		Messenger<Colour>.RemoveListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);
	}

	void PlayerChangedColour(Colour colourToChangeTo)
	{
		switch(colourToChangeTo)
		{
			case Colour.Blue:
				colourChangeSprite.spriteName = "BallColour-Red";

				break;
			case Colour.Green:
				colourChangeSprite.spriteName = "BallColour-Blue";
				break;
			case Colour.Red:
				colourChangeSprite.spriteName = "BallColour-Green";
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
