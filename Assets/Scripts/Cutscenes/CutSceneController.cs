using UnityEngine;
using System.Collections;

public enum CutSceneNotification
{
	CutSceneStarted, CutSceneFinished
};

public class CutSceneController : MonoBehaviour 
{
	public Camera cutSceneCamera;
	public DialogueDisplayer cutSceneLabel;

	void Awake()
	{
	}

	void TriggererEntered(CutSceneObj cutSceneObj)
	{
		NotificationCenter<CutSceneNotification>.DefaultCenter.PostNotification(CutSceneNotification.CutSceneStarted, cutSceneObj.lengthInSeconds);

		StartCoroutine(CutSceneTimerRoutine(cutSceneObj.lengthInSeconds));

		//Do cutsceney stuff here.
		if(cutSceneObj.cameraAnimation != null)
		{
			cutSceneCamera.depth = 1;

			if(cutSceneCamera.animation.GetClip(cutSceneObj.cameraAnimation.name) == null)
			{
				cutSceneCamera.animation.AddClip(cutSceneObj.cameraAnimation, cutSceneObj.cameraAnimation.name);
			}
			cutSceneCamera.animation.Play(cutSceneObj.cameraAnimation.name);
		}
		if(cutSceneObj.dialogue != null)
		{
			cutSceneLabel.StartCoroutine(cutSceneLabel.DisplayText(cutSceneObj.dialogueLines));
		}
		if(cutSceneObj.narrationAudio != null)
		{
			audio.clip = cutSceneObj.narrationAudio;
			audio.Play();
		}
	}

	void StopCutScene()
	{
		//Put camera back, unload resources.
		cutSceneCamera.depth = -2;


		NotificationCenter<CutSceneNotification>.DefaultCenter.PostNotification(CutSceneNotification.CutSceneFinished, null);
	}
	
	IEnumerator CutSceneTimerRoutine(int length)
	{
		yield return new WaitForSeconds(length);

		StopCutScene();
	}
}
