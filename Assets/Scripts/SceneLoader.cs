using UnityEngine;
using System;
using System.Collections;

public class SceneLoader : MonoBehaviour 
{
	private static SceneLoader _instance;

	public static SceneLoader Instance 
	{
		get
		{
			if(_instance == null)
			{
				if(GameObject.Find("SceneLoader") == null)
				{
					_instance = new GameObject("SceneLoader").AddComponent<SceneLoader>();
				}
				else
				{
					_instance = GameObject.Find ("SceneLoader").GetComponent<SceneLoader> ();
				}

				if(_instance == null)
				{
					_instance = new GameObject ("SceneLoader").AddComponent<SceneLoader> ();
				}
			}
			return _instance;
		}
	}
	
	void Awake()
	{
		DontDestroyOnLoad(this);
	}

	public void LoadLevel(string levelToLoad)
	{
		StartCoroutine(LoadLevelRoutine(levelToLoad));
	}

	public void LoadLevel(string levelToLoad, Action onComplete)
	{
		StartCoroutine(LoadLevelRoutine(levelToLoad, onComplete));
	}

	public IEnumerator LoadLevelRoutine(string levelToLoad)
	{
		var asyncOp = Application.LoadLevelAsync (levelToLoad);
		yield return asyncOp;
	}

	public IEnumerator LoadLevelRoutine(string levelToLoad, Action onComplete)
	{
		Action doOnComplete = onComplete;
		var asyncOp = Application.LoadLevelAsync (levelToLoad);
		Debug.Log("Scene loading started, waiting for asyncOp to finished");
		yield return asyncOp;
		Debug.Log("scene loading finished, we should be loading the objects now");
		doOnComplete();
	}
}
