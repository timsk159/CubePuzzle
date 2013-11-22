using UnityEngine;
using System.Collections;

public enum PauseMenuNotification
{
	ResumeButtonClicked, QuitButtonClicked, RestartButtonClicked, ReloadCheckpointClicked
};

public class PauseMenuUINotifier : MonoBehaviour 
{
	public PauseMenuNotification notiType;
	
	void OnClick()
	{
		Messenger.Invoke(notiType.ToString());
	}
}
