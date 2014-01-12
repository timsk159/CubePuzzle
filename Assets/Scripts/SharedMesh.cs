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
		if(!isMovingUp)
		{
			ClearActiveTweens();
			print("Moving mesh: " + gameObject.name + " Up. PLayers colour is: " + LevelController.Instance.PlayerColour);
			iTween.MoveTo(gameObject, iTween.Hash("position", new Vector3(0, 0.5f, 0), "time", 0.5f, "oncomplete", "UpComplete"));
			isMovingUp = true;
		}
	}

	public void MoveDown()
	{
		if(!isMovingDown)
		{
			ClearActiveTweens();
			print("Moving mesh: " + gameObject.name + " Down. PLayers colour is: " + LevelController.Instance.PlayerColour);
			iTween.MoveTo(gameObject, iTween.Hash("position", Vector3.zero, "time", 0.5f, "oncomplete", "DownComplete"));
			isMovingDown = true;
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
