using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class CutSceneObj
{
	public TextAsset dialogueAsset;
	public AudioClip narrationAudio;
	public AnimationClip cameraAnimation;
	public int lengthInSeconds;

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
