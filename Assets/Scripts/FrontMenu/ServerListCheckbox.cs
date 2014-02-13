using UnityEngine;
using System.Collections;

public class ServerListCheckbox : UICheckbox
{
	public delegate void OnSelectionChanged(bool state, string levelName);
	
	public OnSelectionChanged onSelectionChanged;
	
	public string levelName = "";

	string originalSprite;
	public string hoverSprite;

	protected override void Start()
	{
		originalSprite = checkSprite.spriteName;
		base.Start();
		this.onStateChange = HandleStateChanged;
	}
	
	void HandleStateChanged(bool state)
	{
		if(state)
		{
			checkSprite.spriteName = originalSprite;
		}
		if(onSelectionChanged != null && levelName != string.Empty)
		{
			onSelectionChanged(state, levelName);
		}
	}

	void OnHover(bool isOver)
	{
		if(!string.IsNullOrEmpty(hoverSprite))
		{
			if(isOver && !isChecked)
			{
				checkSprite.spriteName = hoverSprite;

				if (instantTween)
				{
					checkSprite.alpha = 1f;
				}
				else
				{
					TweenAlpha.Begin(checkSprite.gameObject, 0.15f, 1f);
				}

				//checkSprite.gameObject.SetActive(true);
			}
			else if(!isOver && !isChecked)
			{
				//checkSprite.gameObject.SetActive(false);

				if (checkSprite != null)
				{
					if (instantTween)
					{
						checkSprite.alpha = 0f;
					}
					else
					{
						TweenAlpha.Begin(checkSprite.gameObject, 0.15f, 0f);
					}
				}
			}
		}
	}
}
