using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour 
{
	UITexture colourCycleImage;
	UITexture colourCycleHighlight;

	void Start()
	{
		Messenger<Colour>.AddListener(ColourCollisionNotification.PlayerChangedColour.ToString(), PlayerChangedColour);
	}

	void OnDestroy()
	{
		Messenger<Colour>.RemoveListener(ColourCollisionNotification.PlayerChangedColour.ToString(), PlayerChangedColour);
	}

	void PlayerChangedColour(Colour colourToChangeTo)
	{
		switch(colourToChangeTo)
		{
			case Colour.Blue:
				//colourCycleHighlight.transform.position = ;
				break;
			case Colour.Green:

				break;
			case Colour.Red:

				break;
			default:
				Debug.LogError("Unknown colour");
				break;
		}
	}
}
