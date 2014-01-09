using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using StateMachineMessenger = Messenger<StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData>;

[DoNotSerializePublic()]
public class LevelCreatorUIController : MonoBehaviour 
{
	public GameObject frontMenuPanel;
	public GameObject levelCreationMenuPanel;
	public GameObject levelAssetMenuPanel;
	public GameObject savingMapPanel;
	public GameObject loadingMapPanel;

	public UICheckbox isFilledCheckbox;

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
		DeRegisterUIListeners();
		DeRegisterStateListeners();
	}

	void OnDeserialized()
	{
		TurnOffLoadingBar();
		NGUITools.SetActive(frontMenuPanel, false);
		NGUITools.SetActive(levelAssetMenuPanel, true);
	}
	
	#region State Changes
	void RegisterStateListeners()
	{
		StateMachineMessenger.AddListener(LevelCreatorStateMessage.FrontMenuEnter.ToString(), FrontMenuEnter);
		StateMachineMessenger.AddListener(LevelCreatorStateMessage.FrontMenuExit.ToString(), FrontMenuExit);

		StateMachineMessenger.AddListener(LevelCreatorStateMessage.LevelCreationEnter.ToString(), LevelCreationEnter);
		StateMachineMessenger.AddListener(LevelCreatorStateMessage.LevelCreationExit.ToString(), LevelCreationExit);

		StateMachineMessenger.AddListener(LevelCreatorStateMessage.SavingMapEnter.ToString(), SavingMapEnter);
		StateMachineMessenger.AddListener(LevelCreatorStateMessage.SavingMapExit.ToString(), SavingMapExit);

		StateMachineMessenger.AddListener(LevelCreatorStateMessage.LoadingMapEnter.ToString(), LoadingMapEnter);
		StateMachineMessenger.AddListener(LevelCreatorStateMessage.LoadingMapExit.ToString(), LoadingMapExit);

		StateMachineMessenger.AddListener(LevelCreatorStateMessage.TestingMapEnter.ToString(), TestingMapEnter);
		StateMachineMessenger.AddListener(LevelCreatorStateMessage.TestingMapExit.ToString(), TestingMapExit);
	}

	void DeRegisterStateListeners()
	{
		StateMachineMessenger.RemoveListener(LevelCreatorStateMessage.FrontMenuEnter.ToString(), FrontMenuEnter);
		StateMachineMessenger.RemoveListener(LevelCreatorStateMessage.FrontMenuExit.ToString(), FrontMenuExit);

		StateMachineMessenger.RemoveListener(LevelCreatorStateMessage.LevelCreationEnter.ToString(), LevelCreationEnter);
		StateMachineMessenger.RemoveListener(LevelCreatorStateMessage.LevelCreationExit.ToString(), LevelCreationExit);

		StateMachineMessenger.RemoveListener(LevelCreatorStateMessage.SavingMapEnter.ToString(), SavingMapEnter);
		StateMachineMessenger.RemoveListener(LevelCreatorStateMessage.SavingMapExit.ToString(), SavingMapExit);

		StateMachineMessenger.RemoveListener(LevelCreatorStateMessage.LoadingMapEnter.ToString(), LoadingMapEnter);
		StateMachineMessenger.RemoveListener(LevelCreatorStateMessage.LoadingMapExit.ToString(), LoadingMapExit);

		StateMachineMessenger.RemoveListener(LevelCreatorStateMessage.TestingMapEnter.ToString(), TestingMapEnter);
		StateMachineMessenger.RemoveListener(LevelCreatorStateMessage.TestingMapExit.ToString(), TestingMapExit);
	}
	
	void FrontMenuEnter(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
	{
		isInFrontMenu = true;
		if(!frontMenuPanel.activeSelf)
		{
			NGUITools.SetActive(frontMenuPanel, true);
		}
		NGUITools.SetActive(levelAssetMenuPanel, false);

		if(fileMenu.items.Contains("Save"))
			fileMenu.items.Remove("Save");
		if(fileMenu.items.Contains("TestMap"))
			fileMenu.items.Remove("TestMap");
	}
	
	void FrontMenuExit(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
	{
		if(frontMenuPanel.activeSelf)
		{
			NGUITools.SetActive(frontMenuPanel, false);
		}
	}
	
	void LevelCreationEnter(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
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

	void LevelCreationExit(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
	{
		if(fileMenu.items.Contains("Save"))
			fileMenu.items.Remove("Save");
		if(fileMenu.items.Contains("TestMap"))
			fileMenu.items.Remove("TestMap");

		NGUITools.SetActive(levelAssetMenuPanel, false);
	}
	
	void SavingMapEnter(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
	{
		NGUITools.SetActive(savingMapPanel, true);
		isSavingMap = true;
		var fileMenuGrid = savingMapPanel.transform.Find("FileListScrollablePanel/FileListGrid").gameObject;
		StartCoroutine(PopulateFileMenu(fileMenuGrid));
	}
	
	void SavingMapExit(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
	{
		isSavingMap = false;
		TurnOffLoadingBar ();
		NGUITools.SetActive(savingMapPanel, false);	
	}
	
	void LoadingMapEnter(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
	{
		NGUITools.SetActive(loadingMapPanel, true);
		var fileMenuGrid = loadingMapPanel.transform.Find("FileListScrollablePanel/FileListGrid").gameObject;
		StartCoroutine(PopulateFileMenu(fileMenuGrid));
	}
	
	void LoadingMapExit(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
	{
		NGUITools.SetActive(loadingMapPanel, false);
	}

	void TestingMapEnter(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
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

	void TestingMapExit(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
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
		Messenger.AddListener(LevelCreatorUIMessage.CreateButtonClicked.ToString(), CreateButtonClicked);
		Messenger.AddListener(LevelCreatorUIMessage.LoadMenuCancelClicked.ToString(), LoadMenuCancelClicked);
		Messenger.AddListener(LevelCreatorUIMessage.LoadMenuLoadClicked.ToString(), LoadMenuLoadClicked);
		Messenger.AddListener(LevelCreatorUIMessage.SaveMenuCancelClicked.ToString(), SaveMenuCancelClicked);
		Messenger.AddListener(LevelCreatorUIMessage.SaveMenuSaveClicked.ToString(), SaveMenuSaveClicked);
		Messenger<InputMessageData>.AddListener(LevelCreatorUIMessage.GenericInputSubmitted.ToString(), GenericInputSubmitted);

		Messenger<bool>.AddListener(LevelCreatorUIMessage.SideHoverEnter.ToString(), SideHoverEnter);
		Messenger<bool>.AddListener(LevelCreatorUIMessage.SideHoverExit.ToString(), SideHoverExit);


		fileMenu.GetComponent<UIPopupList>().onSelectionChange = FileMenuSelectionChanged;
	}

	void DeRegisterUIListeners()
	{
		Messenger.RemoveListener(LevelCreatorUIMessage.CreateButtonClicked.ToString(), CreateButtonClicked);
		Messenger.RemoveListener(LevelCreatorUIMessage.LoadMenuCancelClicked.ToString(), LoadMenuCancelClicked);
		Messenger.RemoveListener(LevelCreatorUIMessage.LoadMenuLoadClicked.ToString(), LoadMenuLoadClicked);
		Messenger.RemoveListener(LevelCreatorUIMessage.SaveMenuCancelClicked.ToString(), SaveMenuCancelClicked);
		Messenger.RemoveListener(LevelCreatorUIMessage.SaveMenuSaveClicked.ToString(), SaveMenuSaveClicked);
		Messenger<InputMessageData>.RemoveListener(LevelCreatorUIMessage.GenericInputSubmitted.ToString(), GenericInputSubmitted);

		Messenger<bool>.RemoveListener(LevelCreatorUIMessage.SideHoverEnter.ToString(), SideHoverEnter);
		Messenger<bool>.RemoveListener(LevelCreatorUIMessage.SideHoverExit.ToString(), SideHoverExit);

	}

	void HandleProgress (string arg1, float arg2)
	{
		if(arg1 != "Initializing")
			UpdateProgressBar(arg2);
	}

	void SideHoverEnter(bool isLeft)
	{

	}

	void SideHoverExit(bool isLeft)
	{

	}

	void CreateButtonClicked()
	{
		//change to get the input class and get it's label from there.
		var mapHeightInput = GameObject.Find("MapHeight-Input").GetComponent<UIInput>().label;
		var mapWidthInput = GameObject.Find("MapWidth-Input").GetComponent<UIInput>().label;

		var prefabToUse = levelCreator.assetManager.nullCubePrefab;
		if(isFilledCheckbox.isChecked)
			prefabToUse = levelCreator.assetManager.neutralCubePrefab;

		if (int.TryParse (mapHeightInput.text, out selectedMapHeight) && int.TryParse(mapWidthInput.text, out selectedMapWidth))
		{
			for (int x = 0; x <= selectedMapWidth - 1; x++)
			{
				for (int z = 0; z <= selectedMapHeight - 1; z++)
				{
					var obj = (GameObject)Instantiate(prefabToUse);
					if(isFilledCheckbox.isChecked)
						obj.AddComponent<DraggableMapObject>();
					obj.transform.position = new Vector3 (x, 0, z);

					obj.transform.parent = levelCreator.mapRoot.transform;
				
				}
			}
			StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.ChangeState (LevelCreatorStates.LevelCreation);
		}
	}
	
	void GenericInputSubmitted(InputMessageData inputData)
	{
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
				StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.ChangeState(LevelCreatorStates.LoadingMap);
				break;
			case "TestMap":
				var errorMessage = "";
				if(levelCreator.MapIsComplete(out errorMessage))
				{
					var previousSave = LevelSerializer.SavedGames[LevelSerializer.PlayerName].Where(e => e.Name == "BeforePreviewSave").FirstOrDefault();

					if(previousSave != null)
					{
						LevelSerializer.SavedGames[LevelSerializer.PlayerName].Remove(previousSave);
					}

					LevelSerializer.SaveGame("BeforePreviewSave");

					StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.ChangeState(LevelCreatorStates.TestingMap);
				}
				else
				{
					DisplayErrorMessage(errorMessage);
				}
				break;
			case "StopTesting":
				StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.ChangeState(LevelCreatorStates.LevelCreation);
				break;
			case "Save":
				StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.ChangeState(LevelCreatorStates.SavingMap);
				break;
			case "Help":
				Debug.LogError("Help screen not implemented!");
				break;
			case "Exit":
				if(fileMenu.GetComponent<UIPopupList>().items.Contains("Save"))
				{
					StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.ChangeState(LevelCreatorStates.FrontMenu);
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
			StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.ChangeState(LevelCreatorStates.FrontMenu);
		else
			StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.ChangeState(LevelCreatorStates.LevelCreation);
	}
	
	void LoadMenuLoadClicked()
	{
		if(!string.IsNullOrEmpty(selectedFileName))
		{
			levelCreator.LoadMapInCreator(selectedFileName);
			selectedFileName = "";
		}
	}
	
	void SaveMenuCancelClicked()
	{
		if(isInFrontMenu)
			StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.ChangeState(LevelCreatorStates.FrontMenu);
		else
			StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.ChangeState(LevelCreatorStates.LevelCreation);
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
				StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.ChangeState (LevelCreatorStates.LevelCreation);
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