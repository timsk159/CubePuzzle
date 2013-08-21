using UnityEngine;
using System.Collections;

public class DragAndDropMenuItem : MonoBehaviour 
{
	public GameObject prefab;
	
	protected virtual void OnPress (bool isPressed)
	{
		if (enabled)
		{
			if(isPressed)
			{
				NotificationCenter<DragAndDropNotification>.DefaultCenter.PostNotification(DragAndDropNotification.MenuItemPressed, prefab);
			}
		}
	}
}
