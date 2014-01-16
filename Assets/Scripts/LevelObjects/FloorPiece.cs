using UnityEngine;
using System.Collections;

using StateM = StateMachine<LevelState, LevelStateMessage>;

public class FloorPiece : ColorCollisionObject 
{
	SharedMesh sharedMeshForThisPiece;
	bool isMovingUp;
	bool isMovingDown;

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

	protected override void LevelInitialized()
	{
		FindSharedMesh();

		base.LevelInitialized();
	}

	protected override void LevelStarted()
	{
		FindSharedMesh();

		base.LevelStarted();
	}

	void MakePassable()
	{
		if(Application.loadedLevelName == "LevelCreator")
		{
			if(!LevelCreatorController.isTesting)
				return;
		}
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
		else
		{
			MoveDown();
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
		if(Application.loadedLevelName == "LevelCreator")
		{
			if(!LevelCreatorController.isTesting)
				return;
		}
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
		else
		{
			MoveUp();
		}

		/*
		if (sharedMeshForThisPiece != null && iTween.Count(sharedMeshForThisPiece) < 1)
		{
			iTween.MoveTo(sharedMeshForThisPiece, new Vector3(0, 0.3f, 0), 0.5f);
		}
		*/
	}

	void MoveUp()
	{
		if(!isMovingUp)
		{
			ClearActiveTweens();
			print("Moving mesh: " + gameObject.name + " Up. PLayers colour is: " + LevelController.Instance.PlayerColour);
			var movePos = transform.localPosition;
			movePos.y = 0.5f;
			iTween.MoveTo(gameObject, iTween.Hash("position", movePos, "time", 0.5f, "oncomplete", "UpComplete", "islocal", true));
			isMovingUp = true;
		}
	}

	void MoveDown()
	{
		if(!isMovingDown)
		{
			ClearActiveTweens();
			print("Moving mesh: " + gameObject.name + " Down. PLayers colour is: " + LevelController.Instance.PlayerColour);
			var movePos = transform.localPosition;
			movePos.y = 0;
			iTween.MoveTo(gameObject, iTween.Hash("position", movePos, "time", 0.5f, "oncomplete", "DownComplete", "islocal", true));
			isMovingDown = true;
		}
	}

	void ClearActiveTweens()
	{
		if(iTween.Count(gameObject) >= 1)
		{
			iTween.Stop(gameObject);
		}
	}

	void UpComplete()
	{
		isMovingUp = false;
	}

	void DownComplete()
	{
		isMovingDown = false;
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
