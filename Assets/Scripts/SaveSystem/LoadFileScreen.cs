using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadFileScreen : MonoBehaviour
{
    public Transform savesList;
    public GameObject saveFilePrefab;
    public List<Save> saveFiles;
    public SaveSystem saveSystem;
    public GameObject confirmDeletionScreen;
    public GameObject confirmLoadScreen;
    Save selectedSave;
    GameObject selectedSaveObject;
    // Start is called before the first frame update
    void Start()
    {
        //saveFiles = saveSystem.GetSaveFiles();
        //saveFiles.Sort((Save s1, Save s2) => s1.dateTime.CompareTo(s2.dateTime));
        //PopulateSavesList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //private void LateUpdate()
    //{
    //    if (takingPicture)
    //    {
    //        takingPicture = false;
    //    }
    //}

    public void PopulateSavesList(List<Save> saves)
    {
        foreach (Transform t in savesList)
        {
            Destroy(t.gameObject);
        }
        foreach (Save save in saves)
        {
            AddSaveFileToList(save);
        }
    }

    private void OnEnable()
    {
        saveFiles = saveSystem.GetSaveFiles();
        saveFiles.Sort((Save s1, Save s2) => s2.dateTime.CompareTo(s1.dateTime));
        PopulateSavesList(saveFiles);
    }

    public void DisplayLoadConfirmScreen()
    {
        confirmLoadScreen.SetActive(true);
    }

    public void AddSaveFileToList(Save saveFile)
    {
        GameObject newSave = Instantiate(saveFilePrefab, savesList);
        newSave.transform.Find("SaveName").GetComponent<TMP_Text>().text = saveFile.saveName;
        newSave.transform.Find("SaveDateTime").GetComponent<TMP_Text>().text = saveFile.dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        if (saveFile.screenshotPath != null && File.Exists(saveFile.screenshotPath))
        {
            saveFile.saveScreenshot = new Texture2D(2, 2);
            saveFile.saveScreenshot.LoadImage(File.ReadAllBytes(saveFile.screenshotPath));
            newSave.transform.Find("SaveScreenshot").GetComponent<RawImage>().texture = saveFile.saveScreenshot;
        }
        newSave.transform.Find("LoadButton").GetComponent<Button>().onClick.AddListener(
        () =>
        {
            selectedSave = saveFile;
            selectedSaveObject = newSave;
            DisplayLoadConfirmScreen();
        });
        newSave.transform.Find("DeleteButton").GetComponent<Button>().onClick.AddListener(
        () =>
        {
            selectedSave = saveFile;
            selectedSaveObject = newSave;
            confirmDeletionScreen.SetActive(true);
        });
    }

    public void ConfirmLoad()
    {
        selectedSave.saveScreenshot = null;
        saveSystem.LoadSaveFile(selectedSave.saveName + ".save");
        confirmLoadScreen.SetActive(false);
    }

    public void ClearSelection()
    {
        selectedSave = null;
        selectedSaveObject = null;
        confirmDeletionScreen.SetActive(false);
        confirmLoadScreen.SetActive(false);
    }

    public void ConfirmDelete()
    {
        saveFiles.Remove(selectedSave);
        saveSystem.DeleteSaveFile(selectedSave);
        Destroy(selectedSaveObject);
        ClearSelection();
    }
}
