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

		SetDoorColour(theDoor.objColour);
	}
	
	public override void ChangeColour(Colour colorToChangeTo)
	{
		useSharedMaterial = false;

		base.ChangeColour (colorToChangeTo);
		
		CheckDoor();
	}

	public override void RotateColour ()
	{
		useSharedMaterial = false;

		base.RotateColour ();

		CheckDoor();
	}
	
	public void RotateDoorColour()
	{
		int currentColourIndex = (int)theDoor.objColour;
		//var values = Enum.GetValues(typeof(Colour));

		currentColourIndex++;
			
		if(currentColourIndex == cachedEnumValues.Length)
		{
			currentColourIndex = 1;
		}
		SetDoorColour((Colour)currentColourIndex);
	}
	
	public void SetDoorColour(Colour colourToSet)
	{		
		theDoor.objColour = colourToSet;
		theDoor.renderer.material.color = ColorManager.GetObjectRealColor(theDoor.objColour);
		
		CheckDoor();
	}
	
	void CheckDoor()
	{
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
