using UnityEngine;
using System.Collections;

public class ServerListCheckbox : UICheckbox
{
	public delegate void OnSelectionChanged(bool state, string levelName);
	
	public OnSelectionChanged onSelectionChanged;
	
	public string levelName = "";
	
	protected override void Start()
	{
		base.Start();
		this.onStateChange = HandleStateChanged;
	}
	
	void HandleStateChanged(bool state)
	{
		if(onSelectionChanged != null && levelName != string.Empty)
		{
			onSelectionChanged(state, levelName);
		}
	}
}
