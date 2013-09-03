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

	void TriggererEntered(CutsceneObj cutSceneObj)
	{
		NotificationCenter<CutSceneNotification>.DefaultCenter.PostNotification(CutSceneNotification.CutSceneStarted, null);

		StartCoroutine(CutSceneTimerRoutine(cutSceneObj.lengthInSeconds));

		//Do cutsceney stuff here.
	}

	void DisplayCutscene(CutsceneObj cutsceneObj)
	{

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
