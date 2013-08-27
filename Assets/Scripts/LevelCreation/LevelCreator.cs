
using UnityEngine;
using JsonFx.Json;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelCreator : MonoBehaviour
{
	LevelCreatorUIController uiController;
	public LevelAssetManager assetManager;
	public GameObject mapRoot;

	public bool isLoading;
	public bool isSaving;

	AsyncOperation async;
	
	void Start ()
	{
		StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.LevelCreationEnter);
		StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.LevelCreationExit);
		StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.FrontMenuEnter);
		assetManager = GetComponent<LevelAssetManager>();
		mapRoot = GameObject.Find("MapRoot");
		uiController = GameObject.Find ("UIController").GetComponent<LevelCreatorUIController> ();
	}

	void LevelCreationEnter()
	{
		mapRoot = GameObject.Find("MapRoot");
	}

	void FrontMenuEnter()
	{
		LevelSerializer.Progress -= LevelSerializeProgress;
		LevelSerializer.Progress += LevelSerializeProgress;

		mapRoot = GameObject.Find("MapRoot");
		Destroy(mapRoot);

		mapRoot = new GameObject("MapRoot");
	}

	void OnDestroy()
	{
		LevelSerializer.Progress -= LevelSerializeProgress;
	}
	
	
	void SetupMap(GameObject[] sceneObjects)
	{
		StartCoroutine(SetupMapFrameWait(sceneObjects));
	}
	
	IEnumerator SetupMapFrameWait(GameObject[] objects)
	{
		//Play some sort of animation here.
		yield return false;
	}	
	
	public void SaveMap(string saveLocation, out string errorMessage)
	{
		errorMessage = "";
		if(mapRoot == null)
			mapRoot = GameObject.Find("MapRoot");

		if(!MapIsComplete(out errorMessage))
			return;

		var playerObj = GameObject.Find("Player(Clone)");
		var currentPlayerObjParent = playerObj.transform.parent;

		playerObj.transform.parent = null;
			
		if(Directory.GetFiles(Application.persistentDataPath + LevelCreatorController.mapFilesFilePath).Any(e => e.Contains(saveLocation)))
		{
			File.Delete(Application.persistentDataPath + saveLocation.Remove(saveLocation.Length - 2, 2));
			File.Delete(Application.persistentDataPath + saveLocation);
		}

		isSaving = true;

		StartCoroutine(SaveMapRoutine(saveLocation, currentPlayerObjParent, playerObj));
	}

	IEnumerator SaveMapRoutine(string saveLocation, Transform playerObjParent, GameObject playerObj)
	{
		//Save creator map:
		LevelSerializer.SaveObjectTreeToFile (saveLocation, mapRoot);

		RemoveUneededNullCubes ();
		CheckEdgeCubeNeighbours();
		yield return new WaitForEndOfFrame ();

		SaveMapForPlayMode(saveLocation);

		playerObj.transform.parent = playerObjParent;
		StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.ChangeState (LevelCreatorStates.LevelCreation);
	}

	void SaveMapForPlayMode(string saveLocation)
	{
		saveLocation = saveLocation.Remove(saveLocation.Length - 2, 2);
		LevelSerializer.SaveObjectTreeToFile (saveLocation, mapRoot);
	}

	void RemoveUneededNullCubes()
	{
		var nullCubes = GameObject.FindGameObjectsWithTag("NullCube");
		List<GameObject> objsToDestroy = new List<GameObject>();

		foreach(var nullCube in nullCubes)
		{
			List<GameObject> neighbours = new List<GameObject>(4);
			Vector3[] directions = new Vector3[] 
			{
				nullCube.transform.forward,
				-nullCube.transform.forward,
				nullCube.transform.right,
				-nullCube.transform.right
			};
			foreach(var direction in directions)
			{
				Ray ray = new Ray(nullCube.transform.position, direction);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit, 0.7f, 1 << 10))
				{
					neighbours.Add(hit.collider.gameObject);
				}
			}
			if(neighbours.Any(e => e.name != "NullCube(Clone)") && neighbours.Any(e => e.name != "NullCube"))
			{
				continue;
			}
			else
			{
				objsToDestroy.Add(nullCube);
			}
		}

		//Free up memory:
		nullCubes = null;
		foreach(var obj in objsToDestroy)
		{
			Destroy(obj.GetComponent<UniqueIdentifier>());
			Destroy(obj.GetComponent<PrefabIdentifier>());
			Destroy(obj.GetComponent<StoreMaterials>());
		}
	}

	public void CheckEdgeCubeNeighbours()
	{
		List<GameObject> allCubes = new List<GameObject>();
		var floors = GameObject.FindGameObjectsWithTag("FloorPiece");
		var buttons = GameObject.FindGameObjectsWithTag("ButtonPiece");
		var doors = GameObject.FindGameObjectsWithTag("DoorPiece");
		allCubes.AddRange(floors);
		allCubes.AddRange(buttons);
		allCubes.AddRange(doors);

		//Free up memory:
		floors = null;
		buttons = null;
		doors = null;

		foreach(var cube in allCubes)
		{
			var missingNeighbours = cube.GetComponent<ColorCollisionObject>().cubeNeighbours.GetMissingNeighbours();
			if(missingNeighbours.Length > 0)
			{
				foreach(var missingNeighbourDirection in missingNeighbours)
				{
					Vector3 positionToSpawnCube = new Vector3();

					switch(missingNeighbourDirection)
					{
						case CubeNeighbours.NeighbourDirection.Forward:
							positionToSpawnCube = cube.transform.position + cube.transform.forward;
							break;
						case CubeNeighbours.NeighbourDirection.Back:
							positionToSpawnCube = cube.transform.position + -cube.transform.forward;
							break;
						case CubeNeighbours.NeighbourDirection.Right:
							positionToSpawnCube = cube.transform.position + cube.transform.right;
							break;
						case CubeNeighbours.NeighbourDirection.Left:
							positionToSpawnCube = cube.transform.position + -cube.transform.right;
							break;
					}

					var newNullCube = (GameObject)Instantiate(assetManager.nullCubePrefab);
					newNullCube.transform.position = positionToSpawnCube;
					newNullCube.collider.enabled = false;
					newNullCube.renderer.enabled = false;
					if(mapRoot == null)
						mapRoot = GameObject.Find("MapRoot");
					newNullCube.transform.parent = mapRoot.transform;
				}
			}
		}
	}

	public bool MapIsComplete(out string errorMessage)
	{
		var playerObj = GameObject.Find("Player(Clone)");
		errorMessage = "";
		if (playerObj == null)
		{
			errorMessage = "No Player start cube was placed!";
			return false;
		}
		if(GameObject.Find("EndGameCube") == null)
		{
			errorMessage = "No End cube was placed!";
			return false;
		}
		return true;
	}
	
	public void LoadMap(string filePath)
	{
		StartCoroutine (LoadMapFrameWait (filePath));
	}

	IEnumerator LoadMapFrameWait(string filePath)
	{
		LevelSerializer.Progress -= LevelSerializeProgress;
		LevelSerializer.Progress += LevelSerializeProgress;
		Destroy(mapRoot);

		yield return new WaitForEndOfFrame ();

		Action<LevelLoader> loadedComplete = (LevelLoader) => 
		{
			uiController.TurnOffLoadingBar();
			StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.ChangeState(LevelCreatorStates.LevelCreation, null); 
			mapRoot = GameObject.Find("MapRoot");

			var startObj = GameObject.Find("PlayerStartCube");
			if(GameObject.Find("Player(Clone)") == null && startObj != null)
			{
				var playerPrefab = (GameObject)Resources.Load("Player");

				var playerCube = (GameObject)Instantiate(playerPrefab);

				playerCube.transform.parent = startObj.transform;
				playerCube.transform.localPosition = new Vector3(0, 1.01f, 0);

				playerCube.GetComponent<PlayerCharacter>().ChangeColour(startObj.GetComponent<ColorCollisionObject>().objColour);
				playerCube.layer = 10;
				playerCube.GetComponent<PlayerCharacter>().playerMovement.canMove = false;
				playerCube.rigidbody.useGravity = false;
			}
		};

		LevelSerializer.LoadObjectTreeFromFile (filePath, loadedComplete);
	}

	void LevelSerializeProgress (string status, float progress)
	{
		if(status == "Loading")
		{
			uiController.UpdateProgressBar (false, progress);

			if(progress == 1.0f)
			{
				isLoading = false;
			}

		}
		else if(status == "Saving")
		{
			uiController.UpdateProgressBar (true, progress);

			if(progress == 1.0f)
			{
				isSaving = false;
			}
		}
	}

	//Aren't nested delegates easy to read....
	//This loads a scene, once it's done, it loads the object tree (map) for that level, once THATS done, it sets up the null cubes in the map.
	public void LoadMapForPlayMode(string filePath)
	{
		SceneLoader.Instance.LoadLevel("UserLevelScene", delegate
		{
			LevelSerializer.LoadObjectTreeFromFile(filePath, delegate(LevelLoader obj)
			{
				SetupNullCubes();
				CreatePlayer();
				LevelController.Instance.InitLevel();
			});
		});
	}

	void SetupNullCube(GameObject nullCube)
	{
		nullCube.collider.enabled = true;
		nullCube.renderer.enabled = false;
		nullCube.GetComponent<BoxCollider>().size = new Vector3(1, 10, 1);
	}

	void SetupNullCubes()
	{
		var nullCubes = GameObject.FindGameObjectsWithTag("NullCube");

		foreach(var cube in nullCubes)
		{
			SetupNullCube(cube);
		}
	}

	void CreatePlayer()
	{
		var playerStart = GameObject.Find("PlayerStartCube");

		var playerPrefab = (GameObject)Resources.Load("Player");

		var playerCube = (GameObject)Instantiate(playerPrefab);
		playerCube.name = playerCube.name.Replace ("(Clone)", string.Empty);
		var playerPos = playerStart.transform.position;

		playerPos.y += 3;

		playerCube.transform.position = playerPos;

		Resources.UnloadUnusedAssets();

		LevelController.Instance.playerChar = playerCube.GetComponent<PlayerCharacter> ();

		LevelController.Instance.playerChar.ChangeColour(GameObject.Find("PlayerStartCube").GetComponent<PlayerStartPiece>().objColour);
	}
}