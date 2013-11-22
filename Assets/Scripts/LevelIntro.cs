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
	CutSceneObj playingCutsceneObj;
	CutSceneController cutSceneController;

	void Awake()
	{
		mapRoot = GameObject.Find("MapRoot");
		cutSceneController = (CutSceneController)FindObjectOfType(typeof(CutSceneController));
	}

	void OnLevelWasLoaded(int levelId)
	{
		Awake();
	}

	//T = 0.0 - 1.0 defines progress of animation.
	//Am = amount of cubes.
	//
	//TweenTime = T/animTime
	public IEnumerator PlayIntroAnimation(GameObject playerObj, CutSceneObj introCutsceneObj = null)
	{
		Messenger.Invoke(LevelIntroNotification.IntroStarted.ToString());
		if(introCutsceneObj != null && cutSceneController != null)
		{
			playingCutsceneObj = introCutsceneObj;
			cutSceneController.DisplayCutscene(playingCutsceneObj);
			print(playingCutsceneObj.lengthInSeconds);
			introAnimTime = playingCutsceneObj.lengthInSeconds;
		}
		else
		{
			introAnimTime = 4.0f;
		}

		mapRoot = GameObject.Find("MapRoot"); 
		playingIntro = true;
		var movePos = mapRoot.transform.up * 20;

		animatingCubes = new List<AnimatingCube>(mapRoot.transform.childCount);

		foreach(Transform child in mapRoot.transform)
		{
			if(!child.name.Contains("Start") && !child.name.Contains("End") && child.tag != "NullCube")
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

	//	yield return new WaitForSeconds(1.0f);
		Messenger.Invoke(LevelIntroNotification.IntroFinished.ToString());
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
		playingIntro = false;

		foreach(var cube in animatingCubes)
		{
			cube.InterruptAnimation();
		}
		yield return new WaitForEndOfFrame();

		if(playingCutsceneObj)
		{
			cutSceneController.StopCutScene();
			playingCutsceneObj = null;
		}

		Messenger.Invoke(LevelIntroNotification.IntroInterrupted.ToString());
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
