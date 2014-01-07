using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class MeshCombineAndSave : MonoBehaviour 
{
	public MeshFilter[] meshesToCombine;

	void OnGUI()
	{
		if(GUILayout.Button("Save Mesh", GUILayout.Height(100), GUILayout.Width(100)))
		{
			var mesh = CombineMeshes(false);

			SaveMesh(mesh, "Assets/Artwork/3D/SavedMeshNoMerge.asset");
		}
	}

	Mesh CombineMeshes(bool mergeSubMeshes)
	{
		Mesh returnMesh = new Mesh();

		var combine = new CombineInstance[meshesToCombine.Length];

		for(int i = 0; i < meshesToCombine.Length; i++)
		{
			combine[i].mesh = meshesToCombine[i].mesh;
			combine[i].transform = meshesToCombine[i].transform.localToWorldMatrix;
		}

		returnMesh.CombineMeshes(combine, mergeSubMeshes);

		return returnMesh;
	}

	void SaveMesh(Mesh mesh, string path)
	{
		AssetDatabase.CreateAsset(mesh, path);
	}
}
