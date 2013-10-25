using UnityEngine;
using System.Collections;

public enum CutSceneNotification
{
	CutSceneStarted, CutSceneFinished
};

public class CutSceneController : MonoBehaviour 
{
	public DialogueDisplayer dialogueDisplayer;

	void TriggererEntered(CutSceneObj cutSceneObj)
	{
		NotificationCenter<CutSceneNotification>.DefaultCenter.PostNotification(CutSceneNotification.CutSceneStarted, null);

		StartCoroutine(CutSceneTimerRoutine(cutSceneObj.lengthInSeconds, cutSceneObj));

		DisplayCutscene(cutSceneObj);
	}

	void DisplayCutscene(CutSceneObj cutSceneObj)
	{
		if(cutSceneObj.dialogue.dialogueAsset != null)
		{
			//Calculate chars per second given amount of chars in each line and the length of this cutscene

			StartCoroutine(dialogueDisplayer.DisplayText(cutSceneObj.dialogue.dialogueLines));
		}
		if(cutSceneObj.audioClip != null)
		{
			//play audioclip.
			audio.clip = cutSceneObj.audioClip;
			audio.Play();
		}
	}

	void StopCutScene()
	{
		dialogueDisplayer.StopDisplayingText();

		NotificationCenter<CutSceneNotification>.DefaultCenter.PostNotification(CutSceneNotification.CutSceneFinished, null);
	}
	
	IEnumerator CutSceneTimerRoutine(float length, CutSceneObj cutsceneObj)
	{
		yield return new WaitForSeconds(length);
		StopCutScene();
		cutsceneObj.UnloadAudio();
		cutsceneObj.dialogue.UnloadDialogue();
	}
}
