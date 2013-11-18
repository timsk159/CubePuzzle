using UnityEngine;
using System.Collections;

public class CopyPlayerColour : MonoBehaviour 
{
	public bool includeAlpha;

	private Material cachedMat;

	public string colourPropertyName = "";

	void Start()
	{
		cachedMat = renderer.material;

		NotificationCenter<ColourCollisionNotification>.DefaultCenter.AddObserver(this, ColourCollisionNotification.PlayerChangedColour);
	}

	void PlayerChangedColour(NotificationCenter<ColourCollisionNotification>.Notification notiData)
	{
		var playersNewColour = (Colour)notiData.data;

		var targetColour = ColorCollisionObject.GetObjectRealColor(playersNewColour);

		if(includeAlpha)
		{
			if(!string.IsNullOrEmpty(colourPropertyName))
			{
				cachedMat.SetColor(colourPropertyName, targetColour);
			}
			else
			{
				cachedMat.color = targetColour;
			}
		}
		else
		{
			var newColour = targetColour;
			if(!string.IsNullOrEmpty(colourPropertyName))
			{
				newColour.a = cachedMat.GetColor(colourPropertyName).a;

				cachedMat.SetColor(colourPropertyName, newColour);
			}
			else
			{
				newColour.a = cachedMat.color.a;
				cachedMat.color = newColour;
			}
		}
	}
}
