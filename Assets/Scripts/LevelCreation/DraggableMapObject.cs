using UnityEngine;
using System.Collections;

public class DraggableMapObject : MonoBehaviour 
{
	void Start()
	{
		if (Application.loadedLevelName != "LevelCreator")
			Destroy(this);
		else
		{

			var prefabID = GetComponent<PrefabIdentifier>();

			if (prefabID == null)
				prefabID = gameObject.AddComponent<PrefabIdentifier>();

			prefabID.Components.Add(GetType().FullName);
		}

	}

	protected virtual void OnPress(bool isPressed)
	{
		if(UICamera.currentTouchID == -1 && isPressed)
		{
			NotificationCenter<DragAndDropNotification>.DefaultCenter.PostNotification(DragAndDropNotification.MapObjectPressed, gameObject);
		}
	}

	protected virtual void OnDestroy()
	{
		NotificationCenter<DragAndDropNotification>.DefaultCenter.PostNotification(DragAndDropNotification.MapObjectRemoved, this);
	}
}
