using UnityEngine;
using System.Collections;

public class DragAndDropEndPieceMenuItem : DragAndDropMenuItem
{
	protected override void OnPress(bool isPressed)
	{
		if(UICamera.currentTouchID == -1)
		{
			if(GameObject.Find("EndGameCube") == null)
			{
				base.OnPress (isPressed);
			}
		}
	}
}
