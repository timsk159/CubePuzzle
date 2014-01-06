using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum LevelIntroMessage
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
		cutSceneController = (CutSceneController)FindObjectOfType(typeof(CutSceneController));
	}

	public void InitIntro()
	{
		mapRoot = GameObject.Find("MapRoot");
		animatingCubes = new List<AnimatingCube>(mapRoot.transform.childCount);
		InitTweens();
	}

	void InitTweens()
	{
		foreach(Transform child in mapRoot.transform)
		{
			if(!child.name.Contains("Start") && !child.name.Contains("End") && child.tag != "NullCube")
			{
				var animCube = new AnimatingCube(child.gameObject, child.position);
				animatingCubes.Add(animCube);
				iTween.Init(child.gameObject);
			}
		}
	}

	//T = 0.0 - 1.0 defines progress of animation.
	//Am = amount of cubes.
	//
	//TweenTime = T/animTime
	public IEnumerator PlayIntroAnimation(GameObject playerObj, CutSceneObj introCutsceneObj = null)
	{
		Messenger.Invoke(LevelIntroMessage.IntroStarted.ToString());
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
	
		while(LevelSerializer.IsDeserializing)
			yield return new WaitForEndOfFrame();

		foreach(var cube in animatingCubes)
		{
			movePos = new Vector3(UnityEngine.Random.Range(0, (cube.theCube.transform.position.x * 1.5f)), movePos.y, UnityEngine.Random.Range(0, (cube.theCube.transform.position.z * 1.5f)));
			cube.MoveFromSkyToEndPos(movePos);
		}

		//Calculate the angle we need to get the camera behind the player, then animate so that we end up at that pos after animTime seconds.
		float timeCounter = introAnimTime;

		var camForward = Camera.main.transform.forward;
		var playerForward = playerObj.transform.forward;
		camForward.y = 0;
		playerForward.y = 0;
		var angle = Vector3.Angle(camForward, playerForward);
		//Stop just before we get behind to smooth the transition when CameraFollow gets turned on.
		angle += 350f;
		var rotAmount = angle / introAnimTime;

		while(timeCounter > 0 && playingIntro)
		{
			timeCounter -= Time.deltaTime;

			Camera.main.transform.RotateAround(playerObj.transform.position, Vector3.up, (rotAmount * Time.deltaTime));

			yield return new WaitForEndOfFrame();
		}

		if(playingIntro)
			Messenger.Invoke(LevelIntroMessage.IntroFinished.ToString());
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
		//Have to wait for a frame due to removing components (iTweens)
		yield return new WaitForEndOfFrame();

		if(playingCutsceneObj)
		{
			cutSceneController.StopCutScene();
			playingCutsceneObj = null;
		}
		Messenger.Invoke(LevelIntroMessage.IntroInterrupted.ToString());
		yield return new WaitForSeconds(0.5f);
		Messenger.Invoke(LevelIntroMessage.IntroFinished.ToString());
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
			iTween.MoveFrom(theCube, iTween.Hash("position", movePos, "time", 0.7f, "delay", UnityEngine.Random.Range(0.5f, 4.0f), "easetype", iTween.EaseType.easeOutExpo));
		}

		public void InterruptAnimation()
		{
			if(theCube.transform.position != endPos)
			{
				iTween.Stop(theCube);
				iTween.MoveTo(theCube, iTween.Hash("position", endPos, "time", 0.5f, "easetype", iTween.EaseType.easeInOutSine));
			}
		}
	}
}
