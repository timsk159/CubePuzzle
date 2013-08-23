using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ColourNotiCenter = NotificationCenter<ColourCollisionNotification>;

public enum ColourCollisionNotification
{
	PlayerEnteredColour, PlayerExitedColour, ObjectEnteredColour, ObjectExitedColour,
	LeverPulled, ButtonPressed, InteractionTriggerEnter, InteractionTriggerExit,
	PlayerChangedColour, FloorPiecesChangedColour
};

public class ColorCollisionObject : MonoBehaviour 
{
	public Colour objColour;
	public Vector3 initialColliderSize;
	[DoNotSerialize()]
	public CubeNeighbours cubeNeighbours;
	
	void Awake()
	{
		NotificationCenter<ColourCollisionNotification>.DefaultCenter.AddObserver(this, ColourCollisionNotification.ButtonPressed);
		cubeNeighbours = new CubeNeighbours(gameObject);
	}
	
	protected virtual void Start()
	{
		var col = collider as BoxCollider;
		initialColliderSize = col.size;
		renderer.material.color = GetObjectRealColor(objColour);
		if(Application.loadedLevelName != "LevelCreator")
			EnsureCollidersAreEnabled();
	}

	void EnsureCollidersAreEnabled()
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
			gameObject.renderer.material.color = GetObjectRealColor(objColour);
		}
		
	}
	
	public virtual void RotateColour(bool forward)
	{
		int currentColourIndex = (int)objColour;
		var values = Enum.GetValues(typeof(Colour));
		
		if(forward)
		{
			currentColourIndex++;
			
			if(currentColourIndex == values.Length)
			{
				currentColourIndex = 1;
			}
			
			ChangeColour((Colour)currentColourIndex);
		}
		else
		{
			currentColourIndex--;
			
			if(currentColourIndex == 1)
			{
				currentColourIndex = (int)values.GetValue(values.Length);
			}
			
			ChangeColour((Colour)currentColourIndex);
		}
	}
	
	public Color GetObjectRealColor(Colour objectsColour)
	{
		switch(objectsColour)
		{
			case Colour.Red:
				return Color.red;
			case Colour.Green:
				return Color.green;
			case Colour.Blue:
				return Color.blue;
			default:
				return Color.white;
		}
	}
	
	public void ButtonPressed(NotificationCenter<ColourCollisionNotification>.Notification notiData)
	{
		var forward = (bool)notiData.data;
		
		RotateColour(forward);
	}
}

public class CubeNeighbours
{
	GameObject gameObject;
	Transform transform;

	public enum NeighbourDirection
	{
		None, Forward, Back, Right, Left
	};

	Vector3[] directions = new Vector3[4];

	GameObject[] _neighbours;

	public CubeNeighbours(GameObject gameObject)
	{
		this.gameObject = gameObject;
		this.transform = gameObject.transform;

		directions[0] = transform.forward;
		directions[1] = -transform.forward;
		directions[2] = transform.right;
		directions[3] = -transform.right;
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