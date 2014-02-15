using UnityEngine;
using System;	
using System.Collections;

public class DragAndDropDoorMenuItem : DragAndDropMenuItem
{
	GameObject cycleColourButton;
	GameObject cycleDoorColourButton;
	
	DoorPiece doorPiece;
	
	void Start()
	{
		cycleColourButton = transform.parent.Find("CycleColourButton").gameObject;
		UIEventListener.Get(cycleColourButton).onClick = HandleColourCycleButtonClick;
		
		cycleDoorColourButton = transform.parent.Find("CycleDoorColourButton").gameObject;
		UIEventListener.Get(cycleDoorColourButton).onClick = HandleDoorColourCycleButtonClick;
		
		doorPiece = transform.Find("DoorCubeMenuObj").GetComponent<DoorPiece>();
	}

	void HandleDoorColourCycleButtonClick (GameObject go)
	{
		doorPiece.RotateDoorColour(false);
	}

	void HandleColourCycleButtonClick (GameObject go)
	{
		doorPiece.RotateColour(false);
	}
	
	protected override void OnPress(bool isPressed)
	{
		if(isPressed)
		{			
			base.OnPress (isPressed);
		}
	}
}
