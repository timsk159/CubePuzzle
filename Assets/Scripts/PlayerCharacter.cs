using UnityEngine;
using System;
using System.Collections;

public enum PlayerInteractionNotification
{
	PlayerInteracted
};

[RequireComponent(typeof(PlayerMovement))]
public class PlayerCharacter : MonoBehaviour 
{
	public PlayerMovement playerMovement;
	public Colour currentColor;
	ParticleSystem smokeParticles;

	//The current interaction object that the player is stood in
	InteractiveObject currentInteractionObject;
	[SerializeThis()][SerializeField()]
	bool canReset;
	
	void Awake () 
	{
		smokeParticles = transform.Find("SmokeTrail").GetComponent<ParticleSystem>();
		smokeParticles.transform.parent = null;
		playerMovement = GetComponent<PlayerMovement>();
		
		NotificationCenter<ColourCollisionNotification>.DefaultCenter.AddObserver(this, ColourCollisionNotification.PlayerEnteredColour);
		NotificationCenter<ColourCollisionNotification>.DefaultCenter.AddObserver(this, ColourCollisionNotification.PlayerExitedColour);
		
		NotificationCenter<ColourCollisionNotification>.DefaultCenter.AddObserver(this, ColourCollisionNotification.InteractionTriggerEnter);
		NotificationCenter<ColourCollisionNotification>.DefaultCenter.AddObserver(this, ColourCollisionNotification.InteractionTriggerExit);
		
		NotificationCenter<ColourCollisionNotification>.DefaultCenter.AddObserver(this, ColourCollisionNotification.PlayerChangedColour);

		NotificationCenter<LevelStateNotification>.DefaultCenter.AddObserver(this, LevelStateNotification.LevelStarted);
		NotificationCenter<LevelStateNotification>.DefaultCenter.AddObserver(this, LevelStateNotification.InGameExit);
		NotificationCenter<LevelStateNotification>.DefaultCenter.AddObserver(this, LevelStateNotification.InGameEnter);
	}
	
	void Update()
	{
		if(smokeParticles.isPlaying)
		{
			var smokePos = transform.position;
			smokePos.y -= 0.4f;
			smokePos.z += 0.35f;
			smokeParticles.transform.position = smokePos;

			smokeParticles.transform.forward = -rigidbody.velocity.normalized;
		}
		if(!smokeParticles.isPlaying)
		{
			if(playerMovement.isMovingFast)
				smokeParticles.Play();
		}
		else if(smokeParticles.isPlaying && !smokeParticles.isStopped && !playerMovement.isMovingFast)
		{
			smokeParticles.Stop();
		}
		if(Application.loadedLevelName != "LevelCreator")
		{
			if(Input.GetKeyDown(KeyCode.E))
			{
				if(currentInteractionObject != null)
				{
					currentInteractionObject.PlayerInteracted();
				}
			}
			if(canReset && !LevelSerializer.IsDeserializing)
			{
				if(Input.GetKeyDown(KeyCode.Space))
				{
					if(LevelController.Instance.hasCheckpoint)
						LevelController.Instance.LoadCheckpoint();
					else
						LevelController.Instance.ResetLevel();
				}
			}
		}
	}

	void LevelStarted()
	{
		canReset = true;
	}

	void InGameEnter()
	{
		canReset = true;
	}

	void InGameExit()
	{
		canReset = false;
	}

	void PlayerEnteredColour(NotificationCenter<ColourCollisionNotification>.Notification notiData)
	{
		//ColourCollisionData hitData = (ColourCollisionData)notiData.data;
	}
	
	void PlayerExitedColour(NotificationCenter<ColourCollisionNotification>.Notification notiData)
	{
		//ColourCollisionData hitData = (ColourCollisionData)notiData.data;
	}
	
	void InteractionTriggerEnter(NotificationCenter<ColourCollisionNotification>.Notification notiData)
	{
		currentInteractionObject = (InteractiveObject)notiData.data;
	}
	
	void InteractionTriggerExit(NotificationCenter<ColourCollisionNotification>.Notification notiData)
	{
		currentInteractionObject = null;
	}			
	
	public void ChangeColour(Colour colourToChangeTo)
	{
		currentColor = colourToChangeTo;
		gameObject.renderer.material.color = GetRealColor();
		NotificationCenter<ColourCollisionNotification>.DefaultCenter.PostNotification(ColourCollisionNotification.PlayerChangedColour, currentColor);
	}

	public void SilentlyChangeColour(Colour colourToChangeTo)
	{
		currentColor = colourToChangeTo;
		gameObject.renderer.material.color = GetRealColor();
	}
	
	public void RotateColour(bool forward)
	{
		int currentColourIndex = (int)currentColor;
		var values = Enum.GetValues(typeof(Colour));
		
		if(forward)
		{
			currentColourIndex++;
			
			if(currentColourIndex == values.Length)
			{
				currentColourIndex = 1;
			}
			
			ChangeColour((Colour)currentColourIndex);
		}
		else
		{
			currentColourIndex--;
			
			if(currentColourIndex == 1)
			{
				currentColourIndex = (int)values.GetValue(values.Length);
			}
			
			ChangeColour((Colour)currentColourIndex);
		}
	}
	
	public Color GetRealColor()
	{
		switch(currentColor)
		{
			case Colour.Red:
				return Color.red;
			case Colour.Green:
				return Color.green;
			case Colour.Blue:
				return Color.blue;
			default:
				return Color.white;
		}
	}

}
