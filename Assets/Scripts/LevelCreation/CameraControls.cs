using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour 
{		
	public float movementSensitivity;
	public float rotationSensitivity;
	
	Vector3 dragOrigin;

	private bool isRotating;	
	private bool isZooming;
	
	void LateUpdate()
	{
		var xInput = Input.GetAxis("Horizontal");
		var yInput = Input.GetAxis("Vertical");
		var scrollInput = Input.GetAxis("Mouse ScrollWheel");
		var boost = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		
		
		if(Input.GetMouseButtonDown(2))
		{
			dragOrigin = Input.mousePosition;
			isRotating = true;
		}
		if(xInput != 0 || yInput != 0)
		{
			dragOrigin = Input.mousePosition;
		}
		
		if (Input.GetMouseButtonUp(2)) 
			isRotating=false;
		
		//WASD movement
		if(yInput != 0 || xInput != 0)
		{
			if(boost)
			{
				yInput *= 2.0f;
				xInput *= 2.0f;
			}
			Vector3 move = new Vector3((xInput * movementSensitivity) * Time.deltaTime, (yInput * movementSensitivity) * Time.deltaTime, 0);
			transform.Translate(move, Space.Self);
		}
		
		// Right click rotation
		if (isRotating)
		{
			Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
		 
			transform.RotateAround(transform.position, transform.right, -pos.y * rotationSensitivity);
			transform.RotateAround(transform.position, Vector3.up, pos.x * rotationSensitivity);
		}
		// Scroll wheel zooming
		if (scrollInput != 0)
		{
			Vector3 move = scrollInput * movementSensitivity * transform.forward;
			transform.Translate(move, Space.World);
		}
	}
}