using UnityEngine;
using System.Collections;

public class DraggableRotatableMapObject : DraggableMapObject 
{
	DragAndDropController dragController;
	bool isRotating;

	void Start()
	{
		dragController = GameObject.Find("UIController").GetComponent<DragAndDropController>();
	}

	void Update()
	{
		if(!isRotating)
		{
			if(Input.GetKeyDown(KeyCode.LeftArrow))
			{
				if(dragController.draggingObj == gameObject)
				{
					StartCoroutine(RotateObject(false));
				}
			}
			if(Input.GetKeyDown(KeyCode.RightArrow))
			{
				if(dragController.draggingObj == gameObject)
				{
					StartCoroutine(RotateObject(true));
				}
			}
		}
	}

	IEnumerator RotateObject(bool rotateClockwise)
	{
		isRotating = true;
		int frameCounter = 0;

		if(rotateClockwise)
		{
			while(isRotating)
			{
				transform.Rotate(new Vector3(0, 10 * Time.deltaTime, 0));
				yield return new WaitForEndOfFrame();
				frameCounter++;
				if(frameCounter == 100)
				{
					isRotating = false;
				}
			}
		}
		else
		{
			while(isRotating)
			{
				transform.Rotate(new Vector3(0, -10 * Time.deltaTime, 0));
				yield return new WaitForEndOfFrame();
				frameCounter++;
				if(frameCounter == 100)
				{
					isRotating = false;
				}
			}
		}
	}
}
