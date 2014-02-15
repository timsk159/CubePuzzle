using UnityEngine;
using System.Collections;

public class ServerListCheckbox : UIToggle
{
	public delegate void OnSelectionChanged(bool state, string levelName);
	
	public OnSelectionChanged onSelectionChanged;
	
	public string levelName = "";

	string originalSprite;
	public string hoverSprite;
	UISprite castedSprite;

	protected override void Start()
	{
		castedSprite = activeSprite as UISprite;
		originalSprite = castedSprite.spriteName;
		base.Start();
		EventDelegate.Add(onChange, HandleStateChanged);
	}
	
	void HandleStateChanged()
	{
		if(value)
		{
			castedSprite.spriteName = originalSprite;
		}
		if(onSelectionChanged != null && levelName != string.Empty)
		{
			onSelectionChanged(value, levelName);
		}
	}

	void OnHover(bool isOver)
	{
		if(!string.IsNullOrEmpty(hoverSprite))
		{
			if(isOver && !value)
			{
				castedSprite.spriteName = hoverSprite;

				if (instantTween)
				{
					castedSprite.alpha = 1f;
				}
				else
				{
					TweenAlpha.Begin(castedSprite.gameObject, 0.15f, 1f);
				}

				//checkSprite.gameObject.SetActive(true);
			}
			else if(!isOver && !value)
			{
				//checkSprite.gameObject.SetActive(false);

				if (castedSprite != null)
				{
					if (instantTween)
					{
						castedSprite.alpha = 0f;
					}
					else
					{
						TweenAlpha.Begin(castedSprite.gameObject, 0.15f, 0f);
					}
				}
			}
		}
	}
}
