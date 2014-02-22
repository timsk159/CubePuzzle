using UnityEngine;
using System.Collections;

enum DragAndDropMessage
{
	MenuItemPressed, MapObjectPressed, ObjectPlaced, DoubleClicked, MapObjectRemoved, MenuItemRightClicked
};

public class DragAndDropController : MonoBehaviour 
{
	Vector3 lastMousePos;
	LevelCreator levelCreator;
	
	public GameObject playerSpherePrefab;
	
	public GameObject draggingObj;
	LevelAssetManager assetManager;

	GameObject selectedMenuItem;
	GameObject selectedMenuItemPrefab;
	GameObject selectedMenuItemHighlight;

	int layerMask = 0;
	
	void Start()
	{
		var levelCreationLayer = 1 << 10;
		var nullLayer = 1 << 12;
		layerMask = levelCreationLayer | nullLayer;

		levelCreator = GameObject.Find("LevelCreator").GetComponent<LevelCreator>();
		assetManager = GameObject.Find("LevelCreator").GetComponent<LevelAssetManager>();

		Messenger<GameObject>.AddListener(DragAndDropMessage.MenuItemPressed.ToString(), MenuItemPressed);
		Messenger<GameObject>.AddListener(DragAndDropMessage.MapObjectPressed.ToString(), MapObjectPressed);
		Messenger<Vector3>.AddListener(DragAndDropMessage.DoubleClicked.ToString(), DoubleClicked);
		Messenger<GameObject>.AddListener(DragAndDropMessage.MenuItemRightClicked.ToString(), MenuItemRightClicked);
		Messenger<StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData>.AddListener(LevelCreatorStateMessage.TestingMapEnter.ToString(), TestingMapEnter);
	}

	void OnDestroy()
	{
		Messenger<GameObject>.RemoveListener(DragAndDropMessage.MenuItemPressed.ToString(), MenuItemPressed);
		Messenger<GameObject>.RemoveListener(DragAndDropMessage.MapObjectPressed.ToString(), MapObjectPressed);
		Messenger<Vector3>.RemoveListener(DragAndDropMessage.DoubleClicked.ToString(), DoubleClicked);
		Messenger<GameObject>.RemoveListener(DragAndDropMessage.MenuItemRightClicked.ToString(), MenuItemRightClicked);
		Messenger<StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData>.RemoveListener(LevelCreatorStateMessage.TestingMapEnter.ToString(), TestingMapEnter);
	}
	
	void MenuItemPressed(GameObject prefab)
	{
		if(draggingObj != null)
			Destroy(draggingObj);
		
		draggingObj = (GameObject)Instantiate(prefab);
		draggingObj.name = draggingObj.name.Replace("(Clone)","");

		if(draggingObj.name.Contains("Door"))
		{
			SetupDoorPiece(draggingObj);
		}
		else if(draggingObj.name.Contains("PlayerStart"))
		{
			SetupStartPiece(draggingObj);
		}

		if(draggingObj.transform.childCount > 0)
		{
			foreach(Transform child in draggingObj.transform)
			{
				if(child.collider)
					child.collider.enabled = false;
			}
		}
	
		var mousePos = Input.mousePosition;

		mousePos.z = 8.5f;

		var worldPos = Camera.main.ScreenToWorldPoint(mousePos);

		draggingObj.transform.position = worldPos;
	}

	void MenuItemRightClicked(GameObject prefab)
	{
		if(selectedMenuItemPrefab == prefab)
		{
			DeselectMenuItem();
		}
		else
		{
			DeselectMenuItem();

			SelectMenuItem(UICamera.lastHit.collider.gameObject);
			selectedMenuItemPrefab = prefab;
		}
	}

	void SelectMenuItem(GameObject newSelection)
	{
		var highlight = newSelection.transform.Find("HighlightSprite").gameObject;

		if(selectedMenuItemHighlight != null)
		{
			TweenAlpha.Begin(selectedMenuItemHighlight, 0.5f, 0.0f);
		}
		selectedMenuItemHighlight = highlight;
		selectedMenuItem = newSelection;

		TweenAlpha.Begin(selectedMenuItemHighlight, 0.5f, 1.0f);
	}

	void DeselectMenuItem()
	{
		if(selectedMenuItemHighlight != null)
		{
			TweenAlpha.Begin(selectedMenuItemHighlight, 0.5f, 0.0f);
			selectedMenuItemHighlight = null;
			selectedMenuItemPrefab = null;
		}
	}

	void MapObjectPressed(GameObject objPressed)
	{
		//If we are already dragging something, and it's not this object. Delete it (Something likely went wrong).
		if(draggingObj != null && objPressed != draggingObj)
		{
			Debug.LogError("Tried dragging an object when one was already dragging, this is bad!");
			Destroy(draggingObj);
		}

		ReplaceFloorPiece(objPressed.transform.position);
		draggingObj = objPressed;
		DoDragging(Vector3.up);
	}

	void TestingMapEnter(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData changeData)
	{
		DeselectMenuItem();
	}
	
	void Update()
	{
		if(draggingObj != null)
		{
			if(Input.GetMouseButtonUp(0))
			{
				FinishDragging();
				lastMousePos = Input.mousePosition;
				//return;
			}
			if(Input.GetMouseButton(0))
			{
				if(lastMousePos != Vector3.zero)
				{
					var delta = Input.mousePosition - lastMousePos;
					if(delta != Vector3.zero)
						DoDragging(delta);
				}
			}
		}
		if(selectedMenuItemPrefab != null)
		{
			if(UICamera.hoveredObject != null && UICamera.hoveredObject != selectedMenuItem)
			{
				if(Input.GetMouseButtonDown(1))
				{
					PlaceSelectedItem();
				}
			}
		}
		lastMousePos = Input.mousePosition;
	}
	
	void DoDragging(Vector3 delta)
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if(draggingObj != null)
		{
			if(draggingObj.transform.localScale.x < 1.1f)
				draggingObj.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
			
			if(draggingObj.transform.childCount > 0)
			{
				foreach(Transform child in draggingObj.transform)
				{
					child.gameObject.layer = 11;
				}
			}
			draggingObj.layer = 11;
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
			{
				if(hit.collider != null)
				{
					draggingObj.transform.position = hit.collider.transform.position;
				}
			}
			else
			{
				draggingObj.transform.position += (Vector3)(delta * Time.deltaTime);
				draggingObj.transform.parent = null;
			}
		}
	}

	void FinishDragging()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(lastMousePos);
	
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
		{
			if(hit.collider != null)
			{
				GameObject objToReplace = hit.collider.gameObject;

				if(draggingObj.GetComponent<DraggableMapObject>() == null)
				{
					draggingObj.AddComponent<DraggableMapObject>();
				}

				if(draggingObj.transform.childCount > 0)
				{
					foreach(Transform child in draggingObj.transform)
					{
						child.gameObject.layer = 10;
					}
				}
				draggingObj.layer = 10;
				draggingObj.transform.localScale = Vector3.one;
				//Before moving the object, double check that it's position is correct.
				draggingObj.transform.position = objToReplace.transform.position;

				draggingObj.transform.parent = levelCreator.mapRoot.transform;
				Messenger<GameObject>.Invoke(DragAndDropMessage.ObjectPlaced.ToString(), draggingObj);
				var camControls = Camera.main.GetComponent<CameraControls>();
				camControls.canMove = true;
				draggingObj = null;
				Destroy(objToReplace);
			}
		}
		else
		{
			Destroy(draggingObj);
			draggingObj = null;
		}
	}

	void PlaceSelectedItem()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
		{
			if(hit.collider != null)
			{
				var objToReplace = hit.collider.gameObject;

				var clone = (GameObject)Instantiate(selectedMenuItemPrefab);
				clone.name = clone.name.Remove(clone.name.IndexOf("(Clone)"), 7);

				if(clone.name.Contains("Door"))
				{
					SetupDoorPiece(clone);
				}
				else if(clone.name.Contains("PlayerStart"))
				{
					SetupStartPiece(clone);
					DeselectMenuItem();
				}
				else if(clone.name.Contains("End"))
				{
					DeselectMenuItem();
				}
				else if(clone.GetComponent<DraggableMapObject>() == null)
				{
					clone.AddComponent<DraggableMapObject>();
				}


				if(clone.transform.childCount > 0)
				{
					foreach(Transform child in clone.transform)
					{
						child.gameObject.layer = 10;
						if(child.collider)
						{
							child.collider.enabled = false;
						}
					}
				}
				clone.layer = 10;
				clone.transform.localScale = Vector3.one;
				clone.transform.position = hit.collider.transform.position;
				clone.transform.parent = levelCreator.mapRoot.transform;
				Messenger<GameObject>.Invoke(DragAndDropMessage.ObjectPlaced.ToString(), clone);

				Destroy(objToReplace);
			}
		}
	}

	void SetupDoorPiece(GameObject doorGo)
	{
		var camControls = Camera.main.GetComponent<CameraControls>();
		camControls.canMove = false;

		var doorMenuPieceGo = GameObject.Find("DoorCubeMenuObj");

		var doorMenuPiece = doorMenuPieceGo.GetComponent<DoorPiece>();
		var doorPiece = doorGo.GetComponent<DoorPiece>();

		doorPiece.SetDoorColour(doorMenuPiece.theDoor.objColour, false);
		doorPiece.theDoor.initialColour = doorMenuPiece.theDoor.objColour;
		doorPiece.ChangeColour(doorMenuPiece.objColour, false);
		doorPiece.initialColour = doorMenuPiece.objColour;
		doorGo.AddComponent<DraggableRotatableMapObject>();
	}

	void SetupStartPiece(GameObject startGo)
	{
		var menuPlayerSphere = GameObject.Find("PlayerStartCubeMenuObj");
		var menuPlayerObj = menuPlayerSphere.GetComponent<PlayerStartPiece>();

		var playerSphere = (GameObject)Instantiate(playerSpherePrefab);

		var playerCharObj = playerSphere.GetComponent<PlayerCharacter>();

		playerSphere.transform.parent = startGo.transform;
		playerSphere.transform.localPosition = new Vector3(0, 1.3f, 0);
		playerCharObj.rigidbody.useGravity = false;
		playerCharObj.playerMovement.canMove = false;

		startGo.GetComponent<PlayerStartPiece>().ChangeColour(menuPlayerObj.objColour);
		playerSphere.collider.enabled = false;
		playerCharObj.SilentlyChangeColour(menuPlayerObj.objColour);
		startGo.AddComponent<DraggableMapObject>();
	}

	void SetupEndPiece(GameObject endGo)
	{

	}

	void ReplaceFloorPiece(Vector3 pos)
	{
		GameObject nullPiece = (GameObject)Instantiate(assetManager.nullCubePrefab, pos, Quaternion.identity);
		nullPiece.transform.parent = levelCreator.mapRoot.transform;
		nullPiece.name = nullPiece.name.Replace("(Clone)", "");
	}

	void DoubleClicked(Vector3 pos)
	{
		ReplaceFloorPiece (pos);
	}
}
