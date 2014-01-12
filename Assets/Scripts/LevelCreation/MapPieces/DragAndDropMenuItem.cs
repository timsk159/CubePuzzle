using UnityEngine;
using System.Collections;

public class DragAndDropMenuItem : MonoBehaviour 
{
	public GameObject prefab;
	GameObject dashBoard;

	void Awake()
	{
		if(dashBoard == null)
		{
			dashBoard = transform.parent.parent.gameObject;
		}
	}
	
	protected virtual void OnPress (bool isPressed)
	{
		if (enabled)
		{
			if(isPressed)
			{
				if(UICamera.currentTouchID == -1)
				{
					Messenger<GameObject>.Invoke(DragAndDropMessage.MenuItemPressed.ToString(), prefab);
				}
				else if(UICamera.currentTouchID == -2)
				{
					Messenger<GameObject>.Invoke(DragAndDropMessage.MenuItemRightClicked.ToString(), prefab);
				}
			}
		}
	}
}
