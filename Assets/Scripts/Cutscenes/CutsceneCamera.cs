using UnityEngine;
using System.Collections;

public class CutsceneCamera : MonoBehaviour 
{
	Camera mainCam;
	AudioListener mainCamAudioListener;
	float originalMainCamDepth;


	public void PlayAnimation(AnimationClip clip)
	{
		mainCam = Camera.main;
		originalMainCamDepth = mainCam.depth;
		mainCamAudioListener = mainCam.GetComponent<AudioListener>();

		//Swap camera depths to ensure this camera is drawn on top.
		mainCam.depth = camera.depth;
		camera.depth = originalMainCamDepth;
		mainCamAudioListener.enabled = false;

		if(camera.animation == null)
			camera.gameObject.AddComponent<Animation>();

		camera.animation.clip = clip;

		camera.animation.Play(clip.name);
	}

	public void FinishAnimation()
	{
		camera.depth = mainCam.depth;
		mainCam.depth = originalMainCamDepth;
		mainCamAudioListener.enabled = true;
	}
}
