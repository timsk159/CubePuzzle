using UnityEngine;
using System.Collections;

public class FrontMenuPlayerAnimation : MonoBehaviour 
{
	GameObject player;
	public float minTimeBetween;
	public float maxTimeBetween;
	float timer;

	Vector3 force;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");

	}

	void FixedUpdate()
	{
		//Apply force for a random amount of time.
		//random timer between movements
		//Constrain to mesh.

		/*
		var randomDirection = new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

		randomDirection.Normalize();

		var randomForce = randomDirection * Random.Range(0.0f, 50.0f);
		*/

		if(Time.time > timer)
		{
			timer = Time.time + Random.Range(minTimeBetween, maxTimeBetween);
			force = new Vector3(Random.Range(-50.0f, 50.0f), 0, Random.Range(-50.0f, 50.0f));
		}
		RaycastHit hit;

		if(Physics.Raycast(player.transform.position, force.normalized, out hit, 5))
		{
			var oldForce = force;

			force = hit.normal * oldForce.magnitude;
		}
		Debug.DrawRay(player.transform.position, force.normalized);

		player.rigidbody.AddForce(force);
	}
}
