using UnityEngine;
using System.Collections;

public enum OptionsMenuMessage
{
	BackPressed
};

public class OptionsMenuUINotifier : MonoBehaviour 
{
	public OptionsMenuMessage messageType;

	void OnClick()
	{
		Messenger.Invoke(messageType.ToString());
	}
}
