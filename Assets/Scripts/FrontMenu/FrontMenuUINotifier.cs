using UnityEngine;
using System.Collections;

public class FrontMenuUINotifier : MonoBehaviour 
{
	public FrontMenuUINotification notiType;
	public string payload;

	void OnClick()
	{
		if(string.IsNullOrEmpty(payload))
			Messenger.Invoke(notiType.ToString());
		else
			Messenger<string>.Invoke(notiType.ToString(), payload);
	}
}
