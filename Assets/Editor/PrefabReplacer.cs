using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PrefabReplacer : EditorWindow 
{
	List<ReplaceInfo> replaceInfos;

	int indexToReplace;

	[MenuItem("Window/Prefab Replacer")]
	static void Init()
	{
		var window = GetWindow<PrefabReplacer>("Prefab Replacer");
		window.replaceInfos = new List<ReplaceInfo>();
		window.indexToReplace = -1;
	}

	void OnGUI()
	{
		if(GUILayout.Button("Add replace info"))
		{
			replaceInfos.Add(new ReplaceInfo());
		}
		foreach(var replace in replaceInfos)
		{
			replace.prefab = (GameObject)EditorGUILayout.ObjectField(replace.prefab, typeof(GameObject), false);
			GUILayout.BeginHorizontal();

			replace.matchScale = EditorGUILayout.Toggle("Match scale?", replace.matchScale);

			EditorGUILayout.Space();

			replace.nameToMatch = EditorGUILayout.TextField(replace.nameToMatch);

			if(GUILayout.Button("Remove"))
			{
				indexToReplace = replaceInfos.IndexOf(replace);
			}

			GUILayout.EndHorizontal();
			EditorGUILayout.Separator();
		}

		if(indexToReplace != -1)
		{
			replaceInfos.RemoveAt(indexToReplace);
			indexToReplace = -1;
		}

		if(GUILayout.Button("Replace!"))
		{
			ReplaceGameobjects();
		}
	}


	void ReplaceGameobjects()
	{
		foreach(var replaceinfo in replaceInfos)
		{
			var gameobjectsToReplace = new List<GameObject>();

			var allGameobjects = GameObject.FindObjectsOfType<GameObject>();

			gameobjectsToReplace = allGameobjects.ToList().Where(go => go.name == replaceinfo.nameToMatch).ToList();

			foreach(var go in gameobjectsToReplace)
			{
				var newGO = (GameObject)PrefabUtility.InstantiatePrefab(replaceinfo.prefab);

				MatchTransform(go, newGO, replaceinfo.matchScale);

				DestroyImmediate(go);
			}
		}
	}

	void MatchTransform(GameObject oldGO, GameObject newGO, bool matchScale)
	{
		newGO.transform.parent = oldGO.transform.parent;

		var pos = oldGO.transform.localPosition;
		var rot = oldGO.transform.localRotation;
		var scal = oldGO.transform.localScale;

		newGO.transform.localPosition = pos;
		newGO.transform.localRotation = rot;
		if(matchScale)
			newGO.transform.localScale = scal;
	}
}

public class ReplaceInfo
{
	public GameObject prefab;
	public string nameToMatch;
	public bool matchScale;
}
