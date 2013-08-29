using UnityEngine;
using System.Collections;

public class CutSceneTrigger : Triggerer 
{
	public CutSceneObj cutSceneObj;
	public bool hasPlayed ;

	protected override void OnTriggerEnter(Collider col)
	{
		if(!hasPlayed)
		{
			foreach(var listener in listeners)
			{
				listener.SendMessage("TriggererEntered", cutSceneObj, SendMessageOptions.RequireReceiver);
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
