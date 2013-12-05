using UnityEngine;
using System.Collections;

public class FrontMenuUINotifier : MonoBehaviour 
{
	public FrontMenuUIMessage notiType;
	public string payload;

	void OnClick()
	{
		if(string.IsNullOrEmpty(payload))
			Messenger.Invoke(notiType.ToString());
		else
			Messenger<string>.Invoke(notiType.ToString(), payload);
	}
}
