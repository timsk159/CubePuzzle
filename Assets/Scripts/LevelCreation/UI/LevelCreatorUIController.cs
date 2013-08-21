using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class LevelCreatorUIController : MonoBehaviour 
{
	public GameObject frontMenuPanel;
	public GameObject levelCreationMenuPanel;
	public GameObject levelAssetMenuPanel;
	public GameObject savingMapPanel;
	public GameObject loadingMapPanel;

	public GameObject loadingProgressPanel;
	public GameObject savingProgressPanel;

	public UISlider loadingProgressBar;
	public UISlider savingProgressBar;

	public UILabel saveErrorLabel;

	public GameObject fileMenuEntryPrefab;
	
	UIPopupList fileMenu;	
	LevelCreator levelCreator;
	
	int selectedMapSize;
	bool isInFrontMenu = true;
	bool isSavingMap = false;
	
	string selectedFileName;

	static bool cameFromPreview;

	void Start()
	{
		fileMenu = GameObject.Find("FileMenu").GetComponent<UIPopupList>();
		levelCreator = GameObject.Find("LevelCreator").GetComponent<LevelCreator>();
		RegisterStateListeners();
		RegisterUIListeners();
	}
	
	#region State Changes
	void RegisterStateListeners()
	{
		StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.FrontMenuEnter);
		StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.FrontMenuExit);
		
		StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.LevelCreationEnter);
		StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.LevelCreationExit);
		
		StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.SavingMapEnter);
		StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.SavingMapExit);
		
		StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.LoadingMapEnter);
		StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.LoadingMapExit);

		StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.TestingMapEnter);
		StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.StateNotificationCenter.AddObserver(this, LevelCreatorStateNotification.TestingMapExit);
	}
	
	void FrontMenuEnter()
	{
		isInFrontMenu = true;
		if(!frontMenuPanel.activeSelf)
		{
			NGUITools.SetActive(frontMenuPanel, true);
		}
	}
	
	void FrontMenuExit()
	{
		if(frontMenuPanel.activeSelf)
		{
			NGUITools.SetActive(frontMenuPanel, false);
		}
	}
	
	void LevelCreationEnter()
	{
		Debug.Log ("Level Creation enter!");
		isInFrontMenu = false;

		if(!fileMenu.items.Contains("Save"))
			fileMenu.items.Insert(1,"Save");
		if(!fileMenu.items.Contains("TestMap"))
			fileMenu.items.Insert(2, "TestMap");

		if(cameFromPreview)
		{
			fileMenu.items.Clear();
			fileMenu.items.Insert(0, "Open");
			fileMenu.items.Insert(1, "Save");
			fileMenu.items.Insert(2, "Exit");
			cameFromPreview = false;
		}

		NGUITools.SetActive(levelAssetMenuPanel, true);
	}
	
	void LevelCreationExit()
	{
		if (fileMenu.items.Contains("Save"))
			fileMenu.items.Remove("Save");
		if(fileMenu.items.Contains("TestMap"))
			fileMenu.items.Remove("TestMap");

		NGUITools.SetActive(levelAssetMenuPanel, false);
	}
	
	void SavingMapEnter()
	{
		NGUITools.SetActive(savingMapPanel, true);
		isSavingMap = true;
		var fileMenuGrid = savingMapPanel.transform.Find("FileListScrollablePanel/FileListGrid").gameObject;
		StartCoroutine(PopulateFileMenu(fileMenuGrid));
	}
	
	void SavingMapExit()
	{
		isSavingMap = false;
		TurnOffLoadingBar ();
		NGUITools.SetActive(savingMapPanel, false);	
	}
	
	void LoadingMapEnter()
	{
		NGUITools.SetActive(loadingMapPanel, true);
		var fileMenuGrid = loadingMapPanel.transform.Find("FileListScrollablePanel/FileListGrid").gameObject;
		StartCoroutine(PopulateFileMenu(fileMenuGrid));
	}
	
	void LoadingMapExit()
	{
		NGUITools.SetActive(loadingMapPanel, false);
	}

	void TestingMapEnter()
	{
		cameFromPreview = true;
		var fileMenuList = fileMenu.GetComponent<UIPopupList>();
		fileMenuList.items.Clear();
		fileMenuList.items.Add("StopTesting");
	}

	void TestingMapExit()
	{

	}
	
	#endregion
	
	#region UI Response
	
	void RegisterUIListeners()
	{
		NotificationCenter<LevelCreatorUINotification>.DefaultCenter.AddObserver(this, LevelCreatorUINotification.CreateButtonClicked);
		NotificationCenter<LevelCreatorUINotification>.DefaultCenter.AddObserver(this, LevelCreatorUINotification.GenericInputSubmitted);
		
		NotificationCenter<LevelCreatorUINotification>.DefaultCenter.AddObserver(this, LevelCreatorUINotification.LoadMenuCancelClicked);
		NotificationCenter<LevelCreatorUINotification>.DefaultCenter.AddObserver(this, LevelCreatorUINotification.LoadMenuLoadClicked);
		
		NotificationCenter<LevelCreatorUINotification>.DefaultCenter.AddObserver(this, LevelCreatorUINotification.SaveMenuCancelClicked);
		NotificationCenter<LevelCreatorUINotification>.DefaultCenter.AddObserver(this, LevelCreatorUINotification.SaveMenuSaveClicked);
		
		fileMenu.GetComponent<UIPopupList>().onSelectionChange = FileMenuSelectionChanged;
	}
	
	void CreateButtonClicked()
	{
		var mapsizeinput = GameObject.Find ("MapSize-Input").GetComponentInChildren<UILabel> ();

		if (int.TryParse (mapsizeinput.text, out selectedMapSize))
		{
			for (int x = 0; x <= selectedMapSize; x++)
			{
				for (int z = 0; z <= selectedMapSize; z++)
				{
					var obj = (GameObject)Instantiate (levelCreator.assetManager.nullCubePrefab);

					obj.transform.position = new Vector3 (x, 0, z);

					obj.transform.parent = levelCreator.mapRoot.transform;
				
				}
			}
			levelCreator.mapRoot.GetComponent<MapRoot>().mapSize = selectedMapSize;
			StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.ChangeState (LevelCreatorStates.LevelCreation);
		}
	}
	
	void GenericInputSubmitted(NotificationCenter<LevelCreatorUINotification>.Notification notiData)
	{
		InputNotificationData inputData = (InputNotificationData)notiData.data;
		
		if(inputData.go.name == "MapSize-Input")
		{
			selectedMapSize = Convert.ToInt32(inputData.theInput);
		}
		else if(inputData.go.name == "FileNameInput")
		{
			if(!string.IsNullOrEmpty(selectedFileName))
			{
				if(selectedFileName.Contains(Application.persistentDataPath))
					selectedFileName = inputData.theInput;
			}
			else
				selectedFileName = LevelCreatorController.mapFilesFilePath + inputData.theInput + ".mpcr";
		}
	}
	
	void FileMenuSelectionChanged (string item)
	{
		switch(item)
		{
			case "Open":
				StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.ChangeState(LevelCreatorStates.LoadingMap);
				break;
			case "TestMap":
				var errorMessage = "";
				if(levelCreator.MapIsComplete(out errorMessage))
				{
					LevelSerializer.SaveGame("BeforePreviewSave");

					StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.ChangeState(LevelCreatorStates.TestingMap);
				}
				else
				{
					DisplayErrorMessage(errorMessage);
				}
				break;
			case "StopTesting":

				StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.ChangeState(LevelCreatorStates.LevelCreation);
				break;
			case "Save":
				StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.ChangeState(LevelCreatorStates.SavingMap);
				break;
			case "Exit":
				if(fileMenu.GetComponent<UIPopupList>().items.Contains("Save"))
				{
					StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.ChangeState(LevelCreatorStates.FrontMenu);
				}
				else
				{
					Application.LoadLevel("FrontMenu");
				}
				
				break;
		}
	}
	
	
	void LoadMenuCancelClicked()
	{
		if(isInFrontMenu)
			StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.ChangeState(LevelCreatorStates.FrontMenu);
		else
			StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.ChangeState(LevelCreatorStates.LevelCreation);
	}
	
	void LoadMenuLoadClicked()
	{
		if(!string.IsNullOrEmpty(selectedFileName))
		{
			NGUITools.SetActive (loadingProgressPanel, true);
			levelCreator.LoadMap(selectedFileName);
			selectedFileName = "";
		}
	}
	
	void SaveMenuCancelClicked()
	{
		if(isInFrontMenu)
			StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.ChangeState(LevelCreatorStates.FrontMenu);
		else
			StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.ChangeState(LevelCreatorStates.LevelCreation);
	}
	
	void SaveMenuSaveClicked()
	{
		var fileNameInputText = GameObject.Find ("FileNameInput").GetComponentInChildren<UILabel> ().text.Replace("|","");

		selectedFileName = LevelCreatorController.mapFilesFilePath + fileNameInputText + ".mpcr";

		if(!string.IsNullOrEmpty(selectedFileName))
		{
			string errorMessage = "";
			NGUITools.SetActive (savingProgressPanel, true);
			levelCreator.SaveMap(selectedFileName, out errorMessage);
			if (errorMessage != "")
			{
				DisplayErrorMessage (errorMessage);
				TurnOffLoadingBar ();
			}
			else
			{
				selectedFileName = null;
				StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.ChangeState (LevelCreatorStates.LevelCreation);
			}
		}
	}

	void DisplayErrorMessage(string errorMessage)
	{
		if(!saveErrorLabel.gameObject.activeSelf)
		{
			NGUITools.SetActiveSelf(saveErrorLabel.gameObject, true);
		}
		saveErrorLabel.text = errorMessage;

		saveErrorLabel.GetComponent<TweenAlpha> ().onFinished += ErrorLabelForwardFinished;

		saveErrorLabel.GetComponent<TweenAlpha> ().Play (true);
	}

	void ErrorLabelForwardFinished(UITweener tweener)
	{
		StartCoroutine (ErrorLabelForwardFinishedRoutine (tweener));
	}

	void ErrorLabelBackwardFinished(UITweener tweener)
	{
		tweener.onFinished -= ErrorLabelBackwardFinished;
		NGUITools.SetActive(saveErrorLabel.gameObject, false);
	}

	IEnumerator ErrorLabelForwardFinishedRoutine(UITweener tweener)
	{
		yield return new WaitForSeconds (1.5f);
		tweener.onFinished -= ErrorLabelForwardFinished;
		tweener.onFinished += ErrorLabelBackwardFinished;
		tweener.Play (false);
	}
	
	
	void FileListSelectionChanged(bool isChecked, string fileName)
	{
		if(isChecked)
		{
			if(isSavingMap)
			{
				//Remove file extension
				var fileNameSplit = fileName.Split (new string[] {".mpcr"}, StringSplitOptions.RemoveEmptyEntries);
				var file = fileNameSplit [0]; 

				GameObject.Find("FileNameInput").GetComponentInChildren<UILabel>().text = file.Substring(file.LastIndexOf("/") + 1);
			}
			selectedFileName = fileName;
		}
		else
		{
			selectedFileName = string.Empty;
		}
	}
	
	#endregion

	public void TurnOffLoadingBar()
	{
		NGUITools.SetActive (loadingProgressPanel, false);
		NGUITools.SetActive (savingProgressPanel, false);
	}

	public void UpdateProgressBar(bool isSaving, float progress)
	{
		if(isSaving)
		{
			savingProgressBar.sliderValue = progress;
		}
		else
		{
			loadingProgressBar.sliderValue = progress;
		}
	}

	IEnumerator PopulateFileMenu(GameObject fileMenuGrid)
	{
		yield return StartCoroutine(ClearFileMenu(fileMenuGrid));
		
		string[] fileNames = Directory.GetFiles(Application.persistentDataPath + LevelCreatorController.mapFilesFilePath);
		
		foreach(var file in fileNames)
		{
			if(file.EndsWith(".mpcr"))
			{
				var fileListEntry = NGUITools.AddChild(fileMenuGrid, fileMenuEntryPrefab);
				var fileListCheckBox = fileListEntry.GetComponent<FileListCheckbox>();
			
				//Remove file extension
				var fileNameSplit = file.Split(new string[] { ".mpcr" }, StringSplitOptions.RemoveEmptyEntries);
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
	
	IEnumerator ClearFileMenu(GameObject fileMenuGrid)
	{
		foreach(Transform child in fileMenuGrid.transform)
		{
			Destroy(child.gameObject);
		}
		yield return new WaitForEndOfFrame ();
		fileMenuGrid.GetComponent<UIGrid>().Reposition();
		yield return new WaitForEndOfFrame ();
	}
}