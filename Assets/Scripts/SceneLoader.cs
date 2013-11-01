using UnityEngine;
using System;
using System.Collections;

public class SceneLoader : MonoSingleton<SceneLoader> 
{
	public GameObject loadingScreenObjsParent;
	private GameObject progressBarObj;
	public UISlider progressBar;

	Action onComplete;

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
		if(arg1 == "Loading")
		{
			progressBar.sliderValue = arg2;
			if(arg2 > 0.999f)
				ProgressComplete();
		}
	}

	void ProgressComplete()
	{
		DestroyLoadingScreenObjects();
		Destroy(this);
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

		FindLoadingScreenObjects();
		LevelSerializer.Progress += HandleProgress;

		if(Application.HasProLicense())
		{
			var asyncOp = Application.LoadLevelAsync(levelToLoad);
			yield return asyncOp;
		}
		else
		{
			Application.LoadLevel(levelToLoad);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
		}
		if(LevelController.Instance.isStoryMode)
		{
			ProgressComplete();
		}
		else
		{
			LevelSerializer.Progress += HandleProgress;
			Destroy(this);
		}
	}

	private IEnumerator LoadLevelRoutine(string levelToLoad, Action onComplete)
	{
		this.onComplete = onComplete;
		Application.LoadLevel("LoadingScene");
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		FindLoadingScreenObjects();

		if(Application.HasProLicense())
		{
			var asyncOp = Application.LoadLevelAsync (levelToLoad);

			yield return asyncOp;
			yield return new WaitForEndOfFrame();
		}
		else
		{
			Application.LoadLevel(levelToLoad);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
		}
		if(onComplete != null)
			onComplete();
		if(LevelController.Instance.isStoryMode)
		{
			ProgressComplete();
		}
		else
		{
			LevelSerializer.Progress += HandleProgress;
		}
	}

	void FindLoadingScreenObjects()
	{
		loadingScreenObjsParent = GameObject.Find("LoadingScreenObjs");
		progressBarObj = GameObject.Find(@"Progress Bar");
		progressBar = progressBarObj.GetComponent<UISlider>();
		DontDestroyOnLoad(loadingScreenObjsParent);
	}

	void DestroyLoadingScreenObjects()
	{
		if(loadingScreenObjsParent != null)
		{
			Destroy(loadingScreenObjsParent);
		}
	}
}
