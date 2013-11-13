using UnityEngine;
using System.Collections;

public class CopyShaderColour : MonoBehaviour 
{
	public Transform target;

	public bool includeAlpha;

	private Color cachedTargetColour;

	void Update () 
	{
		var targetColour = target.renderer.material.color;

		if(targetColour != cachedTargetColour)
		{
			cachedTargetColour = targetColour;
			if(includeAlpha)
			{
				renderer.material.color = targetColour;
			}
			else
			{
				var newColour = targetColour;
				newColour.a = renderer.material.color.a;
				renderer.material.color = newColour;
			}
		}
	}
}
