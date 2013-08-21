using UnityEngine;
using System;
using System.IO;
using System.Collections;

public enum FrontMenuUINotification
{
	StoryModeButtonPressed, UserLevelsButtonPressed, LevelCreatorButtonPressed, QuitButtonPressed,
	PlayUserLevelButtonPressed, CancelUserLevelMenuPressed
};

public class FrontMenu : MonoBehaviour 
{
	static bool firstLoad = true;

	public GameObject frontMenuPanel;
	public GameObject loadMapPanel;
	
	public GameObject fileMenuEntryPrefab;

	public GameObject fileMenuGrid;

	LevelCreator levelCreator;

	public string selectedFileName;
	
	void Start ()
	{
		levelCreator = GetComponent<LevelCreator> ();
		NotificationCenter<FrontMenuUINotification>.DefaultCenter.AddObserver(this, FrontMenuUINotification.StoryModeButtonPressed);
		NotificationCenter<FrontMenuUINotification>.DefaultCenter.AddObserver(this, FrontMenuUINotification.QuitButtonPressed);
		NotificationCenter<FrontMenuUINotification>.DefaultCenter.AddObserver(this, FrontMenuUINotification.LevelCreatorButtonPressed);
		NotificationCenter<FrontMenuUINotification>.DefaultCenter.AddObserver(this, FrontMenuUINotification.UserLevelsButtonPressed);
		NotificationCenter<FrontMenuUINotification>.DefaultCenter.AddObserver(this, FrontMenuUINotification.PlayUserLevelButtonPressed);
		NotificationCenter<FrontMenuUINotification>.DefaultCenter.AddObserver (this, FrontMenuUINotification.CancelUserLevelMenuPressed);
		if(firstLoad)
		{
			RegisterPrefabPaths ();
			firstLoad = false;
		}
	}
	
	void StoryModeButtonPressed()
	{
		Application.LoadLevel ("TestScene");
	}
	
	void LevelCreatorButtonPressed()
	{
		Application.LoadLevel("LevelCreator");
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
		levelCreator.LoadMapForPlayMode (selectedFileName);
	}

	void CancelUserLevelMenuPressed()
	{
		NGUITools.SetActive(frontMenuPanel, true);
		NGUITools.SetActive(loadMapPanel, false);
	}

	void QuitButtonPressed()
	{
		Application.Quit();
	}


	void RegisterPrefabPaths()
	{
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
