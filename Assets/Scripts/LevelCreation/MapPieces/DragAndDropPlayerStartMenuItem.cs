using UnityEngine;
using System.Collections;

public class DragAndDropPlayerStartMenuItem : DragAndDropMenuItem
{
	GameObject cycleColourButton;

	PlayerStartPiece playerStartPiece;
	GameObject playerSpherePrefab;

	void Start()
	{
		cycleColourButton = transform.Find("CycleColourButton").gameObject;
		UIEventListener.Get(cycleColourButton).onClick = HandleColourCycleButtonClick;

		playerStartPiece = transform.Find("PlayerStartCubeMenuObj").GetComponent<PlayerStartPiece>();
		playerSpherePrefab = (GameObject)Resources.Load("Player");
	}

	void HandleColourCycleButtonClick (GameObject go)
	{
		playerStartPiece.RotateColour(true);
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
