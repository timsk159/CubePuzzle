using UnityEngine;
using System.Collections;

public class CutSceneTrigger : Triggerer 
{
	public CutsceneObj cutSceneObj;

	public bool hasPlayed ;

	protected override void OnTriggerEnter(Collider col)
	{
		if(!hasPlayed)
		{
			foreach(var listener in listeners)
			{
				listener.SendMessage("TriggererEntered", cutSceneObj, SendMessageOptions.RequireReceiver);
				hasPlayed = true;
			}
		}
	}

	protected override void OnTriggerExit(Collider col)
	{
		if(!hasPlayed)
		{
			foreach(var listener in listeners)
			{
				listener.SendMessage("TriggererExited", cutSceneObj, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
