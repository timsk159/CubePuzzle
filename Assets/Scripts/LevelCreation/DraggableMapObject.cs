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
			Messenger<GameObject>.Invoke(DragAndDropMessage.MapObjectPressed.ToString(), gameObject);
		}
	}

	protected virtual void OnDestroy()
	{
		Messenger<DraggableMapObject>.Invoke(DragAndDropMessage.MapObjectRemoved.ToString(), this);
	}
}
