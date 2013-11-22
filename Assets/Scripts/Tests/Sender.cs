using UnityEngine;
using System.Collections;
using System.Diagnostics;

public enum EventNotification
{
	OnEvent
};

public class Sender : MonoBehaviour 
{
	public int testAmount = 10000;

	public delegate void EventDelegate();

	public static EventDelegate onEvent;

	void OnGUI()
	{
		if(GUILayout.Button("Create Recievers"))
		{
			CreateRecievers();
		}
		if(GUILayout.Button("Send Notifications"))
		{
			SendNotifications();
		}
		if(GUILayout.Button("Fire Delegates"))
		{
			FireDelegates();
		}
	}

	void CreateRecievers()
	{
		for(int i = 0; i < testAmount; i++)
		{
			var go = new GameObject("Reciever" + i);
			go.AddComponent<Reciever>();
			go.transform.position = Vector3.zero;
		}
	}

	void SendNotifications()
	{
		var sw = new Stopwatch();
		sw.Start();

		Messenger.Invoke(EventNotification.OnEvent.ToString());

		sw.Stop();
		print("Notifications took: " + sw.ElapsedMilliseconds);
	}

	void FireDelegates()
	{
		var sw = new Stopwatch();
		sw.Start();

		if(onEvent != null)
			onEvent();

		sw.Stop();
		print("Delegates took: " + sw.ElapsedMilliseconds);
	}
}
