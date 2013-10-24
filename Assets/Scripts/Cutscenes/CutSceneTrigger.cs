using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class CutSceneTrigger : Triggerer 
{
	public CutSceneObj cutSceneObj;

	public bool hasPlayed ;

	void Start()
	{
		base.listeners = new GameObject[1];
		var cutsceneController = (CutSceneController)FindObjectOfType(typeof(CutSceneController));
		base.listeners[0] = cutsceneController.gameObject;
	}

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
