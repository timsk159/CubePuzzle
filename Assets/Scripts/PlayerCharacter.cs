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

	Vector3 previousVelocity;

	[SerializeThis()][SerializeField()]
	bool canReset;
	
	void Start () 
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
		//Play smoke trail after sudden changes in speed / direction
		if(smokeParticles != null)
		{
			var deltaV = rigidbody.velocity - previousVelocity;

			var deltaSqrMag = deltaV.sqrMagnitude;

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
				if(deltaSqrMag > 0.8f)
					smokeParticles.Play();
			}
			previousVelocity = rigidbody.velocity;
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
		if(childLight != null)
			childLight.color = realColor;
		Messenger<Colour>.Invoke(ColourCollisionMessage.PlayerChangedColour.ToString(), currentColor);
	}

	public void SilentlyChangeColour(Colour colourToChangeTo)
	{
		currentColor = colourToChangeTo;
		var realColor = ColorManager.GetObjectRealColor(currentColor);
		gameObject.renderer.material.color = realColor;
		if(childLight != null)
			childLight.color = realColor;
	}
	
	public void RotateColour()
	{
		int currentColourIndex = (int)currentColor;
		currentColourIndex++;
		
		if(currentColourIndex == ColorManager.cachedColourValues.Length)
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

	void OnDeserialized()
	{
		StartCoroutine(ChangeAfterFrame());
	}

	IEnumerator ChangeAfterFrame()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		ChangeColour(currentColor);		
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
				var relativeVel = collision.relativeVelocity.sqrMagnitude;
				if(relativeVel > 12 && relativeVel < 16)
				{
					Messenger.Invoke(PlayerMessage.HitWall.ToString());
				}
				else if(relativeVel > 16)
				{
					Messenger.Invoke(PlayerMessage.HitWallHard.ToString());
				}
			}
		}
	}
}
