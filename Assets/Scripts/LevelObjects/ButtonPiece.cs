using UnityEngine;
using System;
using System.Collections;

public class ButtonPiece : InteractiveObject
{
	public bool rotateForward = true;
	public bool shouldChangePlayer;
	public Colour buttonSphereColour;
	
	protected override void Start()
	{
		objColour = Colour.None;
		base.Start();
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

	public override void RotateColour ()
	{
		int currentColourIndex = (int)buttonSphereColour;
		var values = Enum.GetValues(typeof(Colour));
		currentColourIndex++;


		if(currentColourIndex == values.Length)
		{
			currentColourIndex = 1;
		}
	}

	public override void ChangeColour(Colour colorToChangeTo)
	{

	}
	
	void PressButton()
	{
		if(!shouldChangePlayer)
		{
			Messenger.Invoke(ColourCollisionNotification.ButtonPressed.ToString());
		}	
	
		else
		{
			LevelController.Instance.playerChar.RotateColour();
		}
		RotateColour();
		//Play any animations here.
	}
}
