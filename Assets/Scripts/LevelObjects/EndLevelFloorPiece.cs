using UnityEngine;
using System.Collections;

public class EndLevelFloorPiece : ColorCollisionObject
{	
	protected override void Start ()
	{
		objColour = Colour.None;
		base.Start ();
	}

	protected override void OnCollisionEnter(Collision col)
	{
		if(col.collider.CompareTag("Player"))
		{
			LevelController.Instance.FinishLevel();
		}
	}

	public override void ChangeColour (Colour colorToChangeTo)
	{
		return;
	}
}
