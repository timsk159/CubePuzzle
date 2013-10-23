using UnityEngine;
using System.Collections;

public enum CutSceneNotification
{
	CutSceneStarted, CutSceneFinished
};

public class CutSceneController : MonoBehaviour 
{
	public DialogueDisplayer dialogueDisplayer;

	void TriggererEntered(CutsceneObj cutSceneObj)
	{
		NotificationCenter<CutSceneNotification>.DefaultCenter.PostNotification(CutSceneNotification.CutSceneStarted, null);

		StartCoroutine(CutSceneTimerRoutine(cutSceneObj.lengthInSeconds));

		DisplayCutscene(cutSceneObj);
	}

	void DisplayCutscene(CutsceneObj cutSceneObj)
	{

		dialogueDisplayer.StartCoroutine(dialogueDisplayer.DisplayText(cutSceneObj.dialogue.dialogueLines));
	}

	void StopCutScene()
	{
		dialogueDisplayer.StopDisplayingText();
		NotificationCenter<CutSceneNotification>.DefaultCenter.PostNotification(CutSceneNotification.CutSceneFinished, null);
	}
	
	IEnumerator CutSceneTimerRoutine(float length)
	{
		yield return new WaitForSeconds(length);

		StopCutScene();
	}
}
