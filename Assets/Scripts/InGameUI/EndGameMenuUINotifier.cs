using UnityEngine;
using System.Collections;

public enum EndGameMenuNotification
{
	NextLevelPressed, QuitPressed
}

public class EndGameMenuUINotifier : MonoBehaviour 
{
	public EndGameMenuNotification notiType;
	
	void OnClick()
	{
		Messenger.Invoke(notiType.ToString());
	}
}
