using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour 
{
	public Vector3 scale;
	public float speed;
	public float time;

	public bool random;
	public float randomMin;
	public float randomMax;

	public iTween.LoopType loopType;
	public iTween.EaseType easeType;

	public bool playOnStart;

	Vector3 rotateAmount;

	void Start()
	{
		if(random)
		{
			RandomRotation();
		}
		else
		{
			rotateAmount = scale.normalized * speed;
			iTween.RotateAdd(gameObject, iTween.Hash("amount", rotateAmount, "time", time, "looptype", loopType, "easetype", easeType));
		}
	}

	void RandomRotation()
	{
		rotateAmount = scale.normalized;

		var randomRotation = new Vector3(rotateAmount.x * Random.Range(randomMin, randomMax), rotateAmount.y * Random.Range(randomMin, randomMax), rotateAmount.z * Random.Range(randomMin, randomMax));

		rotateAmount = randomRotation;

		iTween.RotateAdd(gameObject, iTween.Hash("amount", rotateAmount, "time", time, "easetype", easeType, "oncomplete", "RandomRotation"));
	}

	public void Play()
	{

	}

	public void Stop()
	{

	}
}
