using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using JsonFx;

//Scene Naming:
//"T-01" Tutorial one
//"01" Story one

public class StoryProgressController : MonoSingleton<StoryProgressController>
{
	static List<StoryLevel> allLevels;
	StoryLevel currentLevel;
	StoryLevel savedLevel;

	#region Binary File Experiments
	void CreateBinaryFile()
	{
		List<StoryLevel> levels = BuildStoryLevelListForFile();
	}

	List<StoryLevel> BuildStoryLevelListForFile()
	{
		var levels = new List<StoryLevel>(11);

		for(int i = 0; i < levels.Capacity; i++)
		{
			levels[i].levelName = "0" + (i + 1);
			levels[i].levelNumber = (i + 1);
			levels[i].displayName = "Introduction " + (i + 1);
		}

		return levels;
	}

	#endregion

	List<StoryLevel> ParseLevelsIni()
	{
		List<StoryLevel> levelsList = new List<StoryLevel>();
		var iniFile = (TextAsset)Resources.Load("StoryLevels.ini", typeof(TextAsset));
		var levelLines = iniFile.text.Split(new char[]{';'}, System.StringSplitOptions.RemoveEmptyEntries);

		foreach(var levelLine in levelLines)
		{
			levelsList.Add(new StoryLevel(levelLine));
		}
		return levelsList;
	}

	public List<StoryLevel> AllLevels
	{
		get
		{
			if(allLevels == null || allLevels.Count == 0)
			{
				allLevels = ParseLevelsIni();
			}
			return allLevels;
		}
	}

	public StoryLevel CurrentLevel
	{
		get
		{
			currentLevel = allLevels.Where(level => level.levelName == Application.loadedLevelName).FirstOrDefault();
			return currentLevel;
		}
	}

	public StoryLevel SavedLevel
	{
		get
		{
			var savedLevelName = PlayerPrefs.GetString("SavedLevelName");
			if(string.IsNullOrEmpty(savedLevelName))
				return null;
			return AllLevels.Where(level => level.levelName == savedLevelName).FirstOrDefault();
		}
		private set
		{
			PlayerPrefs.SetString("SavedLevelName", value.levelName);
			PlayerPrefs.Save();
		}
	}

	public StoryLevel NextLevel
	{
		get
		{
			var currentIndex = allLevels.IndexOf(CurrentLevel);
			return allLevels[currentIndex + 1];
		}
	}

	public LevelSerializer.SaveEntry GetStoryProgressSave()
	{
		var savedGames = LevelSerializer.SavedGames[LevelSerializer.PlayerName];

		if(savedGames == null)
			return null;

		var initialSaveData = savedGames.Where(e => e.Name == "ProgressSave").FirstOrDefault();

		return initialSaveData;
	}

	public void SetStoryProgressSave()
	{
		SavedLevel = CurrentLevel;

		var previousSave = LevelSerializer.SavedGames[LevelSerializer.PlayerName].Where(e => e.Name == "ProgressSave").FirstOrDefault();

		if(previousSave != null)
		{
			LevelSerializer.SavedGames[LevelSerializer.PlayerName].Remove(previousSave);
		}

		LevelSerializer.SaveGame("ProgressSave");
	}

	public void SetNextLevelAsProgressSave()
	{
		SavedLevel = NextLevel;
	}
}

public class StoryLevel
{
	public int levelNumber;
	public string displayName;
	public string levelName;
	private string introCutSceneObjFilename;
	public CutSceneObj cutSceneObj;

	public StoryLevel(string lineFromIni)
	{
		var splitLine = lineFromIni.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
		levelNumber = (int)int.Parse(splitLine[0]);
		displayName = splitLine[1];
		levelName = splitLine[2];
		if(splitLine.Length > 3)
		{
			if(!string.IsNullOrEmpty(splitLine[3]))
			{
				introCutSceneObjFilename = splitLine[3];
				cutSceneObj = (CutSceneObj)Resources.Load(introCutSceneObjFilename, typeof(CutSceneObj));
				Debug.Log("Loaded cutscene obj from: " + introCutSceneObjFilename);
				Debug.Log("Cutscene obj: " + cutSceneObj);
			}
		}
	}
}
