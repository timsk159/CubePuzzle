using UnityEngine;
using System.Collections;

public class PlayerStartPiece : ColorCollisionObject 
{
	protected override void Start ()
	{

	}

	public override void ChangeColour (Colour colorToChangeTo)
	{
		objColour = colorToChangeTo;
	}
}
