using UnityEngine;
using System.Collections;

public class LevelCreatorDashboard : MonoBehaviour 
{
	TweenPosition tween;

	void OnHover(bool isOver)
	{
		if(gameObject.name == "LeftDashboard")
		{
			if(isOver)
			{
				Messenger<bool>.Invoke(LevelCreatorUIMessage.SideHoverEnter.ToString(), true);
				TweenIn();
			}
			else
			{
				Messenger<bool>.Invoke(LevelCreatorUIMessage.SideHoverExit.ToString(), true);
				TweenOut();
			}
		}
		else if(gameObject.name == "RightDashboard")
		{
			if(isOver)
			{
				Messenger<bool>.Invoke(LevelCreatorUIMessage.SideHoverEnter.ToString(), false);
				TweenIn();
			}
			else
			{
				Messenger<bool>.Invoke(LevelCreatorUIMessage.SideHoverExit.ToString(), false);
				TweenOut();
			}
		}
	}

	void TweenIn()
	{
		tween.Play(true);

	}

	void TweenOut()
	{
		tween.Play(false);

	}
}
