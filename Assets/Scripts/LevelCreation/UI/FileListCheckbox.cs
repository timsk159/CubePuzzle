using UnityEngine;
using System.Collections;

public class FileListCheckbox : UICheckbox
{
	public delegate void OnSelectionChanged(bool state, string fileName);
	
	public OnSelectionChanged onSelectionChanged;
	
	public string fullFilePath = "";
	
	protected override void Start()
	{
		base.Start();
		this.onStateChange = HandleStateChanged;
	}
	
	void HandleStateChanged(bool state)
	{
		if(onSelectionChanged != null && fullFilePath != string.Empty)
		{
			onSelectionChanged(state, fullFilePath);
		}
	}
}
