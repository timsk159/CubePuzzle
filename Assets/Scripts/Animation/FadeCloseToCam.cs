using UnityEngine;
using System.Collections;

public class FadeCloseToCam : MonoBehaviour 
{
	Transform mainCam;
	Transform thisTransform;
	Material thisMat;
	Collider thisCol;

	public float maxDistance = 7.5f;
	public float minDistance = 0.0f;

	void Start()
	{
		mainCam = Camera.main.transform;
		thisTransform = transform;
		thisMat = renderer.sharedMaterial;
		thisCol = collider;
	}

	void Update()
	{
		var closestPoint = thisCol.ClosestPointOnBounds(mainCam.position);
		var dist = Vector3.Distance(mainCam.position, closestPoint);

		if(dist < maxDistance)
		{
			Fade(dist);
		}
	}

	void Fade(float distance)
	{
		if(distance <= minDistance)
			SetAlpha (0);
		else
			SetAlpha(distance / maxDistance);
	}

	void SetAlpha(float alpha)
	{
		Color newColour = thisMat.color;
		newColour.a = alpha;
		thisMat.color = newColour;
		thisMat.SetColor("_Emission", newColour);
	}
}
