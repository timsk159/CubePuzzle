using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using StateM = StateMachine<LevelState, LevelStateNotification>;

public enum ColourCollisionNotification
{
	ObjectEnteredColour, ObjectExitedColour,
	LeverPulled, ButtonPressed, InteractionTriggerEnter, InteractionTriggerExit,
	PlayerChangedColour, FloorPiecesChangedColour, PlayerKilled
};

public class ColorCollisionObject : MonoBehaviour 
{
	public Colour initialColour;
	public Colour objColour;
	[DoNotSerialize()]
	public Vector3 initialColliderSize;
	[DoNotSerialize()]
	public CubeNeighbours cubeNeighbours;
	public bool meshCanBeOptimized; 

	protected bool useSharedMaterial = true;
	
	void Awake()
	{
		Messenger.AddListener(ColourCollisionNotification.ButtonPressed.ToString(), ButtonPressed);
		cubeNeighbours = new CubeNeighbours(gameObject);
		initialColour = objColour;
	}

	protected virtual void Start()
	{
		initialColliderSize = Vector3.one;

		Messenger<StateM.StateChangeData>.AddListener(LevelStateNotification.LevelInitialized.ToString(), LevelInitialized);
		Messenger<StateM.StateChangeData>.AddListener(LevelStateNotification.LevelStarted.ToString(), LevelStarted);
	}

	protected virtual void OnDestroy()
	{
		Messenger<StateM.StateChangeData>.RemoveListener(LevelStateNotification.LevelInitialized.ToString(), LevelInitialized);
		Messenger<StateM.StateChangeData>.RemoveListener(LevelStateNotification.LevelStarted.ToString(), LevelStarted);
		Messenger.RemoveListener(ColourCollisionNotification.ButtonPressed.ToString(), ButtonPressed);
	}

	protected virtual void OnDisable()
	{
		if(useSharedMaterial && renderer != null)
		{
			//Start cube uses neutral cube mat. buttons are yellow.
			if(!gameObject.name.Contains("StartCube"))
			{
				this.renderer.sharedMaterial.color = ColorCollisionObject.GetObjectRealColor(initialColour);
			}
		}
	}

	protected virtual void OnDeserialized()
	{
		if(Application.loadedLevelName != "LevelCreator")
			EnsureCollidersAreEnabled();
	}

	protected virtual void LevelInitialized(StateM.StateChangeData changeData)
	{
	}

	protected virtual void LevelStarted(StateM.StateChangeData changeData)
	{
		EnsureCollidersAreEnabled();
		ChangeColour(objColour);
	}

	public void EnsureCollidersAreEnabled()
	{
		collider.enabled = true;

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
		if(objColour != Colour.None)
		{
			objColour = colorToChangeTo;
			if(useSharedMaterial)
			{
				gameObject.renderer.sharedMaterial.color = ColorCollisionObject.GetObjectRealColor(objColour);
			}
			else
			{
				gameObject.renderer.material.color = ColorCollisionObject.GetObjectRealColor(objColour);
			}
		}
		
	}
	
	public virtual void RotateColour()
	{
		int currentColourIndex = (int)objColour;
		var values = Enum.GetValues(typeof(Colour));

		currentColourIndex++;
		
		if(currentColourIndex == values.Length)
		{
			currentColourIndex = 1;
		}
		
		ChangeColour((Colour)currentColourIndex);
	}
	
	public static Color GetObjectRealColor(Colour objectsColour)
	{
		switch(objectsColour)
		{
			case Colour.Red:
				return Color.red;
			case Colour.Green:
				return Color.green;
			case Colour.Blue:
				return Color.blue;
			case Colour.None:
				return Color.white;
			default:
				return Color.white;
		}
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

			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 0.6f, 1 << 10))
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



public struct ColourCollisionData
{
	public GameObject go;
	public Colour objColour;
	
	public ColourCollisionData(GameObject go, Colour objColour)
	{
		this.go = go;
		this.objColour = objColour;
	}
}

public enum Colour
{
	None, Red, Green, Blue
};