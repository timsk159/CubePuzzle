using UnityEngine;
using System.Collections;
using System;

public class CopyPlayerColour : MonoBehaviour 
{
	public bool includeAlpha;

	private Material cachedMat;

	public string colourPropertyName = "";

	void Start()
	{
		cachedMat = renderer.material;

		Messenger<Colour>.AddListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);
	}

	void OnDestroy()
	{
		Messenger<Colour>.RemoveListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);
	}

	void PlayerChangedColour(Colour playersNewColour)
	{
		var currentColourIndex = (int)playersNewColour;
		currentColourIndex++;
		if(currentColourIndex == Enum.GetValues(typeof(Colour)).Length)
		{
			currentColourIndex = 1;
		}
		var targetColour = ColorManager.GetObjectRealColor((Colour)currentColourIndex);

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
