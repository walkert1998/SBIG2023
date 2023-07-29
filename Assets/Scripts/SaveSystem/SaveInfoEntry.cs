using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveInfoEntry
{
    public string key;
    public object value;

    public SaveInfoEntry(string key, object value)
    {
        this.key = key;
        this.value = value;
    }
}
