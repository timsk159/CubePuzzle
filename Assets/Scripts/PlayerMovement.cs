using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	public bool canMove = true;
	
	public float inputSensitivity;
	public float maxSpeed;

	float movementCD = 1.1f;
	float nextMoveTime;

	//Move 1 unit in desired direction over time, with a timer slightly greater than the movement time.

	void FixedUpdate ()
	{
		if(canMove)
		{
			var verticalInput = Input.GetAxis("Vertical");
			var horizontalInput = Input.GetAxis("Horizontal");
			
			if(verticalInput != 0 || horizontalInput != 0)
			{
				//if(Time.time > nextMoveTime)
				{
					var zForce = verticalInput * inputSensitivity;
					var xForce = horizontalInput * inputSensitivity;
				
					var force = new Vector3(xForce, 0, zForce);
					//force.Normalize();
					if(rigidbody.velocity.x < maxSpeed && rigidbody.velocity.y < maxSpeed && rigidbody.velocity.z < maxSpeed)
						rigidbody.AddForce(force, ForceMode.Acceleration);
					//transform.Translate(force * Time.deltaTime, Space.World);
					//canMove = false;
					//iTween.MoveAdd(gameObject, iTween.Hash("amount", force, "time", 0.5f, "easetype", iTween.EaseType.linear, "oncomplete", "MoveComplete"));
					//nextMoveTime = Time.time + movementCD;
				}
			}
		}
	}

	void MoveComplete()
	{
		canMove = true;
	}

	IEnumerator Move(Vector3 normalizedMoveVector)
	{
		yield return 0;
	}
}
