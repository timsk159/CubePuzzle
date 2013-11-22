using UnityEngine;
using System;
using System.Collections;

using StateMachineMessenger = Messenger<StateMachine<LevelState, LevelStateNotification>.StateChangeData>;

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
	Light childLight;

	//The current interaction object that the player is stood in
	InteractiveObject currentInteractionObject;
	[SerializeThis()][SerializeField()]
	bool canReset;
	
	void Awake () 
	{
		//No smoke in level creator.
		if(Application.loadedLevelName != "LevelCreator")
		{
			smokeParticles = transform.Find("SmokeTrail").GetComponent<ParticleSystem>();
			smokeParticles.transform.parent = null;
		}
		childLight = transform.Find("PointLight").GetComponent<Light>();

		playerMovement = GetComponent<PlayerMovement>();

		StateMachineMessenger.AddListener(LevelStateNotification.LevelStarted.ToString(), LevelStarted);
		StateMachineMessenger.AddListener(LevelStateNotification.InGameExit.ToString(), InGameExit);
		StateMachineMessenger.AddListener(LevelStateNotification.InGameEnter.ToString(), InGameEnter);
	}

	void OnDestroy()
	{
		StateMachineMessenger.RemoveListener(LevelStateNotification.LevelStarted.ToString(), LevelStarted);
		StateMachineMessenger.RemoveListener(LevelStateNotification.InGameExit.ToString(), InGameExit);
		StateMachineMessenger.RemoveListener(LevelStateNotification.InGameEnter.ToString(), InGameEnter);
	}

	public void DisablePhysics()
	{
		rigidbody.useGravity = false;
		rigidbody.isKinematic = true;
		collider.enabled = false;
	}

	public void EnablePhysics()
	{
		rigidbody.useGravity = true;
		rigidbody.isKinematic = false;
		collider.enabled = true;
	}
	
	void Update()
	{
		if(Application.loadedLevelName != "LevelCreator")
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

	void LevelStarted(StateMachine<LevelState, LevelStateNotification>.StateChangeData changeData)
	{
		canReset = true;
	}

	void InGameEnter(StateMachine<LevelState, LevelStateNotification>.StateChangeData changeData)
	{
		canReset = true;
	}

	void InGameExit(StateMachine<LevelState, LevelStateNotification>.StateChangeData changeData)
	{
		canReset = false;
	}
	
	public void ChangeColour(Colour colourToChangeTo)
	{
		currentColor = colourToChangeTo;
		var realColor = GetRealColor();
		gameObject.renderer.material.color = realColor;
		childLight.color = realColor;
		Messenger<Colour>.Invoke(ColourCollisionNotification.PlayerChangedColour.ToString(), currentColor);
	}

	public void SilentlyChangeColour(Colour colourToChangeTo)
	{
		currentColor = colourToChangeTo;
		var realColor = GetRealColor();
		gameObject.renderer.material.color = realColor;
		childLight.color = realColor;
	}
	
	public void RotateColour()
	{
		int currentColourIndex = (int)currentColor;
		var values = Enum.GetValues(typeof(Colour));

		currentColourIndex++;
		
		if(currentColourIndex == values.Length)
		{
			currentColourIndex = 1;
		}
		
		ChangeColour((Colour)currentColourIndex);
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
