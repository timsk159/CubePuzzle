using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class Triggerer : MonoBehaviour 
{
	public GameObject[] listeners;	
	
	protected virtual void OnTriggerEnter(Collider col)
	{
		foreach(var listener in listeners)
		{
			listener.SendMessage("TriggererEntered", col.gameObject, SendMessageOptions.RequireReceiver);
		}
	}
	
	protected virtual void OnTriggerExit(Collider col)
	{
		foreach(var listener in listeners)
		{
			listener.SendMessage("TriggererExited", col.gameObject, SendMessageOptions.RequireReceiver);
		}
	}
}
