using UnityEngine;
using System.Collections;

public enum CheckpointMessage
{
	CheckpointPressed
};

public class CheckpointPiece : TriggerObject
{
	bool activeCheckpoint;

	ParticleSystem particles;
	GameObject flagPrefab;
	GameObject flagPole;
	GameObject flag;

	protected override void Start()
	{
		particles = GetComponentInChildren<ParticleSystem>();
		particles.Stop();

		if (flagPrefab == null)
			flagPrefab = (GameObject)Resources.Load("FlagCloth");

		Messenger.AddListener(CheckpointMessage.CheckpointPressed.ToString(), CheckpointPressed);

		flagPole = transform.Find("FlagPole").gameObject;

		Messenger.AddListener(LevelStateMessage.LevelStarted.ToString(), EnableFlag);

		base.Start();
	}

	protected override void OnDestroy()
	{
		Messenger.RemoveListener(CheckpointMessage.CheckpointPressed.ToString(), CheckpointPressed);
		Messenger.RemoveListener(LevelStateMessage.LevelStarted.ToString(), EnableFlag);
		base.OnDestroy();
	}

	public override void RotateColour ()
	{
		return;
	}

	public override void ChangeColour(Colour colorToChangeTo)
	{
		return;
	}

	protected override void TriggererEntered(GameObject go)
	{
		if(go.CompareTag("Player"))
		{
			activeCheckpoint = true;
			LevelController.Instance.SetCheckpoint();

			//Send message to all other checkpoints.
			particles.Play();
			Messenger.RemoveListener(CheckpointMessage.CheckpointPressed.ToString(), CheckpointPressed);
			Messenger.Invoke(CheckpointMessage.CheckpointPressed.ToString());

			Messenger.AddListener(CheckpointMessage.CheckpointPressed.ToString(), CheckpointPressed);
		}
	}

	void CheckpointPressed()
	{
		activeCheckpoint = false;
		//Make sure particle system is off
		particles.Stop();
	}

	void EnableFlag()
	{

	}

	void DisableFlag()
	{

		//flag.GetComponent<Cloth>().enabled = false;
		//flag.GetComponent<InteractiveCloth>().enabled = false;
	}
}
