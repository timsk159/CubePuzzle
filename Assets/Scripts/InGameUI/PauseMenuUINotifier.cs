using UnityEngine;
using System.Collections;

public enum PauseMenuMessage
{
	ResumeButtonClicked, QuitButtonClicked, RestartButtonClicked, ReloadCheckpointClicked, ControlsPressed, StopTesting
};

public class PauseMenuUINotifier : MonoBehaviour 
{
	public PauseMenuMessage notiType;
	
	void OnClick()
	{
		Messenger.Invoke(notiType.ToString());
	}
}
