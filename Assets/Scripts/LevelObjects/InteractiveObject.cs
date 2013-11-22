using UnityEngine;
using System.Collections;

public class InteractiveObject : ColorCollisionObject 
{
	public virtual void PlayerInteracted()
	{
		
	}
	
	protected virtual void TriggererEntered(GameObject go)
	{
		Messenger<GameObject>.Invoke(ColourCollisionNotification.InteractionTriggerEnter.ToString(), gameObject);
	}
	
	protected virtual void TriggererExited(GameObject go)
	{
		Messenger<GameObject>.Invoke(ColourCollisionNotification.InteractionTriggerExit.ToString(), gameObject);
	}
}
