using UnityEngine;
using UnityEditor;
using System.Collections;

public enum ButtonState
{
	Normal, Hover, Pressed, Disabled
}

public class ChangeButtonColour : EditorWindow 
{
	string spriteName;
	ButtonState stateToChange;
	UIAtlas atlasMask;

	[MenuItem("Window/ImageButton Changer")]
	static void Init()
	{
		var window = GetWindow<ChangeButtonColour>("ImageButton Changer");


	}


	void OnGUI()
	{
		spriteName = EditorGUILayout.TextField("Sprite Name: ", spriteName);
		stateToChange = (ButtonState)EditorGUILayout.EnumPopup("State To Change: ", stateToChange);
		atlasMask = (UIAtlas)EditorGUILayout.ObjectField("Atlas Mask: ", atlasMask, typeof(UIAtlas), false);

		if(GUILayout.Button("Execute"))
		{
			if(!string.IsNullOrEmpty(spriteName) && atlasMask != null)
			{
				var buttons = GetImageButtons();
				Debug.Log("Found: " + buttons.Length + " Buttons");
				foreach(var button in buttons)
				{
					if(button.target.atlas == atlasMask)
					{
						switch(stateToChange)
						{
							case ButtonState.Normal:

								button.normalSprite = spriteName;
								break;
							case ButtonState.Hover:
								button.hoverSprite = spriteName;
								break;
							case ButtonState.Disabled:
								button.disabledSprite = spriteName;
								break;
							case ButtonState.Pressed:
								button.pressedSprite = spriteName;
								break;
						}
					}
				}
			}
		}
	}

	UIImageButton[] GetImageButtons()
	{
		//var returnArray = FindObjectsOfType<UIImageButton>();
		var returnArray = Resources.FindObjectsOfTypeAll<UIImageButton>();
		return returnArray;
	}
}
