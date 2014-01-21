using UnityEngine;
using System.Collections;

public class EndLevelFloorPiece : TriggerObject
{	
	protected override void Start ()
	{
		objColour = Colour.None;
		base.Start ();
	}

	protected override void TriggererEntered(GameObject go)
	{
		if(go.CompareTag("Player"))
		{
			LevelController.Instance.FinishLevel();
		}
	}

	public override void ChangeColour (Colour colorToChangeTo)
	{
		return;
	}
}
