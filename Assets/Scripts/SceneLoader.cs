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

	public GameObject loadingScreenObjsParent;
	private GameObject progressBarObj;
	public UISlider progressBar;

	void Awake()
	{
		DontDestroyOnLoad(this);
	}

	void OnDestroy()
	{
		LevelSerializer.Progress -= HandleProgress;
		Destroy(loadingScreenObjsParent);
	}

	void HandleProgress (string arg1, float arg2)
	{
		progressBar.sliderValue = arg2;
	}

	public void LoadLevel(string levelToLoad)
	{
		StartCoroutine(LoadLevelRoutine(levelToLoad));
	}

	public void LoadLevel(string levelToLoad, Action onComplete)
	{
		StartCoroutine(LoadLevelRoutine(levelToLoad, onComplete));
	}

	private IEnumerator LoadLevelRoutine(string levelToLoad)
	{		
		Application.LoadLevel("LoadingScene");
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		loadingScreenObjsParent = GameObject.Find("LoadingScreenObjs");
		progressBarObj = GameObject.Find(@"Progress Bar");
		progressBar = progressBarObj.GetComponent<UISlider>();
		LevelSerializer.Progress += HandleProgress;

		var asyncOp = Application.LoadLevelAsync (levelToLoad);
		yield return asyncOp;
		Destroy(this);
	}

	private IEnumerator LoadLevelRoutine(string levelToLoad, Action onComplete)
	{
		Application.LoadLevel("LoadingScene");
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		loadingScreenObjsParent = GameObject.Find("LoadingScreenObjs");
		progressBarObj = GameObject.Find(@"Progress Bar");
		progressBar = progressBarObj.GetComponent<UISlider>();
		LevelSerializer.Progress += HandleProgress;

		var asyncOp = Application.LoadLevelAsync (levelToLoad);
		yield return asyncOp;
		if(onComplete != null)
			onComplete();
		Destroy(this);
	}
}
