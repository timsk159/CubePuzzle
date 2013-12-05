using UnityEngine;
using System.Collections;

public enum EndGameMenuMessage
{
	NextLevelPressed, QuitPressed
}

public class EndGameMenuUINotifier : MonoBehaviour 
{
	public EndGameMenuMessage notiType;
	
	void OnClick()
	{
		Messenger.Invoke(notiType.ToString());
	}
}
