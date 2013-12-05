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

		Messenger<Colour>.AddListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);
	}

	void OnDestroy()
	{
		Messenger<Colour>.RemoveListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);
	}

	void PlayerChangedColour(Colour playersNewColour)
	{
		var targetColour = ColorManager.GetObjectRealColor(playersNewColour);

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
