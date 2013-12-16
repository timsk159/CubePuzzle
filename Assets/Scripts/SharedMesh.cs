using UnityEngine;
using System.Collections;

public class SharedMesh : MonoBehaviour 
{
	bool _isUp;

	public bool IsUp {
		get;
		private set;
	}

	public void MoveUp()
	{
		iTween.MoveTo(gameObject, iTween.Hash("position", new Vector3(0, 0.3f, 0), "time", 0.5f, "oncomplete", "UpComplete"));
	}

	public void MoveDown()
	{
		iTween.MoveTo(gameObject, iTween.Hash("position", Vector3.zero, "time", 0.5f, "oncomplete", "DownComplete"));
	}

	void UpComplete()
	{
		IsUp = true;
	}

	void DownComplete()
	{
		IsUp = false;
	}
}
