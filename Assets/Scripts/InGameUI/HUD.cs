using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour 
{
	GameObject rootPanel;
	GameObject colourCycleHighlight;

	Vector3 redHighlightPos;
	Vector3 greenHighlightPos;
	Vector3 blueHighlightPos;


	void Start()
	{
		Messenger<Colour>.AddListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);

		rootPanel = GameObject.Find("HudPanel");
		colourCycleHighlight = rootPanel.transform.Find("ColourCycleRoot/ColourHighlight").gameObject;

		redHighlightPos = new Vector3(-0.33333f, 0, 0);
		greenHighlightPos = Vector3.zero;
		blueHighlightPos = new Vector3(0.33333f, 0, 0);
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
				colourCycleHighlight.transform.localPosition = blueHighlightPos;
				break;
			case Colour.Green:
				colourCycleHighlight.transform.localPosition = greenHighlightPos;
				break;
			case Colour.Red:
				colourCycleHighlight.transform.localPosition = redHighlightPos;
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
