using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    public GameObject canvas;
    private string savePath;
    private string screenshotPath;
    public List<Save> saves;
    public bool saving = false;
    public bool saveSuccessful = true;
    bool takingScreenshot = false;
    // public LevelTransition levelTransition;
    public static Save loadOnStart;
    InputManager inputManager;
    // Start is called before the first frame update
    void Start()
    {
        savePath = Application.persistentDataPath + "/Saves/";
        screenshotPath = savePath + "Screenshots/";
        if (loadOnStart != null)
        {
            LoadSaveFile(loadOnStart.saveName + ".save");
        }
        inputManager = InputManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (inputManager.QuicksavePressed())
        {
            Quicksave();
        }
        if (inputManager.QuickloadPressed())
        {
            Quickload();
        }
    }

    public List<Save> GetSaveFiles()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(savePath);
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        if (!Directory.Exists(screenshotPath))
        {
            Directory.CreateDirectory(screenshotPath);
        }
        FileInfo[] info = directoryInfo.GetFiles("*.save");
        saves = new List<Save>();
        foreach (FileInfo file in info)
        {
            string fileTextContent = File.ReadAllText(savePath + file.Name);
            Save loadedGame = JsonConvert.DeserializeObject<Save>(fileTextContent);
            //newSave.saveName = file.Name;
            //newSave.dateTime = file.CreationTime;
            saves.Add(loadedGame);
        }
        //saves.OrderBy(save => save.dateTime);
        return saves;
    }

    IEnumerator Save(Save newSave, bool overwrite=false)
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        if (!Directory.Exists(screenshotPath))
        {
            Directory.CreateDirectory(screenshotPath);
        }
        string newFilePath = savePath + newSave.saveName + ".save";
        // Debug.Log(newFilePath);
        if (File.Exists(newFilePath) && !overwrite)
        {
            // Debug.LogError("File '" + newFilePath + "' already exists!");
            saving = false;
            saveSuccessful = false;
            yield break;
        }
        else
        {
            DateTime dateTime = DateTime.Now;
            newSave.dateTime = dateTime;
            newSave.saveData = new Dictionary<string, object>();
            newSave.sceneName = SceneManager.GetActiveScene().name;
            // Debug.Log(newFilePath);
            StartCoroutine(TakeScreenshot(newSave));
            while (takingScreenshot)
            {
                yield return null;
            }
            saves.Add(newSave);
            SaveState(newSave.saveData);
            string jsonSaveinfo = JsonConvert.SerializeObject(newSave, Formatting.Indented, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            File.WriteAllText(newFilePath, jsonSaveinfo);
            saveSuccessful = true;
            // Debug.Log(saveSuccessful);
        }
        saving = false;
    }

    public void AutoSave(string saveName)
    {
        Save autoSave = new Save();
        autoSave.saveName = saveName;
        CreateNewSaveFile(autoSave, true);
    }

    public void Quicksave()
    {
        Save quicksave = new Save();
        quicksave.saveName = "Quicksave";
        CreateNewSaveFile(quicksave, true);
        // EventLog.AddEventText("Game Saved!");
    }

    public bool Quickload()
    {
        bool quicksaveExists = LoadSaveFile("Quicksave.save");
        return quicksaveExists;
    }

    public void CreateNewSaveFile(Save newSave, bool overwrite=false)
    {
        saving = true;
        StartCoroutine(Save(newSave, overwrite));
    }

    public bool LoadSaveFile(string loadedFile)
    {
        string fileName = savePath + loadedFile;
        // Debug.Log(fileName);
        if (File.Exists(fileName))
        {
            string fileTextContent = File.ReadAllText(fileName);
            Save loadedGame = JsonConvert.DeserializeObject<Save>(fileTextContent);
            if (loadedGame.sceneName != SceneManager.GetActiveScene().name)
            {
                loadOnStart = loadedGame;
                // Debug.Log(loadedGame.sceneName);
                // LoadingScreen.SetLevelToLoadStatic(loadedGame.sceneName, SceneManager.GetActiveScene().name);
                // levelTransition.ChangeLevelTo("LoadingScreen");
            }
            else
            {
                string saveData = JsonConvert.SerializeObject(loadedGame.saveData);
                loadedGame.saveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(saveData, new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.Auto
                    //ReferenceResolverProvider = () => new GenericResolver<Item>(p => p.itemName)
                });
                LoadState(loadedGame.saveData);
                loadOnStart = null;
            }
            return true;
        }
        else
        {
            Debug.LogError("File '" + fileName + "' could not be found!");
            return false;
        }
    }

    public void DeleteSaveFile(Save save)
    {
        saves.Remove(save);
        string fileName = savePath + save.saveName;
        File.Delete(save.screenshotPath);
        File.Delete(fileName + ".save");
    }

    public void SaveState(Dictionary<string, object> state)
    {
        foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
        {
            state[saveable.ID] = saveable.SaveState();
        }
    }

    [ContextMenu("Generate ID's")]
    public void GenerateIDs()
    {
        foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
        {
            saveable.GenerateID();
        }
    }

    public void LoadState(Dictionary<string, object> state)
    {
        SaveableEntity[] entities = FindObjectsOfType<SaveableEntity>();
        Array.Sort(entities, delegate(SaveableEntity a, SaveableEntity b) { return a.name.CompareTo(b.name); } );
        foreach (SaveableEntity saveable in entities)
        {
            if (state.TryGetValue(saveable.ID, out object value))
            {
                Debug.Log("Loading " + saveable.name);
                saveable.LoadState(value);
            }
        }
    }

    IEnumerator TakeScreenshot(Save newSave)
    {
        takingScreenshot = true;
        bool inMenu = canvas.activeSelf;
        if (inMenu)
        {
            canvas.SetActive(false);
        }
        yield return new WaitForEndOfFrame();
        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] texInfo = tex.EncodeToPNG();
        newSave.screenshotPath = screenshotPath + newSave.saveName + ".png";
        File.WriteAllBytes(newSave.screenshotPath, texInfo);
        //newSave.saveScreenshot = tex;
        //AddSaveFileToList(newSave);
        if (inMenu)
        {
            canvas.SetActive(true);
        }
        takingScreenshot = false;
    }
}
