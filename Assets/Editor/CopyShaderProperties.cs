using UnityEditor;
using UnityEngine;
using System.Collections;
using GUI = UnityEditor.EditorGUILayout;

public class CopyShaderProperties : EditorWindow 
{
	GameObject selection;
	string[] selectedShaderParams;

	[MenuItem("Window/Shader Property Clipboard")]
	static void CreateWindow()
	{
		var window = GetWindow<CopyShaderProperties>(true, "Cutscene Creator", true);
		window.Init();
	}

	void Init()
	{

	}

	void OnGUI()
	{
		if(Selection.activeGameObject != selection)
		{
			if(Selection.activeGameObject.renderer == null || Selection.activeGameObject.renderer.sharedMaterial == null)
			{
				GUI.LabelField("Selection has no shader");
			}
		}

		if(selection == null)
		{
			GUI.LabelField("Select a gameobject");
		}


		if(GUILayout.Button("Copy Properties"))
		{

		}

		if(GUILayout.Button("Paste Properties"))
		{

		}
	}

	void OnSelectionChanged()
	{
		CacheSelection();
		EditorWindow.FocusWindowIfItsOpen(typeof(CopyShaderProperties));
	}

	void CacheSelection()
	{
		selection = Selection.activeGameObject;
		selectedShaderParams = selection.renderer.sharedMaterial.shaderKeywords;
	}
}
