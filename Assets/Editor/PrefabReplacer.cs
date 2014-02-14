using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PrefabReplacer : EditorWindow 
{
	List<string> namesOfGameobjectsToReplace;
	GameObject prefab;
	bool matchScale;

	int indexToReplace;

	[MenuItem("Window/Prefab Replacer")]
	static void Init()
	{
		var window = GetWindow<PrefabReplacer>("Prefab Replacer");
		window.namesOfGameobjectsToReplace = new List<string>();
		window.indexToReplace = -1;
	}

	void OnGUI()
	{
		prefab = (GameObject)EditorGUILayout.ObjectField(prefab, typeof(GameObject), false);

		matchScale = EditorGUILayout.Toggle("Match Scale?", matchScale);

		EditorGUILayout.Space();

		GUILayout.Label("Names: ");
		if(GUILayout.Button("Add Name"))
		{
			namesOfGameobjectsToReplace.Add("");
		}
		for(int i = 0; i < namesOfGameobjectsToReplace.Count; i++)
		{
			GUILayout.BeginHorizontal();
			namesOfGameobjectsToReplace[i] = GUILayout.TextField(namesOfGameobjectsToReplace[i]);

			if(GUILayout.Button("-"))
			{
				indexToReplace = i;
			}
			GUILayout.EndHorizontal();
		}

		if(indexToReplace != -1)
		{
			namesOfGameobjectsToReplace.RemoveAt(indexToReplace);
			indexToReplace = -1;
		}

		if(GUILayout.Button("Replace!"))
		{
			if(prefab != null && namesOfGameobjectsToReplace.Count != 0)
				ReplaceGameobjects();
		}
	}

	void ReplaceGameobjects()
	{
		var gameobjectsToReplace = new List<GameObject>();

		var allGameobjects = GameObject.FindObjectsOfType<GameObject>();

		gameobjectsToReplace = allGameobjects.ToList().Where(go => namesOfGameobjectsToReplace.Contains(go.name)).ToList();

		foreach(var go in gameobjectsToReplace)
		{
			var newGO = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

			MatchTransform(go, newGO);

			DestroyImmediate(go);
		}
	}

	void MatchTransform(GameObject oldGO, GameObject newGO)
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
