using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SplashScreens : MonoBehaviour 
{
	public List<SplashScreen> splashScreens;
	public SplashScreen currentSplash;

	public AudioSource audioS;
	public UITexture uiTexture;

	public bool canSkip;

	public void Start()
	{
#if DEMO_VERSION
		var webSecure = new WebSecure();

		if(!webSecure.IsHostedCorrectly())
		{
			//Make sure to modify url for each site thats hosting below (seperate build for each site)
			FailedSecurityUI();
			return;
		}
#endif
		StartCoroutine(SplashScreenDisplayRoutine());
	}

	void FailedSecurityUI()
	{

	}

	IEnumerator SplashScreenDisplayRoutine(int indexToStart = 0)
	{
		if(indexToStart > splashScreens.Count)
		{
			throw new System.ArgumentException("Start index exceeeded list count", "indexToStart");
		}

		for(int i = indexToStart; i < splashScreens.Count; i++)
		{
			canSkip = false;

			currentSplash = splashScreens[i];
			currentSplash.Show(uiTexture, audioS);

			if(currentSplash.timeTillSkippable == 0)
			{
				canSkip = true;
			}
			else
			{
				yield return new WaitForSeconds(currentSplash.timeTillSkippable);
				canSkip = true;
			}

			yield return new WaitForSeconds(currentSplash.timeToShow);
		}
		SplashScreensFinished();
	}

	void SkipSplash()
	{
		StopAllCoroutines();
		canSkip = false;

		var index = splashScreens.IndexOf(currentSplash);
		index++;

		if(index > splashScreens.Count)
		{
			SplashScreensFinished();
		}
		else
		{
			StartCoroutine(SplashScreenDisplayRoutine(index));
		}
	}

	void Update()
	{
		if(canSkip)
		{
			if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
			{
				SkipSplash();
			}
		}
	}

	void SplashScreensFinished()
	{
		if(Application.CanStreamedLevelBeLoaded("FrontMenu"))
		{
			Application.LoadLevel("FrontMenu");
		}
	}
}


public class SplashScreen
{
	public Texture2D splashImage;
	public AudioClip audio;
	public float timeToShow;
	public float timeTillSkippable;

	public void Show(UITexture uiTexture, AudioSource audioS)
	{
		uiTexture.mainTexture = splashImage;
		if(audioS != null)
		{
			audioS.clip = audio;
			audioS.Play();
		}
	}

	public void Show(Material mat, AudioSource audioS)
	{
		mat.mainTexture = splashImage;
		if(audioS != null)
		{
			audioS.clip = audio;
			audioS.Play();
		}
	}
}