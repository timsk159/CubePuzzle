using UnityEngine;
using System.Collections;

public class WebSecure
{
	public bool IsHostedCorrectly(string fullCorrectURL)
	{
		if(Application.isEditor)
		{
			return true;
		}

		if(Application.isWebPlayer)
		{
			if(Application.srcValue != "Cubaze.unity3d")
				return false;
			if(string.Compare(Application.absoluteURL, fullCorrectURL) != 0)
				return false;
		}

		return true;
	}
}
