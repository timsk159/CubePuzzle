using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum LevelIntroNotification
{
	IntroStarted, IntroInterrupted, IntroFinished
};

public class LevelIntro : MonoBehaviour 
{
	bool playingIntro;
	GameObject mapRoot;
	float introAnimTime = 4.0f;
	List<AnimatingCube> animatingCubes;

	void Awake()
	{
		mapRoot = GameObject.Find("MapRoot");
	}

	//T = 0.0 - 1.0 defines progress of animation.
	//Am = amount of cubes.
	//
	//TweenTime = T/animTime
	public IEnumerator PlayIntroAnimation(GameObject playerObj)
	{
		NotificationCenter<LevelIntroNotification>.DefaultCenter.PostNotification(LevelIntroNotification.IntroStarted, null);
		introAnimTime = 4.0f;
		mapRoot = GameObject.Find("MapRoot"); 
		playingIntro = true;
		var movePos = mapRoot.transform.up * 20;

		animatingCubes = new List<AnimatingCube>(mapRoot.transform.childCount);

		foreach(Transform child in mapRoot.transform)
		{
			if(!child.name.Contains("Start") && !child.name.Contains("End"))
			{
				var animCube = new AnimatingCube(child.gameObject, child.position);
				animatingCubes.Add(animCube);
				animCube.MoveFromSkyToEndPos(movePos);
			}
		}

		//Calculate the angle we need to get the camera behind the player, then animate so that we end up at that pos after animTime seconds.
		float timeCounter = introAnimTime;

		var camForward = Camera.main.transform.forward;
		var playerForward = playerObj.transform.forward;
		camForward.y = 0;
		playerForward.y = 0;
		var angle = Vector3.Angle(camForward, playerForward);
		angle += 350f;
		var rotAmount = angle / introAnimTime;

		while(timeCounter > 0 && playingIntro)
		{
			timeCounter -= Time.deltaTime;

			Camera.main.transform.RotateAround(playerObj.transform.position, Vector3.up, (rotAmount * Time.deltaTime));

			yield return new WaitForEndOfFrame();
		}

		//Anim was not interrupted.
		if(timeCounter <= 0)
		{
		}
		yield return new WaitForSeconds(1.0f);
		NotificationCenter<LevelIntroNotification>.DefaultCenter.PostNotification(LevelIntroNotification.IntroFinished, null);
	}

	void Update()
	{
		if(playingIntro)
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				//Finish the intro.
				StartCoroutine(QuickFinishAnimation());
			}
		}
	}

	IEnumerator QuickFinishAnimation()
	{
		NotificationCenter<LevelIntroNotification>.DefaultCenter.PostNotification(LevelIntroNotification.IntroInterrupted, null);
		playingIntro = false;
		var movePos = mapRoot.transform.up * 20;

		foreach(var cube in animatingCubes)
		{
			cube.InterruptAnimation();
		}
		yield return new WaitForSeconds(0.5f);
	}


	public class AnimatingCube
	{
		public GameObject theCube;
		public Vector3 endPos;

		public AnimatingCube(GameObject theCube, Vector3 endPos)
		{
			this.theCube = theCube;
			this.endPos = endPos;
		}

		public void MoveFromSkyToEndPos(Vector3 movePos)
		{
			endPos = theCube.transform.position;
			iTween.MoveFrom(theCube, iTween.Hash("position", movePos, "time", 0.5f, "delay", UnityEngine.Random.Range(0.5f, 4.0f)));
		}

		public void InterruptAnimation()
		{
			if(theCube.transform.position != endPos)
			{
				iTween.Stop(theCube);
				iTween.MoveTo(theCube, iTween.Hash("position", endPos, "time", 0.5f));
			}
		}
	}
}
