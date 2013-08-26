using UnityEngine;
using System.Collections;

public class Door : ColorCollisionObject
{
	bool doorIsDown = true;
	
	protected override void Start()
	{
		if (transform.parent.GetComponent<ColorCollisionObject>().objColour == objColour)
		{
			doorIsDown = true;
			OpenDoor();
		}

		base.Start ();
		doorIsDown = objColour != transform.parent.GetComponent<ColorCollisionObject>().objColour;

	}
	
	public override void ChangeColour(Colour colorToChangeTo)
	{
		return;
	}

	public override void RotateColour (bool forward)
	{
		return;
	}

	public void OpenDoor()
	{
		if(doorIsDown)
			animation.CrossFade("DoorOpen");
		doorIsDown = false;
	}
	
	public void CloseDoor()
	{
		if(!doorIsDown)
			animation.CrossFade("DoorClose");
		doorIsDown = true;
	}
}
