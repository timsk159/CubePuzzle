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
		
		doorPiece = transform.Find("DoorCube").GetComponent<DoorPiece>();
	}

	void HandleDoorColourCycleButtonClick (GameObject go)
	{
		doorPiece.RotateDoorColour();
	}

	void HandleColourCycleButtonClick (GameObject go)
	{
		doorPiece.RotateColour(true);
	}
	
	protected override void OnPress(bool isPressed)
	{
		if(isPressed)
		{
			var prefabCache = prefab;
			
			var go = (GameObject)Instantiate(prefab);
			
			go.GetComponent<DoorPiece>().SetDoorColour(doorPiece.theDoor.objColour);
			go.GetComponent<DoorPiece>().ChangeColour(doorPiece.objColour);
			go.AddComponent<DraggableRotatableMapObject>();

			prefab = go;
			
			base.OnPress (isPressed);
			
			Destroy(go);
			
			prefab = prefabCache;
		}
	}
}
