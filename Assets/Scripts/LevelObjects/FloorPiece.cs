using UnityEngine;
using System.Collections;

using StateM = StateMachine<LevelState, LevelStateMessage>;

public class FloorPiece : ColorCollisionObject 
{
	GameObject sharedMeshForThisPiece;

	protected override void Start()
	{
		base.Start ();

		Messenger<Colour>.AddListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Messenger<Colour>.RemoveListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);
	}

	protected void PlayerChangedColour(Colour colourToChangeTo)
	{
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

	protected override void LevelInitialized(StateM.StateChangeData changeData)
	{
		FindSharedMesh();

		base.LevelInitialized(changeData);
	}

	protected override void LevelStarted(StateM.StateChangeData changeData)
	{
		FindSharedMesh();

		base.LevelStarted(changeData);
	}

	void MakePassable()
	{
		FindSharedMesh();

		if(collider != null)
		{
			var thisCollider = collider as BoxCollider;
			var newColliderSize = new Vector3(thisCollider.size.x, initialColliderSize.y, thisCollider.size.z);

			thisCollider.size = newColliderSize;
		}

		if(sharedMeshForThisPiece != null && iTween.Count(sharedMeshForThisPiece) < 1)
		{
			//iTween.ScaleTo(sharedMeshForThisPiece, Vector3.one, 0.5f);
			iTween.MoveTo(sharedMeshForThisPiece, Vector3.zero, 0.5f);
		}
	}

	void MakeImpassable()
	{
		FindSharedMesh();

		if(collider != null)
		{
			var thisCollider = collider as BoxCollider;
			var newColliderSize = new Vector3(thisCollider.size.x, 10, thisCollider.size.z);

			thisCollider.size = newColliderSize;
		}
		if (sharedMeshForThisPiece != null && iTween.Count(sharedMeshForThisPiece) < 1)
		{
			//iTween.ScaleTo(sharedMeshForThisPiece, newSize, 0.5f);
			iTween.MoveTo(sharedMeshForThisPiece, new Vector3(0, 0.3f, 0), 0.5f);
		}
	}

	protected override void OnDeserialized()
	{
		FindSharedMesh();
	}

	void FindSharedMesh()
	{
		if (sharedMeshForThisPiece == null)
		{
			sharedMeshForThisPiece = GameObject.Find("CombinedMesh: " + renderer.sharedMaterial.name.Replace("(Instance)", ""));
		}
	}
}
