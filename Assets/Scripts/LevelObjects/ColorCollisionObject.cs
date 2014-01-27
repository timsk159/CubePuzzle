using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using StateM = StateMachine<LevelState, LevelStateMessage>;

public enum ColourCollisionMessage
{
	ObjectEnteredColour, ObjectExitedColour,
	LeverPulled, ButtonPressed, TriggerEntered, TriggerExited,
	PlayerChangedColour, FloorPiecesChangedColour, PlayerKilled
};

public class ColorCollisionObject : MonoBehaviour 
{
	public static bool hasSentColourChangeMessage;

	protected static PhysicMaterial impassablePMat;
	protected static PhysicMaterial passablePMat;

	public Colour initialColour;
	public Colour objColour;

	public Collider impassableCollider;

	[DoNotSerialize()]
	public CubeNeighbours cubeNeighbours;
	public bool meshCanBeOptimized;

	protected bool useSharedMaterial = true;

	void Awake()
	{
#if DEBUG_CUBES
		if(impassableCollider == null)
			Debug.LogWarning("Cube: " + gameObject.name + " Has no impassable collider");
#endif
		if(impassablePMat == null)
			impassablePMat = (PhysicMaterial)Resources.Load("ImpassablePMat");
		if(passablePMat == null)
			passablePMat = (PhysicMaterial)Resources.Load("PassablePMat");

		Messenger.AddListener(ColourCollisionMessage.ButtonPressed.ToString(), ButtonPressed);
		cubeNeighbours = new CubeNeighbours(gameObject);
		initialColour = objColour;
	}

	protected virtual void Start()
	{
		Messenger.AddListener(LevelStateMessage.LevelInitialized.ToString(), LevelInitialized);
		Messenger.AddListener(LevelStateMessage.LevelStarted.ToString(), LevelStarted);
	}

	protected virtual void OnDestroy()
	{
		Messenger.RemoveListener(LevelStateMessage.LevelInitialized.ToString(), LevelInitialized);
		Messenger.RemoveListener(LevelStateMessage.LevelStarted.ToString(), LevelStarted);
		Messenger.RemoveListener(ColourCollisionMessage.ButtonPressed.ToString(), ButtonPressed);
	}

	protected virtual void OnDisable()
	{
		if(useSharedMaterial && renderer != null)
		{
			this.renderer.sharedMaterial.color = ColorManager.GetObjectRealColor(initialColour);
		}
	}

	protected virtual void OnDeserialized()
	{
		if(Application.loadedLevelName != "LevelCreator")
			EnsureCollidersAreEnabled();
	}

	protected virtual void LevelInitialized()
	{
	}

	protected virtual void LevelStarted()
	{
		EnsureCollidersAreEnabled();
		ChangeColour(objColour);
	}

	public void EnsureCollidersAreEnabled()
	{
		if(collider)
		{
			collider.enabled = true;
		}
		if(transform.childCount > 0)
		{
			foreach(Transform child in transform)
			{
				if(child.collider)
					child.collider.enabled = true;
			}
		}
	}
	
	protected virtual void OnCollisionEnter(Collision col)
	{

	}
	
	protected virtual void OnCollisionExit(Collision col)
	{

	}
	
	public virtual void ChangeColour(Colour colorToChangeTo)
	{
		if(!hasSentColourChangeMessage)
		{
			Messenger<Colour>.Invoke(ColourCollisionMessage.FloorPiecesChangedColour.ToString(), colorToChangeTo);
			hasSentColourChangeMessage = true;
		}
		if(objColour != Colour.None)
		{
			objColour = colorToChangeTo;
			if(useSharedMaterial)
			{
				gameObject.renderer.sharedMaterial.color = ColorManager.GetObjectRealColor(objColour);
			}
			else
			{
				gameObject.renderer.material.color = ColorManager.GetObjectRealColor(objColour);
			}
		}
		
	}
	
	public virtual void RotateColour()
	{
		int currentColourIndex = (int)objColour;

		currentColourIndex++;
		
		if(currentColourIndex == ColorManager.cachedColourValues.Length)
		{
			currentColourIndex = 1;
		}
		
		ChangeColour((Colour)currentColourIndex);
	}

	public void ButtonPressed()
	{
		RotateColour();
	}
}

public class CubeNeighbours
{
	Transform transform;

	public enum NeighbourDirection
	{
		None, Forward, Back, Right, Left
	};

	Vector3[] directions = new Vector3[4];

	GameObject[] _neighbours;

	public CubeNeighbours(GameObject go)
	{
		this.transform = go.transform;

		/*
		directions[0] = transform.forward;
		directions[1] = -transform.forward;
		directions[2] = transform.right;
		directions[3] = -transform.right;
		*/
		directions[0] = Vector3.forward;
		directions[1] = -Vector3.forward;
		directions[2] = Vector3.right;
		directions[3] = -Vector3.right;
	}

	public GameObject[] neighbours
	{
		get{return _neighbours;}
		private set{ _neighbours = value;}
	}

	public GameObject[] cachedNeighbours
	{
		get
		{
			if(_neighbours.Length == 0)
				return null;
			else
				return _neighbours;
		}
	}

	public NeighbourDirection[] GetMissingNeighbours()
	{
		List<NeighbourDirection> neighbourList = new List<NeighbourDirection>(4);

		for(int i = 0; i < directions.Length; i++)
		{
			Ray ray = new Ray(transform.position, directions[i]);

			if(Physics.Raycast(ray, 0.6f, 1 << 10))
			{
				continue;
			}
			else
			{
				neighbourList.Add((NeighbourDirection)(i + 1));
			}
		}

		return neighbourList.ToArray();
	}
}

public enum Colour
{
	None, Red, Green, Blue
};	