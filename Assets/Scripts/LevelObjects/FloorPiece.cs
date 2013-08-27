using UnityEngine;
using System.Collections;
using ColourNotiCenter = NotificationCenter<ColourCollisionNotification>;

public class FloorPiece : ColorCollisionObject 
{
	protected override void Start()
	{
		base.Start ();
		
		ColourNotiCenter.DefaultCenter.AddObserver(this, ColourCollisionNotification.PlayerChangedColour);
	}
	
	protected void PlayerChangedColour(NotificationCenter<ColourCollisionNotification>.Notification notiData)
	{
		var colourToChangeTo = (Colour)notiData.data;
		
		if(colourToChangeTo == objColour)
		{
			var thisCollider = collider as BoxCollider;
			var newColliderSize = new Vector3(thisCollider.size.x, 10, thisCollider.size.z);
			
			thisCollider.size = newColliderSize;
		}
		else
		{			
			var thisCollider = collider as BoxCollider;
			var newColliderSize = new Vector3(thisCollider.size.x, initialColliderSize.y, thisCollider.size.z);
			
			thisCollider.size = newColliderSize;
		}
	}
	
	public override void ChangeColour(Colour colorToChangeTo)
	{
		base.ChangeColour (colorToChangeTo);

		if(LevelController.Instance != null)
		{
			if(LevelController.Instance.PlayerColour == objColour)
			{
				var thisCollider = collider as BoxCollider;
				var newColliderSize = new Vector3(thisCollider.size.x, 10, thisCollider.size.z);
				
				thisCollider.size = newColliderSize;
			}
			else
			{			
				var thisCollider = collider as BoxCollider;
				var newColliderSize = new Vector3(thisCollider.size.x, initialColliderSize.y, thisCollider.size.z);
				
				thisCollider.size = newColliderSize;
			}
		}

	}
}
