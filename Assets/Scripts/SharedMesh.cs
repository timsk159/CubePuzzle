using UnityEngine;
using System.Collections;

public class SharedMesh : MonoBehaviour 
{
	bool _isUp;

	bool isMovingUp;
	bool isMovingDown;

	public bool IsUp {
		get;
		private set;
	}

	public void MoveUp()
	{
		//if(!isMovingUp)
		{
		//	ClearActiveTweens();
		//	Debug.Log("Stopped tweens, count is: " + iTween.Count(gameObject));
			iTween.MoveTo(gameObject, iTween.Hash("position", new Vector3(0, 0.5f, 0), "time", 0.5f, "oncomplete", "UpComplete"));
			isMovingUp = true;
			IsUp = true;
		}
	}

	public void MoveDown()
	{
		//	if(!isMovingDown)
		{
		//	ClearActiveTweens();
		//	Debug.Log("Stopped tweens, count is: " + iTween.Count(gameObject));
			iTween.MoveTo(gameObject, iTween.Hash("position", Vector3.zero, "time", 0.5f, "oncomplete", "DownComplete"));
			isMovingDown = true;
			IsUp = false;
		}
	}

	void ClearActiveTweens()
	{
		if(iTween.Count(gameObject) >= 1)
		{
			iTween.Stop(gameObject);
		}
	}

	void UpComplete()
	{
		IsUp = true;
		isMovingUp = false;
	}

	void DownComplete()
	{
		IsUp = false;
		isMovingDown = false;
	}
}
