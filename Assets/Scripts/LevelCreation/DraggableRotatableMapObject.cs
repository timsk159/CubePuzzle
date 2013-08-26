using UnityEngine;
using System.Collections;

public class DraggableRotatableMapObject : DraggableMapObject 
{
	DragAndDropController dragController;
	bool isRotating;

	void Awake()
	{
		if(GameObject.Find("UIController") != null)
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
					transform.Rotate(new Vector3(0, -90, 0));
				}
			}
			if(Input.GetKeyDown(KeyCode.RightArrow))
			{
				if(dragController.draggingObj == gameObject)
				{
					transform.Rotate(new Vector3(0, 90, 0));
				}
			}
		}
	}

	protected override void OnDestroy()
	{
		NotificationCenter<DragAndDropNotification>.DefaultCenter.PostNotification(DragAndDropNotification.MapObjectRemoved, this);
	}
}
