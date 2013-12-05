using UnityEngine;
using System;
using System.Collections;

public class OptionsMenu : MonoBehaviour 
{
	public UICheckbox fullScreenToggle;
	public UIPopupList qualityLevelSelectionList;
	public UIPopupList colorBlindSelectionList;

	public class Options
	{
		public bool fullscreen;
		public string qualityLevel;
		public ColorBlindMode colorBlindMode;
	}

	public static Options currentOptions;

	void Start()
	{
		Messenger.AddListener(OptionsMenuMessage.Back.ToString(), Back);
		Messenger.AddListener(OptionsMenuMessage.Apply.ToString(), Apply);
		ApplyChangesToUI();
	}

	void ApplyChangesToUI()
	{
		fullScreenToggle.isChecked = currentOptions.fullscreen;
		qualityLevelSelectionList.selection = currentOptions.qualityLevel;
		colorBlindSelectionList.selection = currentOptions.colorBlindMode.ToString();
	}

	void Back()
	{

	}

	void Apply()
	{
		var newOptions = new Options();

		newOptions.fullscreen = fullScreenToggle.isChecked;

		newOptions.qualityLevel = qualityLevelSelectionList.selection;
		newOptions.colorBlindMode = (ColorBlindMode)Enum.Parse(typeof(ColorBlindMode), colorBlindSelectionList.selection, true);

		ApplyChanges(newOptions);

		SaveOptions();

		ApplyChangesToUI();
	}

	void ApplyChanges(Options newOptions)
	{
		currentOptions = newOptions;

		if(currentOptions.fullscreen != Screen.fullScreen)
		{
			Screen.fullScreen = currentOptions.fullscreen;
		}
		if(currentOptions.qualityLevel != QualitySettings.names[QualitySettings.GetQualityLevel()])
		{
			QualitySettings.SetQualityLevel(QualitySettings.names.IndexOf(currentOptions.qualityLevel), true);
		}
		if(currentOptions.colorBlindMode != ColorManager.currentColorBlindMode)
		{
			ColorManager.currentColorBlindMode = currentOptions.colorBlindMode;
		}
	}

	public void LoadOptions()
	{
		var newOptions = new Options();

		newOptions.qualityLevel = PlayerPrefs.GetString("QualityLevel", "Best");
		newOptions.colorBlindMode = (ColorBlindMode)PlayerPrefs.GetInt("ColorBlindMode", 0);
		newOptions.fullscreen = IntToBool(PlayerPrefs.GetInt("FullScreen", 1));

		ApplyChanges(newOptions);
	}

	public void SaveOptions()
	{
		PlayerPrefs.SetString("QualityLevel", currentOptions.qualityLevel);
		PlayerPrefs.SetInt("FullScreen", BoolToInt(currentOptions.fullscreen));
		PlayerPrefs.SetInt("ColorBlindMode", (int)currentOptions.colorBlindMode);
		PlayerPrefs.Save();
	}

	public int BoolToInt(bool val)
	{
		if(val)
			return 1;
		else
			return 0;
	}

	public bool IntToBool(int val)
	{
		if(val == 1)
			return true;
		else if(val == 0)
			return false;
		else
			throw new ArgumentException("Invalid number when converting from integer to bool. Accepted values are 1 or 0", "val");
	}
}
