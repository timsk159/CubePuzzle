using UnityEngine;
using System.Collections;

public class InteractiveObject : ColorCollisionObject 
{
	public virtual void PlayerInteracted()
	{
		
	}
	
	protected virtual void TriggererEntered(GameObject go)
	{
		NotificationCenter<ColourCollisionNotification>.DefaultCenter.PostNotification(ColourCollisionNotification.InteractionTriggerEnter, this);
	}
	
	protected virtual void TriggererExited(GameObject go)
	{
		NotificationCenter<ColourCollisionNotification>.DefaultCenter.PostNotification(ColourCollisionNotification.InteractionTriggerExit, this);		
	}
}
