using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraControls : MonoBehaviour 
{		
	GameObject mapRoot;
	public float movementSensitivity;
	public float rotationSensitivity;
	public float minDistance;

	Vector3 dragOrigin;

	private bool isRotating;	
	private bool isZooming;
	bool isMoving;

	void Start()
	{
		mapRoot = GameObject.Find("MapRoot");
	}

	void FixedUpdate()
	{
		if (LevelSerializer.IsDeserializing)
			return;

		if(Input.GetKey(KeyCode.Backspace))
		{
			ResetPosition();
			return;
		}

		var xInput = Input.GetAxis("Horizontal");
		var zInput = Input.GetAxis("Vertical");
		var scrollInput = Input.GetAxis("Mouse ScrollWheel");
		var boost = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

		if(xInput != 0 || zInput != 0 || scrollInput != 0)
			isMoving = true;
		else
			isMoving = false;

		if(!isMoving)
			return;

		var closestCube = GetClosestCubeTransform();

		var tooClose = IsTooClose(closestCube);
		
		//If we are too close to the map, only allow movement away from the map.
		if(tooClose)
		{
			var toCube = closestCube.position - transform.position;
			var toCubeDirection = toCube.normalized;
			Debug.DrawRay(transform.position, toCube);

			var moveVector = new Vector3(xInput, 0, zInput);
			var moveDirection = moveVector.normalized;

			if(Mathf.Sign(xInput) == Mathf.Sign(toCubeDirection.x))
			{
				xInput = 0;
			}
			if(Mathf.Sign(zInput) == Mathf.Sign(toCubeDirection.z))
			{
				zInput = 0;
			}


			if(Mathf.Sign(toCubeDirection.y) == Mathf.Sign(scrollInput))
			{
				scrollInput = 0;
			}
		}
		
		if(Input.GetMouseButtonDown(2))
		{
			dragOrigin = Input.mousePosition;
			isRotating = true;
		}
		if(xInput != 0 || zInput != 0)
		{
			dragOrigin = Input.mousePosition;
		}
		
		if(Input.GetMouseButtonUp(2)) 
			isRotating = false;
		
		//WASD movement
		if(zInput != 0 || xInput != 0)
		{
			if(boost)
			{
				zInput *= 2.0f;
				xInput *= 2.0f;
			}
			Vector3 move = new Vector3((xInput * movementSensitivity) * Time.deltaTime, 0, (zInput * movementSensitivity) * Time.deltaTime);
			transform.Translate(move, Space.World);
		}
		
		// Right click rotation
		if(isRotating)
		{
			Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
		 
			transform.RotateAround(transform.position, transform.right, -pos.y * rotationSensitivity);
			transform.RotateAround(transform.position, Vector3.up, pos.x * rotationSensitivity);
		}
		// Scroll wheel zooming
		if(scrollInput != 0)
		{
			Vector3 move = scrollInput * movementSensitivity * transform.forward;
			transform.Translate(move, Space.World);
		}
	}

	bool IsTooClose(Transform cube)
	{
		if(mapRoot == null)
			mapRoot = GameObject.Find("MapRoot");

		var distance = Vector3.Distance(transform.position, cube.position);

		if(distance > minDistance)
			return false;
		else
			return true;
	}

	Transform GetClosestCubeTransform()
	{
		if(mapRoot == null)
			mapRoot = GameObject.Find("MapRoot");

		float closestDistance = 999;
		Transform returnCube = null;
		foreach(Transform cube in mapRoot.transform)
		{
			var dist = Vector3.Distance(transform.position, cube.position);
			if(dist < closestDistance)
			{
				closestDistance = dist;
				returnCube = cube;
			}
		}
		return returnCube;
	}

	float GetDotToCube(Transform cubeTrans)
	{
		var toCube = cubeTrans.position - transform.position;
		return Vector3.Angle(transform.forward, toCube.normalized);
	}

	public void ResetPosition()
	{
		transform.position = new Vector3(0, 10, -10);
		transform.eulerAngles = new Vector3(45, 0, 0);
	}
}