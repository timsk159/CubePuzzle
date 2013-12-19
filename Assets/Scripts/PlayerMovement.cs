using UnityEngine;
using System;
using System.Collections;

public enum MovementType
{
	Physics, Block, Smooth
}

public class PlayerMovement : MonoBehaviour 
{
	MovementType[] allMoveTypes;

	MovementType moveType;

	public bool canMove = true;
	public bool isMoving;

	private bool  _isMovingFast;

	public bool isMovingFast
	{
		get
		{
			var vMag = rigidbody.velocity.magnitude;

			if(vMag > fastMovement)
				return true;

			return false;
		}
	}
	
	public float inputSensitivity;
	public float maxSpeed;

	public float fastMovement;
	float movementCD = 0.5f;
	float nextMoveTime;

	void Start()
	{
		allMoveTypes = (MovementType[])Enum.GetValues(typeof(MovementType));
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.F1))
		{
			CycleMoveType();
			switch(moveType)
			{
				case MovementType.Physics:
					rigidbody.freezeRotation = false;
					rigidbody.useGravity = true;
					rigidbody.isKinematic = false;
					break;
				case MovementType.Block:
					var roundedPos = transform.position;
					roundedPos.x = Mathf.RoundToInt(roundedPos.x);
					roundedPos.z = Mathf.RoundToInt(roundedPos.z);
					transform.rotation = Quaternion.identity;
					transform.position = roundedPos;

					rigidbody.velocity = Vector3.zero;
					rigidbody.useGravity = false;
					rigidbody.freezeRotation = true;
					break;
				case MovementType.Smooth:
					rigidbody.freezeRotation = true;
					rigidbody.useGravity = false;
					rigidbody.isKinematic = true;
					break;
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
				isMoving = true;
				var zForce = verticalInput * inputSensitivity;
				var xForce = horizontalInput * inputSensitivity;

				var force = new Vector3(xForce, 0, zForce);
				switch(moveType)
				{
					case MovementType.Physics:
						DoPhysicsMovement(force);
						break;
					case MovementType.Block:
						DoBlockMovement(force);
						break;
					case MovementType.Smooth:
						DoSmoothMovement(force);
						break;
				}
			}
			else
			{
				isMoving = false;
			}
		}
	}

	void CycleMoveType()
	{
		var currentIndex = (int)moveType;

		currentIndex++;

		if(currentIndex >= allMoveTypes.Length)
			currentIndex = 0;

		moveType = (MovementType)currentIndex;
	}

	void DoPhysicsMovement(Vector3 input)
	{
		//if(rigidbody.velocity.magnitude < maxSpeed)
			rigidbody.AddForce(input, ForceMode.Acceleration);
	}

	void DoBlockMovement(Vector3 input)
	{
		if(Time.time > nextMoveTime)
		{
			//Normalize doesn't work for negative-length vectors. Have to normalize it manually.
			//force.Normalize();

			input.x = Mathf.Clamp(input.x, -1, 1);
			input.z = Mathf.Clamp(input.z, -1, 1);

			input.x = Mathf.CeilToInt(input.x);
			input.z = Mathf.CeilToInt(input.z);

			var movementDistance = Mathf.Abs(input.x) + Mathf.Abs(input.z);
			if(!Physics.Raycast(new Ray(transform.position, input), movementDistance))
			{
				canMove = false;
				iTween.MoveAdd(gameObject, iTween.Hash("amount", input, "time", 0.5f, "easetype", iTween.EaseType.linear, "oncomplete", "MoveComplete"));
				nextMoveTime = Time.time + movementCD;
			}
		}
	}

	void DoSmoothMovement(Vector3 input)
	{
		//Used for animation control:
		var direction = input.normalized;
		var speed = input.magnitude;

		transform.Translate(input * Time.deltaTime);
	}

	void MoveComplete()
	{
		canMove = true;
	}
}
