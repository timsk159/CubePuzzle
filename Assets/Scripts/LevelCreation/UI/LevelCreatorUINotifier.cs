using UnityEngine;
using System.Collections;

public enum LevelCreatorUINotification
{
	GenericInputSubmitted, CreateButtonClicked,
	LoadMenuLoadClicked, LoadMenuCancelClicked,
	SaveMenuSaveClicked, SaveMenuCancelClicked
};

public class LevelCreatorUINotifier : MonoBehaviour 
{
	public LevelCreatorUINotification notiType;
	
	public string dataToSend;
	
	UIInput inputObj;
	
	void Start()
	{
		if(notiType == LevelCreatorUINotification.GenericInputSubmitted)
		{
			inputObj = GetComponent<UIInput>();
		}
	}
	
	void OnClick()
	{
		if(notiType != LevelCreatorUINotification.GenericInputSubmitted)
			NotificationCenter<LevelCreatorUINotification>.DefaultCenter.PostNotification(notiType, dataToSend);
	}
	
	void OnSubmit()
	{
		if(inputObj == null)
			inputObj = GetComponent<UIInput>();
		
		var notiData = new InputNotificationData(gameObject, inputObj.text);
		
		NotificationCenter<LevelCreatorUINotification>.DefaultCenter.PostNotification(notiType, notiData);
	}
}

public struct InputNotificationData
{
	public GameObject go;
	public string theInput;
	
	public InputNotificationData(GameObject go, string theInput)
	{
		this.go = go;
		this.theInput = theInput;
	}
}