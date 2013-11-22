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
					Messenger<GameObject>.Invoke(DragAndDropNotification.MenuItemPressed.ToString(), prefab);
				}
				else if(UICamera.currentTouchID == -2)
				{
					Messenger<GameObject>.Invoke(DragAndDropNotification.MenuItemRightClicked.ToString(), prefab);
				}
			}
		}
	}
}
