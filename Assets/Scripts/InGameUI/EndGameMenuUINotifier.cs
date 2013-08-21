using UnityEngine;
using System.Collections;

public enum EndGameMenuNotification
{
	NextLevelPressed, QuitPressed, QuitAndSavePressed
}

public class EndGameMenuUINotifier : MonoBehaviour 
{
	public EndGameMenuNotification notiType;
	
	void OnClick()
	{
		NotificationCenter<EndGameMenuNotification>.DefaultCenter.PostNotification(notiType, null);
	}
}
