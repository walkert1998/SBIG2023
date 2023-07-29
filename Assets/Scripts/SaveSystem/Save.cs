using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    public string saveName;
    public string sceneName;
    public DateTime dateTime;
    public Texture2D saveScreenshot;
    public string screenshotPath;
    public Dictionary<string, object> saveData;
}
