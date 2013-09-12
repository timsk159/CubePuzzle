using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class CutsceneCreator : EditorWindow 
{
	static CutsceneCreator window;

	CutsceneObj cutsceneObj;
	string fileName;

	Rect buttonsRect;

	List<CutSceneAnimation.TimedDialogue> dialoguesToRemove = new List<CutSceneAnimation.TimedDialogue>();
	List<CutSceneAnimation.TimedAudio> audiosToRemove = new List<CutSceneAnimation.TimedAudio>();
	List<CutSceneAnimation.TimedAnimationClip> animsToRemove = new List<CutSceneAnimation.TimedAnimationClip>();

	//Just for easier typing...
	CutSceneAnimation csanim;

	[MenuItem("Window/Cutscene Creator")]
	static void Init()
	{
		window = EditorWindow.GetWindow<CutsceneCreator>("Cut scene creator");

		window.SetupWindow();
	}

	void SetupWindow()
	{
		cutsceneObj = ScriptableObject.CreateInstance<CutsceneObj>();

		cutsceneObj.cutSceneAnim.Init();

		csanim = cutsceneObj.cutSceneAnim;
	}

	void OnGUI()
	{
		cutsceneObj.lengthInSeconds = EditorGUILayout.IntSlider("Length", cutsceneObj.lengthInSeconds, 1, 240);

		DrawTimeline();
		GUILayout.Space(350);
		DrawAddfileButtons();

		/*
		foreach(var timedDialogue in csanim.timedDialogue)
		{
			EditorGUILayout.BeginHorizontal();

			timedDialogue.dialogue.dialogueAsset = (TextAsset)EditorGUILayout.ObjectField("Dialogue: ", timedDialogue.dialogue.dialogueAsset, typeof(TextAsset), false);

			if(GUILayout.Button("-", GUILayout.Width(20)))
			{
				dialoguesToRemove.Add(timedDialogue);
			}
			EditorGUILayout.LabelField ((timedDialogue.timeToFinish - timedDialogue.timeToStart).ToString("##.0") + "s", GUILayout.Width(30));

			EditorGUILayout.MinMaxSlider(ref timedDialogue.timeToStart, ref timedDialogue.timeToFinish, 0, cutsceneObj.lengthInSeconds);

			EditorGUILayout.EndHorizontal();
		}
		
		foreach(var timedAudio in csanim.timedAudioClips)
		{
			EditorGUILayout.BeginHorizontal();
			timedAudio.clip = (AudioClip)EditorGUILayout.ObjectField("Audio: ", timedAudio.clip, typeof(AudioClip), false);

			if(GUILayout.Button("-", GUILayout.Width(20)))
			{
				audiosToRemove.Add(timedAudio);
			}

			if(timedAudio.clip != null)
			{
				var endTime = (timedAudio.timeToStart + timedAudio.clip.length);

				EditorGUILayout.LabelField ((endTime - timedAudio.timeToStart).ToString("##.0") + "s", GUILayout.Width(30));

				EditorGUILayout.MinMaxSlider(ref timedAudio.timeToStart, ref endTime, 0, cutsceneObj.lengthInSeconds);
			}

			EditorGUILayout.EndHorizontal();
		}

		foreach(var timedAnim in csanim.timedAnimationClips)
		{
			EditorGUILayout.BeginHorizontal();
			timedAnim.animClip = (AnimationClip)EditorGUILayout.ObjectField("Animation: ", timedAnim.animClip, typeof(AnimationClip), false);

			if(GUILayout.Button("-", GUILayout.Width(20)))
			{
				animsToRemove.Add(timedAnim);
			}

			if(timedAnim.animClip != null)
			{
				var endTime = (timedAnim.timeToStart + timedAnim.animClip.length);
				EditorGUILayout.LabelField ((endTime - timedAnim.timeToStart).ToString("##.0") + "s", GUILayout.Width(30));
				EditorGUILayout.MinMaxSlider(ref timedAnim.timeToStart, ref endTime, 0, cutsceneObj.lengthInSeconds);
			}

			EditorGUILayout.EndHorizontal();
		}

		if(dialoguesToRemove.Count > 0)
		{
			foreach(var itemToRemove in dialoguesToRemove)
			{
				csanim.timedDialogue.Remove(itemToRemove);
			}
		}
		if(audiosToRemove.Count > 0)
		{
			foreach(var itemtoremove in audiosToRemove)
			{
				csanim.timedAudioClips.Remove(itemtoremove);
			}
		}
		if(animsToRemove.Count > 0)
		{
			foreach(var itemtoremove in animsToRemove)
			{
				csanim.timedAnimationClips.Remove(itemtoremove);
			}
		}
		*/

		fileName = EditorGUILayout.TextField("Filename: ", fileName);

		if(!string.IsNullOrEmpty(fileName))
		{
			if(GUILayout.Button("Create!"))
			{
				if(!Directory.Exists(Application.dataPath + "/Resources/Cutscenes/"))
				{
					Directory.CreateDirectory(Application.dataPath + "/Resources/Cutscenes/");
				}
				AssetDatabase.CreateAsset(cutsceneObj, "Assets/Resources/Cutscenes/" + fileName + ".asset");

				AssetDatabase.Refresh();
			}
			EditorGUILayout.HelpBox(@"Cut scene assets are places in Resources/Cutscenes/", MessageType.Info);
		}
	}

	void DrawTimeline()
	{
		// 3 rows, one for each filetype
		var boxWidth = window.position.width - 10;
		var boxTop = 25;
		var sliderTop = 0;


		GUI.Box(new Rect(0, boxTop, boxWidth, 50),"");
		var areaRect = new Rect(0, boxTop, boxWidth, 50);

		GUI.BeginGroup(areaRect);

		foreach(var timedDialogue in csanim.timedDialogue)
		{
			EditorGUI.MinMaxSlider(new Rect(0, sliderTop, boxWidth, 15), ref timedDialogue.timeToStart, ref timedDialogue.timeToFinish, 0, cutsceneObj.lengthInSeconds);
			sliderTop += 15;
		}

		GUI.EndGroup();

		boxTop += 55;
		sliderTop = 0;

		GUI.Box(new Rect(0, boxTop, boxWidth, 50), "");
		areaRect = new Rect(0, boxTop, boxWidth, 50);

		GUI.BeginGroup(areaRect);

		foreach(var timedAudio in csanim.timedAudioClips)
		{
			if(timedAudio.clip != null)
			{
				var endTime = (timedAudio.timeToStart + timedAudio.clip.length);
				EditorGUI.MinMaxSlider(new Rect(0, sliderTop, boxWidth, 15), ref timedAudio.timeToStart, ref endTime, 0, cutsceneObj.lengthInSeconds);
				sliderTop += 15;
			}
		}

		GUI.EndGroup();

		boxTop += 55;
		sliderTop = 0;

		GUI.Box(new Rect(0, boxTop, boxWidth, 50), "");
		areaRect = new Rect(0, boxTop, boxWidth, 50);

		GUI.BeginGroup(areaRect);

		foreach(var timedAnim in csanim.timedAnimationClips)
		{
			if(timedAnim.animClip != null)
			{
				var endTime = (timedAnim.timeToStart + timedAnim.animClip.length);
				EditorGUI.MinMaxSlider(new Rect(0, sliderTop, boxWidth, 15), ref timedAnim.timeToStart, ref endTime, 0, cutsceneObj.lengthInSeconds);
				sliderTop += 15;
			}
		}

		GUI.EndGroup();
		
	}

	void DrawAddfileButtons()
	{
		if(GUILayout.Button("Add Dialogue"))
		{
			csanim.timedDialogue.Add(new CutSceneAnimation.TimedDialogue());
		}
		if(GUILayout.Button("Add Audio"))
		{
			csanim.timedAudioClips.Add(new CutSceneAnimation.TimedAudio());
		}
		if(GUILayout.Button("Add Animation"))
		{
			csanim.timedAnimationClips.Add(new CutSceneAnimation.TimedAnimationClip());
		}
	}
}
