using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour 
{
	GameObject rootPanel;

	GameObject redSphere;
	GameObject greenSphere;
	GameObject blueSphere;

	Vector3 sphereSelectScale;
	Vector3 sphereNormalScale;


	void Start()
	{
		Messenger<Colour>.AddListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);

		rootPanel = GameObject.Find("HudPanel");

		redSphere = GameObject.Find("PlayerRed");
		greenSphere = GameObject.Find("PlayerGreen");
		blueSphere = GameObject.Find("PlayerBlue");

		sphereSelectScale = new Vector3(23, 23, 23);
		sphereNormalScale = new Vector3(15, 15, 15);
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
				redSphere.transform.localScale = sphereSelectScale;

				greenSphere.transform.localScale = sphereNormalScale;
				blueSphere.transform.localScale = sphereNormalScale;

				break;
			case Colour.Green:
				blueSphere.transform.localScale = sphereSelectScale;

				greenSphere.transform.localScale = sphereNormalScale;
				redSphere.transform.localScale = sphereNormalScale;
				break;
			case Colour.Red:
				greenSphere.transform.localScale = sphereSelectScale;

				redSphere.transform.localScale = sphereNormalScale;
				blueSphere.transform.localScale = sphereNormalScale;
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
