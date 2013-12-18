using UnityEngine;
using System.Collections;

using StateM = StateMachine<LevelState, LevelStateMessage>;

public class FloorPiece : ColorCollisionObject 
{
	SharedMesh sharedMeshForThisPiece;

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
		if(enabled && gameObject.activeInHierarchy && gameObject.activeSelf)
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
	}
	
	public override void ChangeColour(Colour colorToChangeTo)
	{
		if(enabled && gameObject.activeInHierarchy && gameObject.activeSelf)
		{
			base.ChangeColour(colorToChangeTo);

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

		if(collider != null)
		{
			var thisCollider = collider as BoxCollider;
			var newColliderSize = new Vector3(thisCollider.size.x, initialColliderSize.y, thisCollider.size.z);

			thisCollider.size = newColliderSize;
			thisCollider.sharedMaterial = passablePMat;
		}

		if(useSharedMaterial)
		{
			FindSharedMesh();

			if(sharedMeshForThisPiece.IsUp)
			{
				sharedMeshForThisPiece.MoveDown();
			}
		}

		/*
		if(sharedMeshForThisPiece != null && iTween.Count(sharedMeshForThisPiece) < 1)
		{
			iTween.MoveTo(sharedMeshForThisPiece, Vector3.zero, 0.5f);
		}
		*/
	}

	void MakeImpassable()
	{
		if(collider != null)
		{
			var thisCollider = collider as BoxCollider;
			var newColliderSize = new Vector3(thisCollider.size.x, 10, thisCollider.size.z);

			thisCollider.size = newColliderSize;
			thisCollider.sharedMaterial = impassablePMat;
		}

		if(useSharedMaterial)
		{
			FindSharedMesh();

			if(!sharedMeshForThisPiece.IsUp)
			{
				sharedMeshForThisPiece.MoveUp();
			}
		}

		/*
		if (sharedMeshForThisPiece != null && iTween.Count(sharedMeshForThisPiece) < 1)
		{
			iTween.MoveTo(sharedMeshForThisPiece, new Vector3(0, 0.3f, 0), 0.5f);
		}
		*/
	}

	void FindSharedMesh()
	{
		if(useSharedMaterial)
		{
			if (sharedMeshForThisPiece == null)
			{
				var meshGO = GameObject.Find("CombinedMesh: " + renderer.sharedMaterial.name.Replace("(Instance)", ""));
				if(meshGO == null)
				{
					Debug.LogError("Could not find shared mesh for piece: " + gameObject.name);
					return;
				}
				sharedMeshForThisPiece = meshGO.GetComponent<SharedMesh>();
				if(sharedMeshForThisPiece == null)
					sharedMeshForThisPiece = meshGO.AddComponent<SharedMesh>();
			}
		}
	}
}
