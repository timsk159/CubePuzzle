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
		if(GameObject.Find("PlayerStartCube") == null)
		{
			if(isPressed && UICamera.currentTouchID == -1)
			{
				var prefabCache = prefab;

				var go = (GameObject)Instantiate(prefab);

				var playerSphere = (GameObject)Instantiate (playerSpherePrefab);
				var playerCharObj = playerSphere.GetComponent<PlayerCharacter>();
				playerSphere.transform.parent = go.transform;
				playerSphere.transform.localPosition = new Vector3(0, 1.3f, 0);
				playerCharObj.rigidbody.useGravity = false;
				playerCharObj.playerMovement.canMove = false;

				go.GetComponent<PlayerStartPiece>().ChangeColour(playerStartPiece.objColour);
				playerSphere.collider.enabled = false;
				playerCharObj.SilentlyChangeColour(playerStartPiece.objColour);

				prefab = go;

				base.OnPress (isPressed);

				Destroy(go);

				prefab = prefabCache;
			}
		}
	}
}
