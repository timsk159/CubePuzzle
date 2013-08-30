using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode()]

[System.Serializable]
[CustomEditor(typeof(CutSceneTrigger))]
public class CutsceneTriggerInspector : Editor 
{
	CutSceneTrigger thisTrigger;

	bool oldIsComplex;
	bool isComplex;

	protected virtual void OnEnable ()
	{
		thisTrigger = (CutSceneTrigger)target;
		if(thisTrigger.cutSceneObj == null)
			thisTrigger.cutSceneObj = new CutsceneObj();
	}

	public override void OnInspectorGUI ()
	{
		if(thisTrigger.cutSceneObj != null)
		{
			EditorGUILayout.LabelField("Cut Scene Object:");

			EditorGUILayout.Space();

			thisTrigger.cutSceneObj.dialogueAsset = (TextAsset)EditorGUILayout.ObjectField("Dialogue File: ", thisTrigger.cutSceneObj.dialogueAsset, typeof(TextAsset), false);

			thisTrigger.cutSceneObj.timeInSeconds = EditorGUILayout.IntField("Scene Length: ", thisTrigger.cutSceneObj.timeInSeconds);

			isComplex = EditorGUILayout.Toggle("Complex scene?", isComplex);

			if(isComplex != oldIsComplex)
			{
				var cachedDialogue = thisTrigger.cutSceneObj.dialogueAsset;

				var cachedTime = thisTrigger.cutSceneObj.timeInSeconds;

				if(isComplex)
				{
					thisTrigger.cutSceneObj = new ComplexCutsceneObj();
				}
				else
				{
					thisTrigger.cutSceneObj = new SimpleCutSceneObj();
				}
				thisTrigger.cutSceneObj.dialogueAsset = cachedDialogue;
				thisTrigger.cutSceneObj.timeInSeconds = cachedTime;

				oldIsComplex = isComplex;
			}

			if(isComplex)
			{
				//Draw complex UI
				var complexObj = thisTrigger.cutSceneObj as ComplexCutsceneObj;

				complexObj.prefabToInstantiate = (GameObject)EditorGUILayout.ObjectField("Prefab: ", complexObj.prefabToInstantiate, typeof(GameObject), false);

				complexObj.dialogueAsset = thisTrigger.cutSceneObj.dialogueAsset;

				complexObj.timeInSeconds = thisTrigger.cutSceneObj.timeInSeconds;

				thisTrigger.cutSceneObj = complexObj;
			}
			else
			{
				//Draw simple UI
				var simpleObj = thisTrigger.cutSceneObj as SimpleCutSceneObj;

				simpleObj.cameraAnimation = (AnimationClip)EditorGUILayout.ObjectField("Camera Animation: ", simpleObj.cameraAnimation, typeof(AnimationClip), false);

				simpleObj.narrationAudio = (AudioClip)EditorGUILayout.ObjectField("Audio file: ", simpleObj.narrationAudio, typeof(AudioClip), false);

				simpleObj.dialogueAsset = thisTrigger.cutSceneObj.dialogueAsset;

				simpleObj.timeInSeconds = thisTrigger.cutSceneObj.timeInSeconds;

				thisTrigger.cutSceneObj = simpleObj;
			}
		}
		EditorGUILayout.Separator();

		DrawDefaultInspector();
	}
}
