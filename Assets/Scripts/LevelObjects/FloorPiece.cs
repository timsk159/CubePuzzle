using UnityEngine;
using System.Collections;
using ColourNotiCenter = NotificationCenter<ColourCollisionNotification>;

public class FloorPiece : ColorCollisionObject 
{
	GameObject sharedMeshForThisPiece;

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
			MakeImpassable();
		}
		else
		{			
			MakePassable();
		}
	}
	
	public override void ChangeColour(Colour colorToChangeTo)
	{
		base.ChangeColour (colorToChangeTo);

		if(LevelController.Instance != null)
		{
			if(LevelController.Instance.PlayerColour == objColour)
			{
				MakeImpassable();
			}
			else
			{			
				MakePassable();
			}
		}
	}

	protected override void LevelInitialized()
	{
		if(sharedMeshForThisPiece == null)
		{
			sharedMeshForThisPiece = GameObject.Find("CombinedMesh: " + renderer.sharedMaterial.name.Replace("(Instance)", ""));
		}
		base.LevelInitialized();
	}

	protected override void LevelStarted()
	{
		if(sharedMeshForThisPiece == null)
		{
			sharedMeshForThisPiece = GameObject.Find("CombinedMesh: " + renderer.sharedMaterial.name.Replace("(Instance)", ""));
		}
		base.LevelStarted();
	}

	void MakePassable()
	{
		if(collider != null)
		{
			var thisCollider = collider as BoxCollider;
			var newColliderSize = new Vector3(thisCollider.size.x, initialColliderSize.y, thisCollider.size.z);

			thisCollider.size = newColliderSize;
		}

		if(sharedMeshForThisPiece != null)
		{
			iTween.ScaleTo(sharedMeshForThisPiece, Vector3.one, 0.5f);
		}
		else
		{
			iTween.ScaleTo(gameObject, Vector3.one, 0.5f);
		}
	}

	void MakeImpassable()
	{
		if(collider != null)
		{
			var thisCollider = collider as BoxCollider;
			var newColliderSize = new Vector3(thisCollider.size.x, 10, thisCollider.size.z);

			thisCollider.size = newColliderSize;
		}

		var newSize = transform.localScale;

		newSize.y += (newSize.y * 25f / 100f);

		if(sharedMeshForThisPiece != null)
		{
			iTween.ScaleTo(sharedMeshForThisPiece, newSize, 0.5f);
		}
		else
		{
			iTween.ScaleTo(gameObject, newSize, 0.5f);
		}
	}
}
