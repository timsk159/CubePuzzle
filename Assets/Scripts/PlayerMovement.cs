using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	public bool canMove = true;
	
	public float inputSensitivity;
	public float maxSpeed;
	
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
				
				rigidbody.AddForce(Vector3.ClampMagnitude(force, maxSpeed), ForceMode.Acceleration);

			}
		}
	}
}
