using UnityEngine;
using System.Collections;

public class Triggerer : MonoBehaviour 
{
	public GameObject[] listeners;	
	
	void OnTriggerEnter(Collider col)
	{
		foreach(var listener in listeners)
		{
			listener.SendMessage("TriggererEntered", col.gameObject, SendMessageOptions.RequireReceiver);
		}
	}
	
	void OnTriggerExit(Collider col)
	{
		foreach(var listener in listeners)
		{
			listener.SendMessage("TriggererExited", col.gameObject, SendMessageOptions.RequireReceiver);
		}
	}
}
