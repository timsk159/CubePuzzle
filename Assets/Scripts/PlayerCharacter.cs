using UnityEngine;
using System;
using System.Collections;

using StateMachineMessenger = Messenger<StateMachine<LevelState, LevelStateMessage>.StateChangeData>;

public enum PlayerMessage
{
	HitWall, HitWallHard
};

[RequireComponent(typeof(PlayerMovement))]
public class PlayerCharacter : MonoBehaviour 
{
	public PlayerMovement playerMovement;
	public Colour currentColor;
	ParticleSystem smokeParticles;
	Light childLight;

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

		Messenger.AddListener(LevelIntroMessage.IntroFinished.ToString(), IntroFinished);
		StateMachineMessenger.AddListener(LevelStateMessage.InGameExit.ToString(), InGameExit);
		StateMachineMessenger.AddListener(LevelStateMessage.InGameEnter.ToString(), InGameEnter);
		Messenger<StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData>.AddListener(LevelCreatorStateMessage.TestingMapEnter.ToString(), TestingMapEnter);
		Messenger<StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData>.AddListener(LevelCreatorStateMessage.TestingMapExit.ToString(), TestingMapExit);
	}

	void OnDestroy()
	{
		Messenger.RemoveListener(LevelIntroMessage.IntroFinished.ToString(), IntroFinished);
		StateMachineMessenger.RemoveListener(LevelStateMessage.InGameExit.ToString(), InGameExit);
		StateMachineMessenger.RemoveListener(LevelStateMessage.InGameEnter.ToString(), InGameEnter);
		Messenger<StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData>.RemoveListener(LevelCreatorStateMessage.TestingMapEnter.ToString(), TestingMapEnter);
		Messenger<StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData>.RemoveListener(LevelCreatorStateMessage.TestingMapExit.ToString(), TestingMapExit);
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
		if(smokeParticles != null)
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
		}
		/*
		if(Input.GetKeyDown(KeyCode.E))
		{
			if(currentInteractionObject != null)
			{
				currentInteractionObject.PlayerInteracted();
			}
		}
		*/
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

	void IntroFinished()
	{
		canReset = true;
	}

	void InGameEnter(StateMachine<LevelState, LevelStateMessage>.StateChangeData changeData)
	{
		canReset = true;
	}

	void InGameExit(StateMachine<LevelState, LevelStateMessage>.StateChangeData changeData)
	{
		canReset = false;
	}
	
	public void ChangeColour(Colour colourToChangeTo)
	{
		currentColor = colourToChangeTo;
		var realColor = ColorManager.GetObjectRealColor(currentColor);
		gameObject.renderer.material.color = realColor;
		childLight.color = realColor;
		Messenger<Colour>.Invoke(ColourCollisionMessage.PlayerChangedColour.ToString(), currentColor);
	}

	public void SilentlyChangeColour(Colour colourToChangeTo)
	{
		currentColor = colourToChangeTo;
		var realColor = ColorManager.GetObjectRealColor(currentColor);
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

	void TestingMapEnter(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData changeData)
	{
		canReset = true;
	}

	void TestingMapExit(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData changeData)
	{
		canReset = false;
	}

	void OnCollisionEnter(Collision collision) 
	{
		//We only care about box colliders
		if(collision.collider is BoxCollider)
		{
			var theCollider = collision.collider as BoxCollider;
			//Check the hit collider is definitely a wall
			if(theCollider.size.y > 6)
			{
				Messenger.Invoke(PlayerMessage.HitWall.ToString());
				if(collision.relativeVelocity.magnitude > 2)
				{
					Messenger.Invoke(PlayerMessage.HitWallHard.ToString());
				}
			}
		}
	}
}
