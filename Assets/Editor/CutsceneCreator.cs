using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public class CutsceneCreator : EditorWindow
{
	CutSceneObj cutsceneObj;
	TextAsset tempDialogueAsset;
	AudioClip tempAudioClip;
	AnimationClip tempAnimClip;

	[MenuItem("Window/Cutscene Creator")]
	static void CreateWindow()
	{
		var window = GetWindowWithRect<CutsceneCreator>(new Rect(150,150,250,300), true, "Cutscene Creator");
		window.Init();
	}

	void Init()
	{
		cutsceneObj = ScriptableObject.CreateInstance<CutSceneObj>();
		cutsceneObj.dialogue = new Dialogue();
	}

	void OnGUI()
	{
		EditorGUILayout.PrefixLabel("Dialogue .txt file");
		tempDialogueAsset = (TextAsset)EditorGUILayout.ObjectField(tempDialogueAsset, typeof(TextAsset), false);

		EditorGUILayout.PrefixLabel("Audio file");
		tempAudioClip = (AudioClip)EditorGUILayout.ObjectField(tempAudioClip, typeof(AudioClip), false);

		EditorGUILayout.PrefixLabel("Camera animation clip");
		tempAnimClip = (AnimationClip)EditorGUILayout.ObjectField(tempAnimClip, typeof(AnimationClip), false);

		EditorGUILayout.PrefixLabel("Length in Seconds");
		cutsceneObj.lengthInSeconds = EditorGUILayout.FloatField(cutsceneObj.lengthInSeconds);

		if(GUILayout.Button("Create"))
		{
			if(tempDialogueAsset != null && tempAudioClip != null && tempAnimClip != null)
			{
				cutsceneObj.dialogue.dialogueAssetPath = GetResourcesPathForAsset(AssetDatabase.GetAssetPath(tempDialogueAsset));
				cutsceneObj.audioClipFilePath = GetResourcesPathForAsset(AssetDatabase.GetAssetPath(tempAudioClip));
				cutsceneObj.cameraAnimFilepath = GetResourcesPathForAsset(AssetDatabase.GetAssetPath(tempAnimClip));

				var filePath = EditorUtility.SaveFilePanelInProject("Save Location", "", "asset", "");
				var inAssetsFolder = filePath.Contains("Assets");
				if(!inAssetsFolder)
				{
					EditorUtility.DisplayDialog("Error", "You must save the file in the assets folder for this project", "Ok");
					filePath = "";
				}

				if(!string.IsNullOrEmpty(filePath))
				{
					if(cutsceneObj.audioClipFilePath == "NOT IN RESOURCES!")
					{
						EditorUtility.DisplayDialog("Error", "Audio file must be placed inside the Assets/Resources/ folder. Please move the file and try again", "Ok");
					}
					else if(cutsceneObj.dialogue.dialogueAssetPath == "NOT IN RESOURCES!")
					{
						EditorUtility.DisplayDialog("Error", "Dialogue text file must be placed inside the Assets/Resources/ folder. Please move the file and try again", "Ok");
					}
					else if(cutsceneObj.cameraAnimFilepath == "NOT IN RESOURCES!")
					{
						EditorUtility.DisplayDialog("Error", "Camera animation text file must be placed inside the Assets/Resources folder. Please move the file and try again", "Ok");
					}
					else
					{
						filePath = RemoveWindowsPathFromFilePath(filePath);

						AssetDatabase.CreateAsset(cutsceneObj, filePath);

						AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(cutsceneObj));
					}
				}
			}
		}
	}

	string GetResourcesPathForAsset(string filePath)
	{
		if(!filePath.Contains("Assets/Resources/"))
		{
			return "NOT IN RESOURCES!";
		}
		var filePathRelativeToResources = filePath.Substring(filePath.IndexOf("Resources/") + 10);
		var noFileExtension = filePathRelativeToResources.Remove(filePathRelativeToResources.LastIndexOf("."), 4);
		return noFileExtension;
	}

	string RemoveWindowsPathFromFilePath(string filePath)
	{
		var substr = filePath.Substring(filePath.IndexOf("Assets/"));
		return substr;
	}
}
