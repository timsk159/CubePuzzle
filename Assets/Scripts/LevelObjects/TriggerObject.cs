using UnityEngine;
using System.Collections;

public class TriggerObject : ColorCollisionObject 
{
	public virtual void PlayerInteracted()
	{
		
	}
	
	protected virtual void TriggererEntered(GameObject go)
	{
		Messenger<GameObject>.Invoke(ColourCollisionMessage.TriggerEntered.ToString(), gameObject);
	}
	
	protected virtual void TriggererExited(GameObject go)
	{
		Messenger<GameObject>.Invoke(ColourCollisionMessage.TriggerExited.ToString(), gameObject);
	}
}
