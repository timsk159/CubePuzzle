using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CutsceneObj : ScriptableObject
{
	public Dialogue dialogue;

	public AudioClip audioClip;

	public float lengthInSeconds;
}

[System.Serializable]
public class Dialogue
{
	public TextAsset dialogueAsset;

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