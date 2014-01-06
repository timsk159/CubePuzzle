using UnityEngine;
using System.Collections;

public class FrontMenuPlayerAnimation : MonoBehaviour 
{
	GameObject player;
	public float minTimeBetween;
	public float maxTimeBetween;
	float timer;

	float currentCountdown;

	Vector3 previousForce;
	Vector3 force;

	float forceT = 0;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		previousForce = Vector3.zero;
		force = Vector3.zero;
	}

	void FixedUpdate()
	{
		//Apply force for a random amount of time.
		//random timer between movements
		//Wall avoidance.

		if(Time.time > timer)
		{
			currentCountdown = Random.Range(minTimeBetween, maxTimeBetween);
			timer = Time.time + currentCountdown;
			force = new Vector3(Random.Range(-50.0f, 50.0f), 0, Random.Range(-50.0f, 50.0f));
			forceT = 0;
		}
		RaycastHit hit;

		if(Physics.Raycast(player.transform.position, force.normalized, out hit, 5))
		{
			//var oldForce = force;
			//force = hit.normal * oldForce.magnitude;

			previousForce = force;
			force = hit.normal * previousForce.magnitude;

		}
		//Added: Lerp to new force over time (smooths the direction changes).
		if(previousForce != Vector3.zero)
		{
			forceT += Time.deltaTime / (currentCountdown / 3);
			var smoothedForce = Vector3.Lerp(previousForce, force, forceT);
			player.rigidbody.AddForce(smoothedForce);

		}
		else
		{
			player.rigidbody.AddForce(force);
		}
	}
}
