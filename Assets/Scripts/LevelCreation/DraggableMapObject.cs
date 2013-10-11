using UnityEngine;
using System.Collections;

//STRIP WHEN SAVING
public class DraggableMapObject : MonoBehaviour 
{
	float doubleClickStart;
	float doubleClickThreshold = 0.2f;

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
		/*
		if(UICamera.currentTouchID == -2 && isPressed)
		{
			if((Time.time - doubleClickStart) < doubleClickThreshold)
			{
				Destroy (gameObject);

				NotificationCenter<DragAndDropNotification>.DefaultCenter.PostNotification (DragAndDropNotification.DoubleClicked, transform.position);
			}
			else
			{
				doubleClickStart = Time.time;
			}
		}
		*/

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
