using UnityEngine;
using System.Collections;

public enum CutSceneNotification
{
	CutSceneStarted, CutSceneFinished
};

public class CutSceneController : MonoBehaviour 
{
	DialogueDisplayer dialogueDisplayer;
	CutsceneCamera cutsceneCamera;

	void Awake()
	{
	}

	void Start()
	{
		dialogueDisplayer = (DialogueDisplayer)FindObjectOfType(typeof(DialogueDisplayer));
		cutsceneCamera = GameObject.Find("CutsceneCamera").GetComponent<CutsceneCamera>();
	}

	void TriggererEntered(CutSceneObj cutSceneObj)
	{

		StartCoroutine(CutSceneTimerRoutine(cutSceneObj.lengthInSeconds, cutSceneObj));

		DisplayCutscene(cutSceneObj);
	}

	public void DisplayCutscene(CutSceneObj cutSceneObj)
	{
		Messenger.Invoke(CutSceneNotification.CutSceneStarted.ToString());

		if(cutSceneObj.dialogue.dialogueAsset != null)
		{
			//Calculate chars per second given amount of chars in each line and the length of this cutscene

			dialogueDisplayer.StartCoroutine(dialogueDisplayer.DisplayText(cutSceneObj.dialogue.dialogueLines));
		}
		if(cutSceneObj.audioClip != null)
		{
			//play audioclip.
			audio.clip = cutSceneObj.audioClip;
			audio.Play();
		}
		if(cutSceneObj.cameraAnimation != null)
		{
			cutsceneCamera.PlayAnimation(cutSceneObj.cameraAnimation);
		}
	}

	public void StopCutScene()
	{
		if(dialogueDisplayer != null)
			dialogueDisplayer.StopDisplayingText();
		audio.Stop();
		cutsceneCamera.FinishAnimation();

		Messenger.Invoke(CutSceneNotification.CutSceneStarted.ToString());
	}
	
	IEnumerator CutSceneTimerRoutine(float length, CutSceneObj cutsceneObj)
	{
		yield return new WaitForSeconds(length);
		StopCutScene();
		cutsceneObj.UnloadAudio();
		cutsceneObj.dialogue.UnloadDialogue();
		cutsceneObj.UnloadCameraAnim();
	}
}
