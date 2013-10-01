using UnityEngine;
using System.Collections;

public class HolePiece : ColorCollisionObject
{
	void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Player")
		{
			NotificationCenter<ColourCollisionNotification>.DefaultCenter.PostNotification(ColourCollisionNotification.PlayerKilled, null);
		}
	}
}
