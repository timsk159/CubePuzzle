using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour 
{		
	GameObject mapRoot;
	public float movementSensitivity;
	public float rotationSensitivity;
	public float minDistance;

	Vector3 dragOrigin;

	private bool isRotating;	
	private bool isZooming;

	void Start()
	{
		mapRoot = GameObject.Find("MapRoot");
	}

	void LateUpdate()
	{
		if (LevelSerializer.IsDeserializing)
			return;

		if(Input.GetKey(KeyCode.Backspace))
		{
			ResetPosition();
			return;
		}
		var tooClose = IsTooClose();

		var xInput = Input.GetAxis("Horizontal");
		var yInput = Input.GetAxis("Vertical");
		var scrollInput = Input.GetAxis("Mouse ScrollWheel");
		var boost = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		//If we are too close to the map, only allow movement away from the map.
		if(tooClose)
		{
			/*
			if(Mathf.Sign(scrollInput) > 0)
				scrollInput = 0;
			*/

			var toMap = mapRoot.transform.position - transform.position;
			var toMapDirection = toMap.normalized;
		//	print("To Map: " + toMap + " direction: " + toMapDirection);
			var requestMoveVector = new Vector3(xInput, yInput, scrollInput);
			var moveDirection = requestMoveVector.normalized;
			//print("Move: " + requestMoveVector + " direction: " + requestMoveVector.normalized);

			//If signs of movements vector elements differ from direction to map, we are moving away from the map.
			//if(moveDir.x.sign != toMap.x.sign)
				//allow x movement.
			if(Mathf.Sign(moveDirection.x) == Mathf.Sign(toMapDirection.x))
			{
				xInput = 0;
			}
			if(Mathf.Sign(moveDirection.y) == Mathf.Sign(toMapDirection.y))
			{
				yInput = 0;
			}
			if(Mathf.Sign(moveDirection.z) == Mathf.Sign(toMapDirection.z))
			{
				scrollInput = 0;
			}
		}
		
		if(Input.GetMouseButtonDown(2))
		{
			dragOrigin = Input.mousePosition;
			isRotating = true;
		}
		if(xInput != 0 || yInput != 0)
		{
			dragOrigin = Input.mousePosition;
		}
		
		if(Input.GetMouseButtonUp(2)) 
			isRotating = false;
		
		//WASD movement
		if(yInput != 0 || xInput != 0)
		{
			if(boost)
			{
				yInput *= 2.0f;
				xInput *= 2.0f;
			}
			Vector3 move = new Vector3((xInput * movementSensitivity) * Time.deltaTime, (yInput * movementSensitivity) * Time.deltaTime, 0);
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

	bool IsTooClose()
	{
		if(mapRoot == null)
			mapRoot = GameObject.Find("MapRoot");

		var distance = Vector3.Distance(transform.position, mapRoot.transform.position);

		if(distance > minDistance)
			return false;
		else
			return true;
	}

	float GetDotToMap()
	{
		var toMap = mapRoot.transform.position - transform.position;
		return Vector3.Dot(transform.forward, toMap);
	}

	public void ResetPosition()
	{
		transform.position = new Vector3(0, 10, -10);
		transform.eulerAngles = new Vector3(45, 0, 0);
	}
}