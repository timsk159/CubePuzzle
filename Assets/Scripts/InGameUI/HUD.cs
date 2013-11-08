using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour 
{
	UITexture colourCycleImage;
	UITexture colourCycleHighlight;

	void Start()
	{
		NotificationCenter<ColourCollisionNotification>.DefaultCenter.AddObserver(this, ColourCollisionNotification.PlayerChangedColour);
	}

	void PlayerChangedColour(NotificationCenter<ColourCollisionNotification>.Notification notiData)
	{
		var colourToChangeTo = (Colour)notiData.data;

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
