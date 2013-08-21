using UnityEngine;
using System.Collections;

public class DragAndDropButtonMenuItem : DragAndDropMenuItem 
{
	protected override void OnPress(bool isPressed)
	{
		if(isPressed)
		{			
			base.OnPress (isPressed);
		}
	}
}
