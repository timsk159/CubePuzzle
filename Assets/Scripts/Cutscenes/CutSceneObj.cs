using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CutsceneObj : ScriptableObject
{
	public CutSceneAnimation cutSceneAnim;

	public int lengthInSeconds;
}

[System.Serializable]
public struct CutSceneAnimation
{
	public List<TimedDialogue> timedDialogue;

	public List<TimedAudio> timedAudioClips;

	public List<TimedAnimationClip> timedAnimationClips;

	public void Init()
	{
		timedDialogue = new List<TimedDialogue>();
		timedAudioClips = new List<TimedAudio>();
		timedAnimationClips = new List<TimedAnimationClip>();
	}

	[System.Serializable]
	public class TimedDialogue
	{
		public Dialogue dialogue = new Dialogue();
		public float timeToStart;
		public float timeToFinish;
	}

	[System.Serializable]
	public class TimedAudio
	{
		public AudioClip clip;
		public float timeToStart;
	}

	[System.Serializable]
	public class TimedAnimationClip
	{
		public AnimationClip animClip;
		public float timeToStart;
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
}