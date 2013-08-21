using UnityEngine;
using System.Collections;

public class FrontMenuUINotifier : MonoBehaviour 
{
	public FrontMenuUINotification notiType;
	
	void OnClick()
	{
		NotificationCenter<FrontMenuUINotification>.DefaultCenter.PostNotification(notiType, null);
	}
}
