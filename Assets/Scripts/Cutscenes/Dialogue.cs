using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class Dialogue
{
	public string dialogueAssetPath;

	private TextAsset _dialogueAsset;

	public TextAsset dialogueAsset
	{
		get
		{
			if(_dialogueAsset == null)
				_dialogueAsset = (TextAsset)Resources.Load(dialogueAssetPath);
			return _dialogueAsset;
		}
	}

	string[] _dialogueLines;

	public string dialogueText
	{
		get{ return dialogueAsset.text;}
	}

	public string[] dialogueLines
	{
		get
		{
			Debug.Log("Generating dialogue lines");
			_dialogueLines = dialogueText.Split(new string[] { "--BREAKLINE--" }, StringSplitOptions.RemoveEmptyEntries);
			return _dialogueLines;
		}
	}

	public void UnloadDialogue()
	{
		Resources.UnloadAsset(_dialogueAsset);
		_dialogueAsset = null;
	}
}