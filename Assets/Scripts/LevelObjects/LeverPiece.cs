using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LeverPiece : InteractiveObject 
{
	Animation interactionAnimation;
	
	public bool shouldChangePlayer;
	public bool shouldChangeFloorPieces;
	
	public Colour colorToChange;
	public Colour colorToChangeTo;
	
	bool pulledState;
	GameObject playerObject;
	List<GameObject> objectsToChange;
	
	protected override void Start()
	{
		interactionAnimation = GetComponentInChildren<Animation>();
		objectsToChange = new List<GameObject>();
		playerObject = GameObject.FindWithTag("Player");
		//Gets all ColorCollisionObjects, then narrows it down to only those with the colour we want.
		if(shouldChangeFloorPieces)
		{
			objectsToChange.AddRange(GameObject.FindGameObjectsWithTag("FloorPiece").Where(e => e.GetComponent<ColorCollisionObject>() != null).Where(e => 
				e.GetComponent<ColorCollisionObject>().objColour == colorToChange).ToList());
		}		
	}
	
	public override void PlayerInteracted()
	{
		PullLever();
	}
	
	void PullLever()
	{
		if(!pulledState)
		{
			var playerCharacter = playerObject.GetComponent<PlayerCharacter>();
			
			if(playerCharacter != null)
				playerCharacter.ChangeColour(colorToChangeTo);
			
			if(shouldChangePlayer)
				NotificationCenter<ColourCollisionNotification>.DefaultCenter.PostNotification(ColourCollisionNotification.PlayerChangedColour, colorToChangeTo);
			
			foreach(var go in objectsToChange)
			{
				var colorCollisionObject = go.GetComponent<ColorCollisionObject>();
						
				if(colorCollisionObject != null)
					go.GetComponent<ColorCollisionObject>().ChangeColour(colorToChangeTo);
			}

			if(shouldChangeFloorPieces)
				NotificationCenter<ColourCollisionNotification>.DefaultCenter.PostNotification(ColourCollisionNotification.FloorPiecesChangedColour, colorToChangeTo);
			interactionAnimation.Play("LeverForward");
		}
		else
		{
			var playerCharacter = playerObject.GetComponent<PlayerCharacter>();
			
			if(playerCharacter != null)
				playerCharacter.ChangeColour(colorToChange);
			
			if(shouldChangePlayer)
				NotificationCenter<ColourCollisionNotification>.DefaultCenter.PostNotification(ColourCollisionNotification.PlayerChangedColour, colorToChange);
			
			foreach(var go in objectsToChange)
			{
				var colorCollisionObject = go.GetComponent<ColorCollisionObject>();
			
				if(colorCollisionObject != null)
					go.GetComponent<ColorCollisionObject>().ChangeColour(colorToChange);
			}
			
			if(shouldChangeFloorPieces)
				NotificationCenter<ColourCollisionNotification>.DefaultCenter.PostNotification(ColourCollisionNotification.FloorPiecesChangedColour, colorToChange);
			
			interactionAnimation.Play("LeverBack");
		}
		
		pulledState = !pulledState;
	}
}