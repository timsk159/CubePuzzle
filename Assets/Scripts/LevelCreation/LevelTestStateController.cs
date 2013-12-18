using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelTestStateController : MonoSingleton<LevelTestStateController> 
{
	byte[] checkpoint;
	ColourAndPosition playerObjSave;

	public void SetCheckpoint()
	{
		var mapRoot = GameObject.Find("MapRoot");
		var player = GameObject.FindGameObjectWithTag("Player");

		checkpoint = LevelSerializer.SaveObjectTree(mapRoot);
		playerObjSave = new ColourAndPosition();
		playerObjSave.colour = player.GetComponent<PlayerCharacter>().currentColor;
		playerObjSave.pos = player.transform.position;
	}

	public void LoadCheckpoint()
	{
		var oldMapRoot = GameObject.Find("MapRoot");
		var player = GameObject.FindGameObjectWithTag("Player");
		LevelSerializer.LoadObjectTree(checkpoint, delegate
		{
			StartCoroutine(SetupMap(player));
		});
	}

	IEnumerator SetupMap(GameObject player)
	{	
		DestroyOldCombinedMeshes();
		player.transform.position = playerObjSave.pos;
		LevelController.Instance.OptimiseLevelMesh();
		yield return new WaitForEndOfFrame();
		player.GetComponent<PlayerCharacter>().ChangeColour(playerObjSave.colour);
		yield return new WaitForEndOfFrame();
		InitMapObjects();
	}

	void InitMapObjects()
	{
		var colourObjs = (ColorCollisionObject[])FindObjectsOfType(typeof(ColorCollisionObject));

		foreach(var obj in colourObjs)
		{
			obj.ChangeColour(obj.objColour);
		}
	}

	void DestroyOldCombinedMeshes()
	{
		var meshes = GameObject.FindGameObjectsWithTag("CombinedMesh");

		foreach(var mesh in meshes)
		{
			Destroy(mesh.gameObject);
		}
	}

	/*
	private List<GameObject> checkpointMapObjects;
	private GameObject checkpointPlayerObject;


	public void SetCheckpoint()
	{
	
		checkpointMapObjects = new List<GameObject>();

		var mapRoot = GameObject.Find("MapRoot");

		checkpointPlayerObject = GameObject.FindGameObjectWithTag("Player");

		foreach(Transform child in mapRoot.transform)
		{
			checkpointMapObjects.Add(child.gameObject);
		}
	}

	public void LoadCheckpoint()
	{
		var mapRoot = GameObject.Find("MapRoot");
		var player = GameObject.FindGameObjectWithTag("Player");

		foreach(Transform child in mapRoot.transform)
		{
			Destroy(child.gameObject);
		}
		Destroy(player.gameObject);

		foreach(var obj in checkpointMapObjects)
		{
			var createdObj = (GameObject)Instantiate(obj);
			createdObj.transform.parent = mapRoot.transform;
		}
		Instantiate(player);
	}
	*/
}


public class ColourAndPosition
{
	public Vector3 pos;
	public Colour colour;
}