using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class CutsceneObj 
{
	public TextAsset dialogueAsset;
	public int timeInSeconds;

	string[] _dialogueLines;

	public string dialogue
	{
		get{ return dialogueAsset.text;}
	}

	public string[] dialogueLines
	{
		get
		{
			if(_dialogueLines == null)
			{
				_dialogueLines = dialogue.Split(new string[] { "--BREAKLINE--" }, StringSplitOptions.RemoveEmptyEntries);
			}
			return _dialogueLines;
		}
	}
}
