using UnityEngine;
using System.Collections;

public enum PauseMenuMessage
{
	ResumeButtonClicked, QuitButtonClicked, RestartButtonClicked, ReloadCheckpointClicked
};

public class PauseMenuUINotifier : MonoBehaviour 
{
	public PauseMenuMessage notiType;
	
	void OnClick()
	{
		Messenger.Invoke(notiType.ToString());
	}
}
