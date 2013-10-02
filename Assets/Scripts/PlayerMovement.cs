using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	private bool physicsMovement = true;

	public bool canMove = true;
	
	public float inputSensitivity;
	public float maxSpeed;

	float movementCD = 0.5f;
	float nextMoveTime;

	//Move 1 unit in desired direction over time, with a timer slightly greater than the movement time.

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.F1))
		{
			physicsMovement = !physicsMovement;
			if(!physicsMovement)
			{
				var roundedPos = transform.position;
				roundedPos.x = Mathf.RoundToInt(roundedPos.x);
				roundedPos.z = Mathf.RoundToInt(roundedPos.z);
				transform.rotation = Quaternion.identity;
				transform.position = roundedPos;

				rigidbody.useGravity = false;
				rigidbody.freezeRotation = true;
			}
			else
			{
				rigidbody.freezeRotation = true;
				rigidbody.useGravity = false;
			}
			canMove = true;
		}
	}

	void FixedUpdate ()
	{
		if(canMove)
		{
			var verticalInput = Input.GetAxis("Vertical");
			var horizontalInput = Input.GetAxis("Horizontal");

			if(verticalInput != 0 || horizontalInput != 0)
			{
				var zForce = verticalInput * inputSensitivity;
				var xForce = horizontalInput * inputSensitivity;

				var force = new Vector3(xForce, 0, zForce);
				if(physicsMovement)
				{
					if(rigidbody.velocity.x < maxSpeed && rigidbody.velocity.y < maxSpeed && rigidbody.velocity.z < maxSpeed)
						rigidbody.AddForce(force, ForceMode.Acceleration);
				}
				else
				{
					if(Time.time > nextMoveTime)
					{
						//Normalize doesn't work for negative-length vectors (moving backwards). Have to normalize it manually.
					//	force.Normalize();

						force.x = Mathf.Clamp(force.x, -1, 1);
						force.z = Mathf.Clamp(force.z, -1, 1);

						force.x = Mathf.CeilToInt(force.x);
						force.z = Mathf.CeilToInt(force.z);

						var movementDistance = Mathf.Abs(force.x) + Mathf.Abs(force.z);
						if(!Physics.Raycast(new Ray(transform.position, force), movementDistance))
						{
							canMove = false;
							iTween.MoveAdd(gameObject, iTween.Hash("amount", force, "time", 0.5f, "easetype", iTween.EaseType.linear, "oncomplete", "MoveComplete"));
							nextMoveTime = Time.time + movementCD;
						}
					}
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
