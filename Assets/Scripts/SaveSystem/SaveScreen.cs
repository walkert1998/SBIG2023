using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveScreen : MonoBehaviour
{
    public TMP_InputField inputField;
    public Transform savesList;
    public GameObject saveFilePrefab;
    public List<Save> saveFiles;
    Texture2D tex;
    bool takingPicture = false;
    public SaveSystem saveSystem;
    public GameObject newSavePanel;
    public GameObject confirmDeletionScreen;
    public GameObject confirmOverwriteScreen;
    public GameObject infoPanel;
    public TMP_Text infoHeader;
    public TMP_Text infoContent;
    Save selectedSave;
    GameObject selectedSaveObject;

    //private void LateUpdate()
    //{
    //    if (takingPicture)
    //    {
    //        takingPicture = false;
    //    }
    //}

    public void PopulateSavesList()
    {
        foreach (Transform t in savesList)
        {
            Destroy(t.gameObject);
        }
        foreach (Save save in saveFiles)
        {
            AddSaveFileToList(save);
        }
    }

    private void OnEnable()
    {
        if (!saveFiles.Equals(saveSystem.GetSaveFiles()))
        {
            saveFiles = saveSystem.GetSaveFiles();
            saveFiles.Sort((Save s1, Save s2) => s2.dateTime.CompareTo(s1.dateTime));
            PopulateSavesList();
        }
    }

    public void DisplayNewSavePanel()
    {
        inputField.text = "Save " + (saveFiles.Count + 1);
        newSavePanel.SetActive(true);
    }

    public void DisplayOverwriteSavePanel()
    {
        confirmOverwriteScreen.SetActive(true);
    }

    public void CreateNewSaveFile()
    {
        string name = inputField.text;
        Save newSave = new Save();
        newSave.saveName = name;
        bool saveFound = false;
        foreach (Save save in saveFiles)
        {
            if (save.saveName == name)
            {
                saveFound = true;
                break;
            }
        }
        if (saveFound)
        {
            DisplayInfoPanel("CANNOT CREATE SAVE", "Save file already exists with name: " + name);
        }
        else
        {
            newSavePanel.SetActive(false);
        }
        saveSystem.CreateNewSaveFile(newSave);
        gameObject.SetActive(false);
    }

    public void OverwriteSave()
    {
        selectedSave.saveScreenshot = null;
        saveSystem.CreateNewSaveFile(selectedSave, true);
        confirmOverwriteScreen.SetActive(false);
        gameObject.SetActive(false);
    }

    public void AddSaveFileToList(Save saveFile)
    {
        GameObject newSave = Instantiate(saveFilePrefab, savesList);
        newSave.transform.Find("SaveName").GetComponent<TMP_Text>().text = saveFile.saveName;
        newSave.transform.Find("SaveDateTime").GetComponent<TMP_Text>().text = saveFile.dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        if (File.Exists(saveFile.screenshotPath))
        {
            saveFile.saveScreenshot = new Texture2D(2, 2);
            saveFile.saveScreenshot.LoadImage(File.ReadAllBytes(saveFile.screenshotPath));
            newSave.transform.Find("SaveScreenshot").GetComponent<RawImage>().texture = saveFile.saveScreenshot;
        }
        newSave.transform.Find("OverwriteButton").GetComponent<Button>().onClick.AddListener(
        () =>
        {
            selectedSave = saveFile;
            selectedSaveObject = newSave;
            DisplayOverwriteSavePanel();
        });
        newSave.transform.Find("DeleteButton").GetComponent<Button>().onClick.AddListener(
        () =>
        {
            selectedSave = saveFile;
            selectedSaveObject = newSave;
            confirmDeletionScreen.SetActive(true);
        });
    }

    public void DisplayInfoPanel(string header, string content)
    {
        infoHeader.text = header;
        infoContent.text = content;
        infoPanel.SetActive(true);
    }

    public void ClearSelection()
    {
        selectedSave = null;
        selectedSaveObject = null;
        confirmDeletionScreen.SetActive(false);
    }

    public void ConfirmDelete()
    {
        saveSystem.DeleteSaveFile(selectedSave);
        RemoveSaveFileFromList(selectedSaveObject);
        saveFiles.Remove(selectedSave);
        ClearSelection();
    }

    public void RemoveSaveFileFromList(GameObject save)
    {
        Destroy(save);
    }
}
