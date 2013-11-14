using UnityEngine;
using System.Collections;

public class CopyShaderColour : MonoBehaviour 
{
	public Transform target;

	public bool includeAlpha;

	private Color cachedTargetColour;
	private Material cachedMat;
	private Material cachedTargetMat;

	public string colourPropertyName = "";

	void Start()
	{
		cachedMat = renderer.material;
		cachedTargetMat = target.renderer.material;
	}

	void Update () 
	{
		var targetColour = cachedTargetMat.color;

		if(targetColour != cachedTargetColour)
		{
			cachedTargetColour = targetColour;
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
}
