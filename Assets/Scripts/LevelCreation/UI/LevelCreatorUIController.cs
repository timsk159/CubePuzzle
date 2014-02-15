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
	public GameObject newMapPanel;
	public GameObject levelAssetMenuPanel;
	public GameObject savingMapPanel;
	public GameObject loadingMapPanel;
	public GameObject controlsPanel;

	public UIToggle isFilledCheckbox;

	public GameObject loadingProgressPanel;

	public UISlider loadingProgressBar;
	public UILabel saveErrorLabel;

	public GameObject fileMenuEntryPrefab;
	
	LevelCreator levelCreator;
	
	int selectedMapHeight;
	int selectedMapWidth;
	bool isInFrontMenu = true;
	bool isSavingMap = false;
	
	string selectedFileName;

	public static bool cameFromPreview;

	void Awake()
	{
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

		StateMachineMessenger.AddListener(LevelCreatorStateMessage.NewMapEnter.ToString(), NewMapEnter);
		StateMachineMessenger.AddListener(LevelCreatorStateMessage.NewMapExit.ToString(), NewMapExit);

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

		StateMachineMessenger.RemoveListener(LevelCreatorStateMessage.NewMapEnter.ToString(), NewMapEnter);
		StateMachineMessenger.RemoveListener(LevelCreatorStateMessage.NewMapExit.ToString(), NewMapExit);

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
	}
	
	void FrontMenuExit(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
	{
		if(frontMenuPanel.activeSelf)
		{
			NGUITools.SetActive(frontMenuPanel, false);
		}
	}

	void NewMapEnter(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
	{
		NGUITools.SetActive(newMapPanel, true);
	}

	void NewMapExit(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
	{
		NGUITools.SetActive(newMapPanel, false);
	}
	
	void LevelCreationEnter(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
	{
		levelCreator = GameObject.Find("LevelCreator").GetComponent<LevelCreator>();

		isInFrontMenu = false;

		NGUITools.SetActive(levelAssetMenuPanel, true);
	}

	void LevelCreationExit(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
	{
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
		if(loadingProgressPanel.activeSelf == true)
		{
			loadingProgressPanel.SetActive(false);
		}
	}

	void TestingMapExit(StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.StateChangeData stateChangeData)
	{

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

		Messenger.AddListener(LevelCreatorUIMessage.NewMap.ToString(), NewMapPressed);
		Messenger.AddListener(LevelCreatorUIMessage.LoadMap.ToString(), LoadMapPressed);
		Messenger.AddListener(LevelCreatorUIMessage.FrontMenuBack.ToString(), FrontMenuBackPressed);
		Messenger.AddListener(LevelCreatorUIMessage.NewMapBack.ToString(), NewMapBackPressed);
		Messenger.AddListener(LevelCreatorUIMessage.Controls.ToString(), ControlsPressed);
		Messenger.AddListener(LevelCreatorUIMessage.ControlsBack.ToString(), ControlsBackPressed);

		Messenger.AddListener(LevelCreatorUIMessage.Test.ToString(), TestPressed);
		Messenger.AddListener(LevelCreatorUIMessage.Save.ToString(), SavePressed);
		Messenger.AddListener(LevelCreatorUIMessage.Exit.ToString(), ExitPressed);

		Messenger.AddListener(EndGameMenuMessage.StopTesting.ToString(), StopTesting);
		Messenger.AddListener(PauseMenuMessage.StopTesting.ToString(), StopTesting);
	}

	void DeRegisterUIListeners()
	{
		Messenger.RemoveListener(LevelCreatorUIMessage.CreateButtonClicked.ToString(), CreateButtonClicked);
		Messenger.RemoveListener(LevelCreatorUIMessage.LoadMenuCancelClicked.ToString(), LoadMenuCancelClicked);
		Messenger.RemoveListener(LevelCreatorUIMessage.LoadMenuLoadClicked.ToString(), LoadMenuLoadClicked);
		Messenger.RemoveListener(LevelCreatorUIMessage.SaveMenuCancelClicked.ToString(), SaveMenuCancelClicked);
		Messenger.RemoveListener(LevelCreatorUIMessage.SaveMenuSaveClicked.ToString(), SaveMenuSaveClicked);
		Messenger<InputMessageData>.RemoveListener(LevelCreatorUIMessage.GenericInputSubmitted.ToString(), GenericInputSubmitted);

		Messenger.RemoveListener(LevelCreatorUIMessage.NewMap.ToString(), NewMapPressed);
		Messenger.RemoveListener(LevelCreatorUIMessage.LoadMap.ToString(), LoadMapPressed);
		Messenger.RemoveListener(LevelCreatorUIMessage.FrontMenuBack.ToString(), FrontMenuBackPressed);
		Messenger.RemoveListener(LevelCreatorUIMessage.NewMapBack.ToString(), NewMapBackPressed);
		Messenger.RemoveListener(LevelCreatorUIMessage.Controls.ToString(), ControlsPressed);
		Messenger.RemoveListener(LevelCreatorUIMessage.ControlsBack.ToString(), ControlsBackPressed);

		Messenger.RemoveListener(LevelCreatorUIMessage.Test.ToString(), TestPressed);
		Messenger.RemoveListener(LevelCreatorUIMessage.Save.ToString(), SavePressed);
		Messenger.RemoveListener(LevelCreatorUIMessage.Exit.ToString(), ExitPressed);

		Messenger.RemoveListener(EndGameMenuMessage.StopTesting.ToString(), StopTesting);
		Messenger.RemoveListener(PauseMenuMessage.StopTesting.ToString(), StopTesting);
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

		var prefabToUse = levelCreator.assetManager.nullCubePrefab;
		if(isFilledCheckbox.value)
			prefabToUse = levelCreator.assetManager.neutralCubePrefab;

		if (int.TryParse (mapHeightInput.text, out selectedMapHeight) && int.TryParse(mapWidthInput.text, out selectedMapWidth))
		{
			for (int x = 0; x <= selectedMapWidth - 1; x++)
			{
				for (int z = 0; z <= selectedMapHeight - 1; z++)
				{
					var obj = (GameObject)Instantiate(prefabToUse);
					if(isFilledCheckbox.value)
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

	void NewMapPressed()
	{
		StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.ChangeState(LevelCreatorStates.NewMap);
	}

	void LoadMapPressed()
	{
		StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.ChangeState(LevelCreatorStates.LoadingMap);
	}

	void FrontMenuBackPressed()
	{
		SceneLoader.Instance.LoadLevel("FrontMenu");
	}

	void NewMapBackPressed()
	{
		StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.ChangeState(LevelCreatorStates.FrontMenu);
	}

	void ControlsPressed()
	{
		NGUITools.SetActive(controlsPanel, true);
	}

	void ControlsBackPressed()
	{
		NGUITools.SetActive(controlsPanel, false);
	}

	void TestPressed()
	{
		var errorMessage = "";
		if (levelCreator.MapIsComplete(out errorMessage))
		{
			var previousSave = LevelSerializer.SavedGames[LevelSerializer.PlayerName].Where(e => e.Name == "BeforePreviewSave").FirstOrDefault();

			if (previousSave != null)
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
	}

	void StopTesting()
	{
		StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.ChangeState(LevelCreatorStates.LevelCreation);
	}

	void SavePressed()
	{
		StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.ChangeState(LevelCreatorStates.SavingMap);
	}

	void ExitPressed()
	{
		StateMachine<LevelCreatorStates, LevelCreatorStateMessage>.ChangeState(LevelCreatorStates.FrontMenu);
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

		EventDelegate.Add(saveErrorLabel.GetComponent<TweenAlpha> ().onFinished, ErrorLabelForwardFinished);

		saveErrorLabel.GetComponent<TweenAlpha> ().Play (true);
	}

	void ErrorLabelForwardFinished()
	{
		StartCoroutine (ErrorLabelForwardFinishedRoutine ());
	}

	void ErrorLabelBackwardFinished()
	{
		EventDelegate.Remove(UITweener.current.onFinished, ErrorLabelBackwardFinished);
		NGUITools.SetActive(saveErrorLabel.gameObject, false);
	}

	IEnumerator ErrorLabelForwardFinishedRoutine()
	{
		var tweener = UITweener.current;
		yield return new WaitForSeconds (1.5f);
		EventDelegate.Remove(tweener.onFinished, ErrorLabelForwardFinished);
		EventDelegate.Add(tweener.onFinished, ErrorLabelBackwardFinished);
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
		loadingProgressBar.value = progress;

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

				var panel = fileListEntry.GetComponent<UIPanel>();
				if(panel != null)
					Destroy(panel);
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