using UnityEngine;
using System;
using System.Collections;

public class DoorPiece : FloorPiece 
{
	public Door theDoor;
	
	
	protected override void Start()
	{
		useSharedMaterial = false;

		base.Start ();

		theDoor = GetComponentInChildren<Door>();

		bool shouldCheckDoor = false;

		if(Application.loadedLevelName == "LevelCreator" && !LevelCreatorController.isTesting)
		{
			SetDoorColour(theDoor.objColour, false);
		}
		else
		{
			SetDoorColour(theDoor.objColour, true);
		}
	}
	
	public override void ChangeColour(Colour colorToChangeTo)
	{
		useSharedMaterial = false;

		base.ChangeColour (colorToChangeTo);
		
		CheckDoor();
	}

	public void ChangeColour(Colour colorToChangeTo, bool checkDoor)
	{
		useSharedMaterial = false;

		base.ChangeColour(colorToChangeTo);

		if(checkDoor)
			CheckDoor();
	}

	public override void RotateColour ()
	{
		useSharedMaterial = false;

		base.RotateColour ();

		CheckDoor();
	}

	public void RotateColour(bool checkDoor)
	{
		useSharedMaterial = false;

		int currentColourIndex = (int)objColour;
		//var values = Enum.GetValues(typeof(Colour));

		currentColourIndex++;

		if(currentColourIndex == cachedEnumValues.Length)
		{
			currentColourIndex = 1;
		}

		ChangeColour((Colour)currentColourIndex, checkDoor);
	}

	public void RotateDoorColour(bool checkDoor)
	{
		int currentColourIndex = (int)theDoor.objColour;
		//var values = Enum.GetValues(typeof(Colour));

		currentColourIndex++;
			
		if(currentColourIndex == cachedEnumValues.Length)
		{
			currentColourIndex = 1;
		}
		SetDoorColour((Colour)currentColourIndex, checkDoor);
	}
	
	public void SetDoorColour(Colour colourToSet, bool checkDoor)
	{		
		theDoor.objColour = colourToSet;
		theDoor.renderer.material.color = ColorManager.GetObjectRealColor(theDoor.objColour);
		Debug.Log("Door colour being set to: " + colourToSet + " Checking door?: " + checkDoor);
		if(checkDoor)
			CheckDoor();
	}
	
	void CheckDoor()
	{
		Debug.Log("Checking door");
		if(theDoor.objColour == objColour)
		{			
			theDoor.OpenDoor();
		}
		else
		{
			theDoor.CloseDoor();
		}
	}
}
