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

		StartCoroutine(CutSceneTimerRoutine(cutSceneObj.timeInSeconds));

		//Do cutsceney stuff here.

		if(cutSceneObj is SimpleCutSceneObj)
		{
			var simepleCutSceneObj = cutSceneObj as SimpleCutSceneObj;

			DisplaySimpleCuscene(simepleCutSceneObj);
		}
		else if(cutSceneObj is ComplexCutsceneObj)
		{
			var complexCutsceneObj = cutSceneObj as ComplexCutsceneObj;

			DisplayComplexCutscene(complexCutsceneObj);
		}
		if(cutSceneObj.dialogue != null)
		{
			cutSceneLabel.StartCoroutine(cutSceneLabel.DisplayText(cutSceneObj.dialogueLines));
		}
	}

	void DisplaySimpleCuscene(SimpleCutSceneObj simpleCutSceneObj)
	{
		if(simpleCutSceneObj.cameraAnimation != null)
		{
			cutSceneCamera.depth = 1;

			if(cutSceneCamera.animation.GetClip(simpleCutSceneObj.cameraAnimation.name) == null)
			{
				cutSceneCamera.animation.AddClip(simpleCutSceneObj.cameraAnimation, simpleCutSceneObj.cameraAnimation.name);
			}
			cutSceneCamera.animation.Play(simpleCutSceneObj.cameraAnimation.name);
		}
		if(simpleCutSceneObj.narrationAudio != null)
		{
			audio.clip = simpleCutSceneObj.narrationAudio;
			audio.Play();
		}
	}

	void DisplayComplexCutscene(ComplexCutsceneObj complexCutsceneObj)
	{
		var go = (GameObject)Instantiate(complexCutsceneObj.prefabToInstantiate);
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
