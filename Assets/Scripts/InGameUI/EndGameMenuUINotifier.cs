using UnityEngine;
using System.Collections;

public enum EndGameMenuMessage
{
	NextLevelPressed, QuitPressed, StopTesting, WebsiteButton
}

public class EndGameMenuUINotifier : MonoBehaviour 
{
	public EndGameMenuMessage notiType;

	public string url;

	void OnClick()
	{
		Messenger.Invoke(notiType.ToString());
	}
}
