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
		cycleColourButton = transform.Find("CycleColourButton").gameObject;
		UIEventListener.Get(cycleColourButton).onClick = HandleColourCycleButtonClick;
		
		cycleDoorColourButton = transform.Find("CycleDoorColourButton").gameObject;
		UIEventListener.Get(cycleDoorColourButton).onClick = HandleDoorColourCycleButtonClick;
		
		doorPiece = transform.Find("DoorCubeMenuObj").GetComponent<DoorPiece>();
	}

	void HandleDoorColourCycleButtonClick (GameObject go)
	{
		doorPiece.RotateDoorColour();
	}

	void HandleColourCycleButtonClick (GameObject go)
	{
		doorPiece.RotateColour();
	}
	
	protected override void OnPress(bool isPressed)
	{
		if(isPressed)
		{			
			base.OnPress (isPressed);
		}
	}
}
