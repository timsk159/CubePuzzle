using UnityEngine;
using System;
using System.Collections;

[DoNotSerializePublic()]
public class SceneLoader : MonoSingleton<SceneLoader> 
{
	public GameObject loadingScreenObjsParent;
	private GameObject progressBarObj;
	public UISlider progressBar;

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
		if (Application.isWebPlayer)
		{
			if (Application.CanStreamedLevelBeLoaded(levelToLoad))
			{
				StartCoroutine(LoadLevelRoutine(levelToLoad));
			}
			else
			{
				StartCoroutine(LoadStreamedLevelRoutine(levelToLoad));
			}
		}
		else
			StartCoroutine(LoadLevelRoutine(levelToLoad));
	}

	public void LoadLevel(string levelToLoad, Action onComplete)
	{
		if (Application.isWebPlayer)
		{
			if (Application.CanStreamedLevelBeLoaded(levelToLoad))
			{
				StartCoroutine(LoadLevelRoutine(levelToLoad, onComplete));
			}
			else
			{
				StartCoroutine(LoadStreamedLevelRoutine(levelToLoad, onComplete));
			}
		}
		else
			StartCoroutine(LoadLevelRoutine(levelToLoad, onComplete));
	}

	public void LoadStreamedLevel(string levelToLoad)
	{
		StartCoroutine(LoadStreamedLevelRoutine(levelToLoad));
	}

	public void LoadStreamedLevel(string levelToLoad, Action onComplete)
	{
		StartCoroutine(LoadStreamedLevelRoutine(levelToLoad, onComplete));
	}

	private IEnumerator LoadStreamedLevelRoutine(string levelToLoad, Action onComplete = null)
	{
		if (!Application.CanStreamedLevelBeLoaded("LoadingScene"))
		{
			while (Application.GetStreamProgressForLevel("LoadingScene") != 1)
			{
				yield return new WaitForEndOfFrame();
			}
		}
		Application.LoadLevel("LoadingScene");
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		FindLoadingScreenObjects();

		while(Application.GetStreamProgressForLevel(levelToLoad) != 1)
		{
			progressBar.sliderValue = Application.GetStreamProgressForLevel(levelToLoad);
			yield return new WaitForEndOfFrame();
		}

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
		if(onComplete != null)
			onComplete();
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
