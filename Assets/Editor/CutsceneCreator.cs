using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public class CutsceneCreator : EditorWindow
{
	CutsceneObj cutsceneObj;

	[MenuItem("Window/Cutscene Creator")]
	static void CreateWindow()
	{
		var window = GetWindow<CutsceneCreator>("Cutscene Creator");
		window.Init();
	}

	void Init()
	{
		cutsceneObj = ScriptableObject.CreateInstance<CutsceneObj>();
		cutsceneObj.dialogue = new Dialogue();
	}

	void OnGUI()
	{
		EditorGUILayout.PrefixLabel("Dialogue .txt file");
		cutsceneObj.dialogue.dialogueAsset = (TextAsset)EditorGUILayout.ObjectField(cutsceneObj.dialogue.dialogueAsset, typeof(TextAsset), false);

		EditorGUILayout.PrefixLabel("Audio file");
		cutsceneObj.audioClip = (AudioClip)EditorGUILayout.ObjectField(cutsceneObj.audioClip, typeof(AudioClip), false);

		if(GUILayout.Button("Create"))
		{
			/*
			var mat = new Material(Shader.Find("Specular"));
			AssetDatabase.CreateAsset(mat, "Assets/testMat.mat");

			var animClip = new AnimationClip();
			animClip.name = "testAnim";
			AssetDatabase.AddObjectToAsset(animClip, mat);
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(animClip));
			*/

			if(cutsceneObj.dialogue.dialogueAsset != null && cutsceneObj.audioClip != null)
			{
				var filePath = EditorUtility.SaveFilePanel("Save Location", "", "", ".asset");
				if(!string.IsNullOrEmpty(filePath))
				{
					AssetDatabase.CreateAsset(cutsceneObj, filePath);
					AssetDatabase.AddObjectToAsset(cutsceneObj, cutsceneObj.audioClip);
					AssetDatabase.AddObjectToAsset(cutsceneObj, cutsceneObj.dialogue.dialogueAsset);

					AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(cutsceneObj));
				}
			}
		}
	}
}
