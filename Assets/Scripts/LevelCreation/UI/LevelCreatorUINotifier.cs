using UnityEngine;
using System.Collections;

public enum LevelCreatorUIMessage
{
	GenericInputSubmitted, CreateButtonClicked,
	LoadMenuLoadClicked, LoadMenuCancelClicked,
	SaveMenuSaveClicked, SaveMenuCancelClicked,

	NewMap, LoadMap, FrontMenuBack,
	NewMapBack, 
	Controls, Test, Save, Exit, ControlsBack
};

public class LevelCreatorUINotifier : MonoBehaviour 
{
	public LevelCreatorUIMessage notiType;
	
	public string dataToSend;
	
	UIInput inputObj;
	
	void Start()
	{
		if(notiType == LevelCreatorUIMessage.GenericInputSubmitted)
		{
			inputObj = GetComponent<UIInput>();
		}
	}
	
	void OnClick()
	{
		if(notiType != LevelCreatorUIMessage.GenericInputSubmitted)
			Messenger.Invoke(notiType.ToString());
	}
	
	void OnSubmit()
	{
		if(inputObj == null)
			inputObj = GetComponent<UIInput>();
		
		var notiData = new InputMessageData(gameObject, inputObj.value);

		Messenger<InputMessageData>.Invoke(notiType.ToString(), notiData);
	}
}

public struct InputMessageData
{
	public GameObject go;
	public string theInput;
	
	public InputMessageData(GameObject go, string theInput)
	{
		this.go = go;
		this.theInput = theInput;
	}
}