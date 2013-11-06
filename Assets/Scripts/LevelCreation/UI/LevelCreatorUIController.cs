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

	public UISlider loadingProgressBar;

	public UILabel saveErrorLabel;

	public GameObject fileMenuEntryPrefab;
	
	UIPopupList fileMenu;	
	LevelCreator levelCreator;
	
	int selectedMapHeight;
	int selectedMapWidth;
	bool isInFrontMenu = true;
	bool isSavingMap = false;
	bool isLoadingMap = false;
	
	string selectedFileName;

	public static bool cameFromPreview;

	void Awake()
	{
		fileMenu = GameObject.Find("FileMenu").GetComponent<UIPopupList>();
		levelCreator = GameObject.Find("LevelCreator").GetComponent<LevelCreator>();
		RegisterStateListeners();
		RegisterUIListeners();
		LevelSerializer.Progress += HandleProgress;
	}

	void OnDestroy()
	{
		LevelSerializer.Progress -= HandleProgress;
	}

	void OnDeserialized()
	{
		TurnOffLoadingBar();
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
		if(fileMenu.items.Contains("Save"))
			fileMenu.items.Remove("Save");
		if(fileMenu.items.Contains("TestMap"))
			fileMenu.items.Remove("TestMap");
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
		fileMenu = GameObject.Find("FileMenu").GetComponent<UIPopupList>();
		levelCreator = GameObject.Find("LevelCreator").GetComponent<LevelCreator>();

		isInFrontMenu = false;

		NGUITools.SetActive(levelAssetMenuPanel, true);

		if(!fileMenu.items.Contains("Save"))
			fileMenu.items.Insert(1,"Save");
		if(!fileMenu.items.Contains("TestMap"))
			fileMenu.items.Insert(2, "TestMap");
	}

	void LevelCreationExit()
	{
		if(fileMenu.items.Contains("Save"))
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
		isLoadingMap = true;
		NGUITools.SetActive(loadingMapPanel, true);
		var fileMenuGrid = loadingMapPanel.transform.Find("FileListScrollablePanel/FileListGrid").gameObject;
		StartCoroutine(PopulateFileMenu(fileMenuGrid));
	}
	
	void LoadingMapExit()
	{
		isLoadingMap = false;
		NGUITools.SetActive(loadingMapPanel, false);
	}

	void TestingMapEnter()
	{
		cameFromPreview = true;
		var fileMenuList = fileMenu.GetComponent<UIPopupList>();
		fileMenuList.items.Clear();
		fileMenuList.items.Add("StopTesting");
		if(loadingProgressPanel.activeSelf == true)
		{
			loadingProgressPanel.SetActive(false);
		}
	}

	void TestingMapExit()
	{
		fileMenu.items.Clear();
		fileMenu.items.Insert(0, "Open");
		fileMenu.items.Insert(1, "Save");
		fileMenu.items.Insert(2, "TestMap");
		fileMenu.items.Insert(3, "Exit");
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

	void HandleProgress (string arg1, float arg2)
	{
		if(arg1 != "Initializing")
			UpdateProgressBar(arg2);
	}
	
	void CreateButtonClicked()
	{
		//change to get the input class and get it's label from there.
		var mapHeightInput = GameObject.Find("MapHeight-Input").GetComponent<UIInput>().label;
		var mapWidthInput = GameObject.Find("MapWidth-Input").GetComponent<UIInput>().label;

		if (int.TryParse (mapHeightInput.text, out selectedMapHeight) && int.TryParse(mapWidthInput.text, out selectedMapWidth))
		{
			for (int x = 0; x <= selectedMapWidth - 1; x++)
			{
				for (int z = 0; z <= selectedMapHeight - 1; z++)
				{
					var obj = (GameObject)Instantiate (levelCreator.assetManager.nullCubePrefab);

					obj.transform.position = new Vector3 (x, 0, z);

					obj.transform.parent = levelCreator.mapRoot.transform;
				
				}
			}
			StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.ChangeState (LevelCreatorStates.LevelCreation);
		}
	}
	
	void GenericInputSubmitted(NotificationCenter<LevelCreatorUINotification>.Notification notiData)
	{
		InputNotificationData inputData = (InputNotificationData)notiData.data;


		if(inputData.go.name == "MapHeight-Input")
		{
			int.TryParse(inputData.theInput, out selectedMapHeight);
		}
		else if(inputData.go.name == "MapWidth-Input")
		{
			int.TryParse(inputData.theInput, out selectedMapWidth);
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
			case "Help":
				Debug.LogError("Help screen not implemented!");
				break;
			case "Exit":
				if(fileMenu.GetComponent<UIPopupList>().items.Contains("Save"))
				{
					StateMachine<LevelCreatorStates, LevelCreatorStateNotification>.ChangeState(LevelCreatorStates.FrontMenu);
				}
				else
				{
					SceneLoader.Instance.LoadLevel("FrontMenu");
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

		if(fileNameInputText.StartsWith("T-"))
		{
			fileNameInputText = fileNameInputText.Remove(0, 2);
		}

		selectedFileName = LevelCreatorController.mapFilesFilePath + fileNameInputText + ".mpcr";

		if(!string.IsNullOrEmpty(selectedFileName))
		{
			string errorMessage = "";
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
		loadingProgressPanel.SetActive(false);
	}

	public void UpdateProgressBar(float progress)
	{
		if(loadingProgressPanel.activeSelf == false)
		{
			loadingProgressPanel.SetActive(true);
		}
		if(loadingProgressBar.gameObject.activeSelf == false)
		{
			loadingProgressBar.gameObject.SetActive(true);
		}
		loadingProgressBar.sliderValue = progress;

		if(progress > 0.999f)
		{
			TurnOffLoadingBar();
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