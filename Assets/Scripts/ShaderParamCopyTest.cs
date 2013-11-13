using UnityEngine;
using System.Collections;

public class ShaderParamCopyTest : MonoBehaviour 
{
	public Material mat;

	void Start () 
	{
		if(mat != null)
		{
			renderer.sharedMaterial.CopyPropertiesFromMaterial(mat);
		}
	}
}
