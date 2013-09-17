using UnityEngine;
using System.Collections;

public class FrontMenuUINotifier : MonoBehaviour 
{
	public FrontMenuUINotification notiType;
	public string payload;

	void OnClick()
	{
		NotificationCenter<FrontMenuUINotification>.DefaultCenter.PostNotification(notiType, payload);
	}
}
