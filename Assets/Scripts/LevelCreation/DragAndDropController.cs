using UnityEngine;
using System.Collections;

enum DragAndDropNotification
{
	MenuItemPressed, MapObjectPressed, ObjectPlaced, DoubleClicked, MapObjectRemoved, MenuItemRightClicked
};

public class DragAndDropController : MonoBehaviour 
{
	Vector3 lastMousePos;
	LevelCreator levelCreator;
	
	public GameObject draggingObj;
	LevelAssetManager assetManager;

	GameObject selectedMenuItemPrefab;
	GameObject selectedMenuItemHighlight;
	
	void Start()
	{
		levelCreator = GameObject.Find("LevelCreator").GetComponent<LevelCreator>();
		assetManager = GameObject.Find("LevelCreator").GetComponent<LevelAssetManager>();
		
		NotificationCenter<DragAndDropNotification>.DefaultCenter.AddObserver(this, DragAndDropNotification.MenuItemPressed);
		NotificationCenter<DragAndDropNotification>.DefaultCenter.AddObserver(this, DragAndDropNotification.MapObjectPressed);
		NotificationCenter<DragAndDropNotification>.DefaultCenter.AddObserver(this, DragAndDropNotification.DoubleClicked);
		NotificationCenter<DragAndDropNotification>.DefaultCenter.AddObserver(this, DragAndDropNotification.MenuItemRightClicked);
	}
	
	void MenuItemPressed(NotificationCenter<DragAndDropNotification>.Notification notiData)
	{
		if(draggingObj != null)
			Destroy(draggingObj);
		
		GameObject prefab = (GameObject)notiData.data;
		
		draggingObj = (GameObject)Instantiate(prefab);
		draggingObj.name = draggingObj.name.Replace("(Clone)","");
		if(draggingObj.transform.childCount > 0)
		{
			foreach(Transform child in draggingObj.transform)
			{
				if(child.collider)
					child.collider.enabled = false;
			}
		}
		var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		worldPos.z = 2.5f;
		draggingObj.transform.position = worldPos;
	}

	void MenuItemRightClicked(NotificationCenter<DragAndDropNotification>.Notification notiData)
	{
		var prefab = (GameObject)notiData.data;

		if(selectedMenuItemPrefab == prefab)
		{
			DeselectMenuItem();
			selectedMenuItemPrefab = null;
		}
		else
		{
			DeselectMenuItem();

			var highlight = UICamera.lastHit.collider.transform.Find("HighlightSprite").gameObject;
			SelectMenuItem(highlight);
			selectedMenuItemPrefab = prefab;
		}
	}

	void SelectMenuItem(GameObject newSelection)
	{
		if(selectedMenuItemHighlight != null)
		{
			TweenAlpha.Begin(selectedMenuItemHighlight, 0.5f, 0.0f);
		}
		selectedMenuItemHighlight = newSelection;

		TweenAlpha.Begin(selectedMenuItemHighlight, 0.5f, 1.0f);
	}

	void DeselectMenuItem()
	{
		if(selectedMenuItemHighlight != null)
		{
			TweenAlpha.Begin(selectedMenuItemHighlight, 0.5f, 0.0f);
			selectedMenuItemHighlight = null;
		}
	}

	void MapObjectPressed(NotificationCenter<DragAndDropNotification>.Notification notiData)
	{
		GameObject objPressed = (GameObject)notiData.data;
		
		if(draggingObj != null && objPressed != draggingObj)
		{
			Destroy(draggingObj);
		}

		ReplaceFloorPiece(objPressed.transform.position);
		
		draggingObj = objPressed;
	}
	
	void Update()
	{
		if(draggingObj != null)
		{
			if(Input.GetMouseButton(0))
			{
				if(lastMousePos != null && lastMousePos != Vector3.zero)
				{
					var delta = Input.mousePosition - lastMousePos;
					DoDragging(delta);
				}
				
				lastMousePos = Input.mousePosition;
			}
			if(Input.GetMouseButtonUp(0))
			{
				FinishDragging();
			}
		}
		else if(selectedMenuItemPrefab != null)
		{
			if(Input.GetMouseButtonDown(1))
			{
				PlaceSelectedItem();
			}
		}
	}
	
	void DoDragging(Vector3 delta)
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		int layerMask = 1 << 10;
		
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
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		int layerMask = 1 << 10;
		
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
		{
			if(hit.collider != null)
			{
				GameObject objToReplace = hit.collider.gameObject;

				if(draggingObj.GetComponent<DraggableMapObject>() == null)
				{
					if(draggingObj.name.Contains("Door"))
					{
						draggingObj.AddComponent<DraggableRotatableMapObject>();
					}
					else
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
				draggingObj.transform.parent = levelCreator.mapRoot.transform;
				NotificationCenter<DragAndDropNotification>.DefaultCenter.PostNotification(DragAndDropNotification.ObjectPlaced, draggingObj.gameObject);
				draggingObj = null;
				Destroy(objToReplace);
			}
		}
		else
		{
			DraggableMapObject mapObjScript;
			mapObjScript = draggingObj.GetComponent<DraggableMapObject>();

			if(mapObjScript != null)
				Destroy(mapObjScript);
			
		}
	}

	void PlaceSelectedItem()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 10))
		{
			if(hit.collider != null)
			{
				var objToReplace = hit.collider.gameObject;

				var clone = (GameObject)Instantiate(selectedMenuItemPrefab);
				clone.name = clone.name.Remove(clone.name.IndexOf("(Clone)"), 7);

				if(clone.transform.childCount > 0)
				{
					foreach(Transform child in clone.transform)
					{
						child.gameObject.layer = 10;
					}
				}
				clone.layer = 10;
				clone.transform.localScale = Vector3.one;
				clone.transform.position = hit.collider.transform.position;
				clone.transform.parent = levelCreator.mapRoot.transform;

				Destroy(objToReplace);
			}
		}
	}
	
	void ReplaceFloorPiece(Vector3 pos)
	{
		GameObject nullPiece = (GameObject)Instantiate(assetManager.nullCubePrefab, pos, Quaternion.identity);
		nullPiece.transform.parent = levelCreator.mapRoot.transform;
		nullPiece.name = nullPiece.name.Replace("(Clone)", "");
	}

	void DoubleClicked(NotificationCenter<DragAndDropNotification>.Notification notiData)
	{
		var pos = (Vector3)notiData.data;

		ReplaceFloorPiece (pos);
	}
}
