using UnityEngine;
using System.Collections;

public class DragAndDropPlayerStartMenuItem : DragAndDropMenuItem
{
	GameObject cycleColourButton;

	PlayerStartPiece playerStartPiece;

	void Start()
	{
		cycleColourButton = transform.Find("CycleColourButton").gameObject;
		UIEventListener.Get(cycleColourButton).onClick = HandleColourCycleButtonClick;

		playerStartPiece = transform.Find("PlayerStartCubeMenuObj").GetComponent<PlayerStartPiece>();
	}

	void HandleColourCycleButtonClick (GameObject go)
	{
		playerStartPiece.RotateColour();
		GetComponentInChildren<PlayerCharacter>().SilentlyChangeColour(playerStartPiece.objColour);
	}

	protected override void OnPress(bool isPressed)
	{
		if(UICamera.currentTouchID == -1)
		{
			if(GameObject.Find("PlayerStartCube") == null)
			{
				if(isPressed)
				{
					base.OnPress(isPressed);
				}
			}
		}
	}
}
