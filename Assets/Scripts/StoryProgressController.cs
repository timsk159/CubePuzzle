using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//Scene Naming:
//"T-01" Tutorial one
//"01" Story one

public class StoryProgressController : MonoBehaviour 
{
	private static StoryProgressController _instance;

	public static StoryProgressController Instance 
	{
		get
		{
			if(_instance == null)
			{
				if(GameObject.Find("StoryProgressController") == null)
				{
					_instance = new GameObject("StoryProgressController").AddComponent<StoryProgressController>();
				}
				else
				{
					_instance = GameObject.Find ("StoryProgressController").GetComponent<StoryProgressController> ();
				}

				if(_instance == null)
				{
					_instance = new GameObject ("StoryProgressController").AddComponent<StoryProgressController> ();
				}
			}
			return _instance;
		}
	}

	private bool _hasCompletedTutorial;
	private bool _isInTutorial;
	private int _levelNumber;
	private string _savedLevelName;
	private int _savedLevelNumber;


	public bool HasCompletedTutorial
	{
		get
		{
			var _hasCompletedTutorialInt = PlayerPrefs.GetInt("TutorialFinished", 0);
			if(_hasCompletedTutorialInt == 1)
				_hasCompletedTutorial = true;
			else
				_hasCompletedTutorial = false;


			return _hasCompletedTutorial;
		}
		set
		{
			if(value)
			{
				PlayerPrefs.SetInt("TutorialFinished", 1);
			}
			else
			{
				PlayerPrefs.SetInt("TutorialFinished", 0);
			}
			_hasCompletedTutorial = value;
		}
	}

	public bool IsInTutorial
	{
		get
		{
			if(Application.loadedLevelName.StartsWith("T"))
			{
				_isInTutorial = true;
			}
			else
			{
				_isInTutorial = false;
			}
			return _isInTutorial;
		}
	}

	public int LevelNumber
	{
		get
		{
			string levelName = "";
			if(IsInTutorial)
			{
				levelName = Application.loadedLevelName.Replace("T-", "");
			}
			else
			{
				levelName = Application.loadedLevelName;
			}
			_levelNumber = int.Parse(levelName);
			return _levelNumber;
		}
	}

	public string SavedLevelName
	{
		get
		{
			_savedLevelName = PlayerPrefs.GetString("SavedLevelName", "T-01");
			return _savedLevelName;
		}
		set
		{
			_savedLevelName = value;
			PlayerPrefs.SetString("SavedLevelName", value);
		}
	}

	public int SavedLevelNumber
	{
		get
		{
			if(SavedLevelName.StartsWith("T"))
			{
				var trimmedName = SavedLevelName.Replace("T-", "");
				_savedLevelNumber = int.Parse(trimmedName);
			}
			else
			{
				_savedLevelNumber = int.Parse(SavedLevelName);
			}
			return _savedLevelNumber;
		}
	}

	public string NextLevelName()
	{
		var currentLevelNumber = LevelNumber + 1;
		if(IsInTutorial)
		{
			var levelNumberString = currentLevelNumber.ToString("00");
			return ("T-" + levelNumberString);
		}
		else
		{
			var levelNumberString = currentLevelNumber.ToString("00");
			return levelNumberString;
		}
	}

	public string GetStoryProgressSave()
	{
		var savedGames = LevelSerializer.SavedGames[LevelSerializer.PlayerName];

		if(savedGames == null)
			return "";

		var initialSaveData = savedGames.Where(e => e.Name == "ProgressSave").FirstOrDefault();

		if(initialSaveData == null)
			return "";

		return initialSaveData.Data;
	}

	public void SetStoryProgressSave()
	{
		SavedLevelName = Application.loadedLevelName;
		LevelSerializer.SaveGame("ProgressSave");
	}
}
