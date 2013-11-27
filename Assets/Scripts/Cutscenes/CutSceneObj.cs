using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using JsonFx;

public class CutSceneObj : ScriptableObject
{
	public Dialogue dialogue;

	public string audioClipFilePath;

	private AudioClip _audioClip;

	public AudioClip audioClip
	{
		get
		{
			if(_audioClip == null)
				_audioClip = (AudioClip)Resources.Load(audioClipFilePath);
			return _audioClip;
		}
	}

	public string cameraAnimFilepath;

	private AnimationClip _cameraAnim;

	public AnimationClip cameraAnimation
	{
		get
		{
			if(_cameraAnim == null)
				_cameraAnim = (AnimationClip)Resources.Load(cameraAnimFilepath);
			return _cameraAnim;
		}
	}

	public float lengthInSeconds;

	public void UnloadAudio()
	{
		Resources.UnloadAsset(_audioClip);
		_audioClip = null;
	}

	public void UnloadCameraAnim()
	{
		Resources.UnloadAsset(_cameraAnim);
		_cameraAnim = null;
	}
}