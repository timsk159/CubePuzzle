using UnityEngine;
using System;
using System.Collections;

public class ButtonPiece : InteractiveObject
{
	public bool rotateForward = true;
	public bool shouldChangePlayer;
	public Colour buttonSphereColour;

	GameObject buttonSphere;
	
	protected override void Start()
	{
		objColour = Colour.None;
		base.Start();
		
		buttonSphere = transform.Find("ButtonSphere").gameObject;		
		
		SetButtonSphereColour(buttonSphereColour);
	}
	
	public override void PlayerInteracted()
	{
		//PressButton();
	}
	
	protected override void TriggererEntered(GameObject go)
	{
		base.TriggererEntered(go);
		PressButton();
	}
	
	protected override void TriggererExited(GameObject go)
	{
		base.TriggererExited(go);
		//Play any animation here.
	} 

	public override void RotateColour (bool forward)
	{
		int currentColourIndex = (int)buttonSphereColour;
		var values = Enum.GetValues(typeof(Colour));
		currentColourIndex++;


		if(currentColourIndex == values.Length)
		{
			currentColourIndex = 1;
		}

		SetButtonSphereColour((Colour)currentColourIndex);
	}

	public override void ChangeColour(Colour colorToChangeTo)
	{
		SetButtonSphereColour(colorToChangeTo);
	}

	void SetButtonSphereColour(Colour colourToChangeto)
	{
		if(buttonSphere == null)
			buttonSphere = transform.Find("ButtonSphere").gameObject;
	//	buttonSphere.renderer.material.color = GetObjectRealColor((Colour)colourToChangeto);
		buttonSphereColour = (Colour)colourToChangeto;
	}
	
	void PressButton()
	{
		if(!shouldChangePlayer)
		{
			NotificationCenter<ColourCollisionNotification>.DefaultCenter.PostNotification(ColourCollisionNotification.ButtonPressed, rotateForward);
		}	
	
		else
		{
			LevelController.Instance.playerChar.RotateColour(rotateForward);
		}
		RotateColour(true);
		//Play any animations here.
	}
}
