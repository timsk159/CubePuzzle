using UnityEngine;
using System;
using System.IO;
using System.Collections;

public enum FrontMenuUINotification
{
	StoryModeButtonPressed, UserLevelsButtonPressed, LevelCreatorButtonPressed, QuitButtonPressed,
	PlayUserLevelButtonPressed, CancelUserLevelMenuPressed,
	StoryModeContinueButtonPressed, StoryModeLevelButtonPressed, StoryModeCancelButtonPressed
};

public class FrontMenu : MonoBehaviour 
{
	static bool firstLoad = true;

	public GameObject frontMenuPanel;
	public GameObject loadMapPanel;
	public GameObject storyModePanel;
	
	public GameObject fileMenuEntryPrefab;

	public GameObject fileMenuGrid;

	LevelCreator levelCreator;

	public string selectedFileName;
	private GameObject storyModeContinueButton;
	
	void Start ()
	{
		levelCreator = GetComponent<LevelCreator> ();
		NotificationCenter<FrontMenuUINotification>.DefaultCenter.AddObserver(this, FrontMenuUINotification.StoryModeButtonPressed);
		NotificationCenter<FrontMenuUINotification>.DefaultCenter.AddObserver(this, FrontMenuUINotification.QuitButtonPressed);
		NotificationCenter<FrontMenuUINotification>.DefaultCenter.AddObserver(this, FrontMenuUINotification.LevelCreatorButtonPressed);
		NotificationCenter<FrontMenuUINotification>.DefaultCenter.AddObserver(this, FrontMenuUINotification.UserLevelsButtonPressed);
		NotificationCenter<FrontMenuUINotification>.DefaultCenter.AddObserver(this, FrontMenuUINotification.PlayUserLevelButtonPressed);
		NotificationCenter<FrontMenuUINotification>.DefaultCenter.AddObserver (this, FrontMenuUINotification.CancelUserLevelMenuPressed);
		NotificationCenter<FrontMenuUINotification>.DefaultCenter.AddObserver(this, FrontMenuUINotification.StoryModeContinueButtonPressed);
		NotificationCenter<FrontMenuUINotification>.DefaultCenter.AddObserver(this, FrontMenuUINotification.StoryModeLevelButtonPressed);
		NotificationCenter<FrontMenuUINotification>.DefaultCenter.AddObserver(this, FrontMenuUINotification.StoryModeCancelButtonPressed);

		if(firstLoad)
		{
			EnsureDirectoriesExist();
			RegisterPrefabPaths ();
			firstLoad = false;
		}
	}
	
	void StoryModeButtonPressed()
	{
		NGUITools.SetActive(frontMenuPanel, false);
		NGUITools.SetActive(storyModePanel, true);

		if(storyModeContinueButton == null)
		{
			storyModeContinueButton = storyModePanel.transform.Find("StoryModeContinueButton").gameObject;
		}
		if(StoryProgressController.Instance.GetStoryProgressSave() == null && StoryProgressController.Instance.SavedLevel == null)
		{
			storyModeContinueButton.SetActive(false);
		}
		else
		{
			print(StoryProgressController.Instance.SavedLevel.levelName);

			storyModeContinueButton.SetActive(true);
		}
		PopulateStoryModePanel();
	}
	
	void LevelCreatorButtonPressed()
	{
		SceneLoader.Instance.LoadLevel("LevelCreator");
	}
	
	void UserLevelsButtonPressed()
	{
		NGUITools.SetActive(frontMenuPanel, false);
		NGUITools.SetActive(loadMapPanel, true);
		
		PopulateFileMenu();
	}
	
	void PopulateFileMenu()
	{
		if(fileMenuGrid.transform.childCount > 0)
		{
			foreach(Transform child in fileMenuGrid.transform)
			{
				Destroy (child.gameObject);
			}
		}
		
		string[] fileNames = Directory.GetFiles(Application.persistentDataPath + LevelCreatorController.mapFilesFilePath);
		
		foreach(var file in fileNames)
		{
			if(file.EndsWith(".mp"))
			{
				var fileListEntry = NGUITools.AddChild(fileMenuGrid, fileMenuEntryPrefab);
				var fileListCheckBox = fileListEntry.GetComponent<FileListCheckbox>();
			
				//Remove file extension
				var fileNameSplit = file.Split(new string[] { ".mp" }, StringSplitOptions.RemoveEmptyEntries);
				var fileName = fileNameSplit[0]; 
			
				//Set the label to the file name (minus any directorys in the path)
				fileListEntry.GetComponentInChildren<UILabel>().text = fileName.Substring(fileName.LastIndexOf("/") + 1);
				fileListCheckBox.onSelectionChanged = FileListSelectionChanged;
				//Remove the application.persistantdatapath as the level loader doesn't want it.
				fileListCheckBox.fullFilePath = file.Substring(file.IndexOf(Application.persistentDataPath) + Application.persistentDataPath.Length);
				fileListCheckBox.radioButtonRoot = fileListCheckBox.transform.parent;
			}
		}
		fileMenuGrid.GetComponent<UIGrid>().Reposition();
	}
	
	void FileListSelectionChanged(bool isChecked, string fileName)
	{
		if(isChecked)
		{
			selectedFileName = fileName;
		}
		else
		{
			selectedFileName = string.Empty;
		}
	}
	
	void PlayUserLevelButtonPressed()
	{
		if(!string.IsNullOrEmpty(selectedFileName))
			levelCreator.LoadMapForPlayMode (selectedFileName);
	}

	void CancelUserLevelMenuPressed()
	{
		NGUITools.SetActive(frontMenuPanel, true);
		NGUITools.SetActive(loadMapPanel, false);
		selectedFileName = "";
	}

	void QuitButtonPressed()
	{
		PlayerPrefs.DeleteAll(); 
		Application.Quit();
	}

	void StoryModeCancelButtonPressed()
	{
		var levelsGrid = storyModePanel.transform.Find ("LevelListDragPanel/LevelListGrid");
		foreach(Transform child in levelsGrid)
		{
			if(child != levelsGrid)
				Destroy(child.gameObject);
			GameObject.Find ("SelectLevelButton").GetComponent<FrontMenuUINotifier> ().payload = "";
		}
		NGUITools.SetActive(storyModePanel, false);
		NGUITools.SetActive(frontMenuPanel, true);
	}

	void StoryModeLevelButtonPressed(NotificationCenter<FrontMenuUINotification>.Notification notiData)
	{
		string selectedLevel = (string)notiData.data;

		if(!string.IsNullOrEmpty(selectedLevel))
			levelCreator.LoadStoryLevel(selectedLevel);
	}

	void StoryModeContinueButtonPressed()
	{
		//Make sure we didn't click so quick that the playerprefs didn't save.
		PlayerPrefs.Save();
		//If user pressed quit instead of next level the last time they completed a level, the story progress save will be different from the saved level.
		//If this is the case, we just load the saved level, not the progress save. This is so we can still use the progress save for checkpoints.
		var storyProgressSave = StoryProgressController.Instance.GetStoryProgressSave();

		if(storyProgressSave.Level != StoryProgressController.Instance.SavedLevel.levelName)
		{
			SceneLoader.Instance.LoadLevel(StoryProgressController.Instance.SavedLevel.levelName, delegate
			{
				LevelController.Instance.InitLevel(true);
			});
		}
		else
		{
			LevelSerializer.LoadSavedLevel(storyProgressSave.Data, delegate
			{
				LevelController.Instance.InitLevel(false);

			});
		}
	}

	void PopulateStoryModePanel()
	{
		var levelsGrid = storyModePanel.transform.Find("LevelListDragPanel/LevelListGrid").gameObject;

		if(StoryProgressController.Instance.SavedLevel == null)
		{
			AddLevelLabel(levelsGrid, StoryProgressController.Instance.AllLevels[0]);
		}
		else
		{
			var currentLevelNumber = StoryProgressController.Instance.SavedLevel.levelNumber;

			for(int i = 1; i <= currentLevelNumber; i++)
			{
				AddLevelLabel(levelsGrid, StoryProgressController.Instance.AllLevels[i - 1]);
			}
		}
	}

	void AddLevelLabel(GameObject grid, StoryLevel level)
	{
		var prefab = (GameObject)Resources.Load("StoryModeListEntry");
		var clone = NGUITools.AddChild(grid, prefab);

		var label = clone.GetComponentInChildren<UILabel>();

		label.transform.localScale = new Vector3(30, 30, 1);

		label.text = level.displayName;

		NGUITools.AddWidgetCollider(label.gameObject);

		grid.GetComponent<UIGrid>().Reposition();

		var checkbox = clone.GetComponent<ServerListCheckbox>();
		checkbox.gameObject.AddComponent<UIDragPanelContents>();
		checkbox.radioButtonRoot = grid.transform;

		checkbox.onSelectionChanged = StoryModeLevelListSelectionChanged;
		
		checkbox.levelName = level.levelName;
	}

	void StoryModeLevelListSelectionChanged(bool state, string levelName)
	{
		if(state)
			GameObject.Find("SelectLevelButton").GetComponent<FrontMenuUINotifier>().payload = levelName;
		else
			GameObject.Find("SelectLevelButton").GetComponent<FrontMenuUINotifier>().payload = "";
	}

	void EnsureDirectoriesExist()
	{
		if(!Directory.Exists(Application.persistentDataPath + LevelCreatorController.mapFilesFilePath))
		{
			Directory.CreateDirectory(Application.persistentDataPath + LevelCreatorController.mapFilesFilePath);
		}
	}

	void RegisterPrefabPaths()
	{
		LevelSerializer.useCompression = true;

		string basePath = "LevelObjects/";
		LevelSerializer.AddPrefabPath (basePath + "BlueCube");
		LevelSerializer.AddPrefabPath (basePath + "ButtonCube");
		LevelSerializer.AddPrefabPath (basePath + "DoorCube");
		LevelSerializer.AddPrefabPath (basePath + "EndGameCube");
		LevelSerializer.AddPrefabPath (basePath + "GreenCube");
		LevelSerializer.AddPrefabPath (basePath + "LeverCube");
		LevelSerializer.AddPrefabPath (basePath + "NeutralCube");
		LevelSerializer.AddPrefabPath (basePath + "NullCube");
		LevelSerializer.AddPrefabPath (basePath + "PlayerButtonCube");
		LevelSerializer.AddPrefabPath (basePath + "PlayerStartCube");
		LevelSerializer.AddPrefabPath (basePath + "PurpleCube");
		LevelSerializer.AddPrefabPath (basePath + "RedCube");
		LevelSerializer.AddPrefabPath(basePath + "CheckpointCube");
		LevelSerializer.AddPrefabPath("Player");
	}
}
