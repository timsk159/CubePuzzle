using UnityEngine;
using System.Collections;

public class NullPiece : MonoBehaviour 
{
	void Start()
	{
		var normals = GetComponent<MeshFilter>().mesh.normals;
	}
}
