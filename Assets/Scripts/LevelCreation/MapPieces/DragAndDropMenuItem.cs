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
				if(UICamera.currentTouchID == -1)
				{
					NotificationCenter<DragAndDropNotification>.DefaultCenter.PostNotification(DragAndDropNotification.MenuItemPressed, prefab);
				}
				else if(UICamera.currentTouchID == -2)
				{
					NotificationCenter<DragAndDropNotification>.DefaultCenter.PostNotification(DragAndDropNotification.MenuItemRightClicked, prefab);
				}
			}
		}
	}
}
