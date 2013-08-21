// NotificationCenter is used for handling messages between GameObjects.
// GameObjects can register to receive specific notifications. When another objects sends a notification of that type, all GameObjects that registered for it and implement the appropriate message will receive that notification.
// Observing GameObjetcs must register to receive notifications with the AddObserver function, and pass their selves, and the name of the notification. Observing GameObjects can also unregister themselves with the RemoveObserver function. GameObjects must request to receive and remove notification types on a type by type basis.
// Posting notifications is done by creating a Notification object and passing it to PostNotification. All receiving GameObjects will accept that Notification object. The Notification object contains the sender, the notification type name, and an option hashtable containing data.
// To use NotificationCenter, either create and manage a unique instance of it somewhere, or use the static NotificationCenter.
 
 
// We need a static method for objects to be able to obtain the default notification center.
// This default center is what all objects will use for most notifications. We can of course create our own separate instances of NotificationCenter, but this is the static one used by all.
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class NotificationCenter<T>
{
	private static NotificationCenter<T> _defaultCenter;
	public static NotificationCenter<T> DefaultCenter
	{
		get
		{
			if(_defaultCenter == null)
			{
				_defaultCenter = new NotificationCenter<T>();
			}
			return _defaultCenter;
		}
	}
	
	public struct Notification
	{
		public T type;
		public object data;
		
		public Notification(T _type, object _data)
		{
			type = _type;
			data = _data;
		}
		
		public Notification(T _type)
		{
			type = _type;
			data = null;
		}
	}
	
	// Our hashtable containing all the notifications. Each notification in the hash table is an ArrayList that contains all the observers for that notification.
	Hashtable notifications = new Hashtable();
	
	/// <summary>
	/// Adds an observer to the specified notification type. Used to subscribe to events.
	/// </summary>
	/// <param name='_observer'>
	/// Component that wants to subscribe (usually 'this')
	/// </param>
	/// <param name='_type'>
	/// The type of event you wish to subscribe to.
	/// </param>
	public void AddObserver (Component _observer, T _type)
	{
		var name = _type.ToString();
		// If the name isn't good, then throw an error and return.
		if (string.IsNullOrEmpty (name))
		{
			Debug.Log ("Null name specified for notification in AddObserver.");
			return;
		}
		
		// If this specific notification doesn't exist yet, then create it.
		if (notifications[name] == null)
		{
			notifications[name] = new List<Component>();
		}
		
		List<Component> notifyList = notifications[name] as List<Component>;
		
		// If the list of observers doesn't already contain the one that's registering, the add it.
		if (!notifyList.Contains(_observer))
		{
			notifyList.Add(_observer);
		}
	}
	
	
	
	public void RemoveObserver (Component observer, T _type)
	{
		List<Component> notifyList = (List<Component>)notifications[_type.ToString()];
		
		// Assuming that this is a valid notification type, remove the observer from the list.
		// If the list of observers is now empty, then remove that notification type from the notifications hash. This is for housekeeping purposes.
		if (notifyList != null)
		{
			if (notifyList.Contains(observer))
			{
				notifyList.Remove(observer);
			}
			if (notifyList.Count == 0)
			{
				notifications.Remove(_type.ToString());
			}
		}
	}
	
	/// <summary>
	/// Posts the notification.
	/// </summary>
	/// <param name='_type'>
	/// The notification type.
	/// </param>
	/// <param name='_data'>
	/// The data to be sent with this notification
	/// </param>
	public void PostNotification (T _type, object _data)
	{
		PostNotification(new Notification(_type, _data));
	}
	
	/// <summary>
	/// Posts the notification.
	/// </summary>
	/// <param name='_Notification'>
	/// The notification
	/// </param>
	public void PostNotification (Notification _Notification)
	{
		// First make sure that the name of the notification is valid.
		//Debug.Log("sender: " + aNotification.name);
		if (string.IsNullOrEmpty (_Notification.type.ToString()))
		{
			Debug.Log ("Null name sent to PostNotification.");
			return;
		}
		
		List<Component> observerListForNotification = notifications[_Notification.type.ToString()] as List<Component>;
		if (observerListForNotification != null)
		{
			List<Component> observerList = new List<Component>();
			foreach (Component observer in observerListForNotification)
			{
				observerList.Add(observer);
			}
		
		if (observerList != null)
		{
			// Create an array to keep track of invalid observers that we need to remove
			List<Component> observersToRemove = new List<Component> ();
			
			// Itterate through all the objects that have signed up to be notified by this type of notification.
			foreach (Component observer in observerList)
			{
				// If the observer isn't valid, then keep track of it so we can remove it later.
				// We can't remove it right now, or it will mess the for loop up.
				if (!observer)
				{
					observersToRemove.Add(observer);
				}
				else
				{
					// If the observer is valid, then send it the notification. The message that's sent is the name of the notification.
					observer.SendMessage(_Notification.type.ToString(), _Notification, SendMessageOptions.DontRequireReceiver);
				}
			}
				
			// Remove all the invalid observers
			foreach (Component invalidObserver in observersToRemove)
			{
				//observersToRemove.Remove(invalidObserver);
				RemoveObserver(invalidObserver, _Notification.type);
			}
			}
		}
	}
}