using UnityEngine;
using System.Collections;

public enum OptionsMenuMessage
{
	Back, Apply
};

public class OptionsMenuUINotifier : MonoBehaviour 
{
	public OptionsMenuMessage messageType;

	void OnClick()
	{
		Messenger.Invoke(messageType.ToString());
	}
}
