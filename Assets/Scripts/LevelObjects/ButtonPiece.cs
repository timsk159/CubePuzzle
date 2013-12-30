using UnityEngine;
using System;
using System.Collections;

public class ButtonPiece : TriggerObject
{
	public bool rotateForward = true;
	public bool shouldChangePlayer;
	public Colour buttonSphereColour;
	HoloBeam beam;
	
	protected override void Start()
	{
		objColour = Colour.None;
		beam = GetComponentInChildren<HoloBeam>();
		base.Start();
	}

	protected override void TriggererEntered(GameObject go)
	{
		base.TriggererEntered(go);
		PressButton();
	}
	
	protected override void TriggererExited(GameObject go)
	{
		base.TriggererExited(go);
		PlayerLeftButton();
	} 

	public override void RotateColour ()
	{
		int currentColourIndex = (int)buttonSphereColour;
		//var values = Enum.GetValues(typeof(Colour));

		currentColourIndex++;

		if(currentColourIndex == cachedEnumValues.Length)
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
			Messenger.Invoke(ColourCollisionMessage.ButtonPressed.ToString());
		}	
	
		else
		{
			LevelController.Instance.playerChar.RotateColour();
		}
		RotateColour();
		//Play any animations here.
		GameObject animTarget;
		if(shouldChangePlayer)
			animTarget = transform.Find("Sphere").gameObject;
		else
			animTarget = transform.Find("Box002").gameObject;

		beam.IntensifyRimAndBrightness();
		iTween.PunchScale(animTarget, iTween.Hash("amount", new Vector3(0.5f, 0.5f, 0.5f), "time", 0.5f, "easetype", iTween.EaseType.easeInOutSine));
	}

	void PlayerLeftButton()
	{
		//Play any animation here.


	}
}
