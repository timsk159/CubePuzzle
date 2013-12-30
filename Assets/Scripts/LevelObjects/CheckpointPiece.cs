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
	GameObject flagInstance;
	GameObject flagPole;
	GameObject flag;

	protected override void Start()
	{
		particles = GetComponentInChildren<ParticleSystem>();
		particles.Stop();
		flag = transform.Find("FlagPole/Flag").gameObject;

		Messenger.AddListener(CheckpointMessage.CheckpointPressed.ToString(), CheckpointPressed);

		flagPole = transform.Find("FlagPole").gameObject;

		DisableFlag();

		Messenger.AddListener(LevelStateMessage.LevelStarted.ToString(), EnableFlag);

		base.Start();
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


		flag.GetComponent<Cloth>().enabled = true;
		flag.GetComponent<InteractiveCloth>().enabled = true;
	}

	void DisableFlag()
	{
		flag.GetComponent<Cloth>().enabled = false;
		flag.GetComponent<InteractiveCloth>().enabled = false;
	}
}
