using UnityEngine;
using System.Collections;

public class CheckpointPiece : InteractiveObject
{
	public override void RotateColour (bool forward)
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
			LevelController.Instance.SetCheckpoint();
		}
	}
}
