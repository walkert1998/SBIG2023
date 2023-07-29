using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveableEntity : MonoBehaviour
{
    [SerializeField]
    private string id = string.Empty;

    public string ID => id;

    [ContextMenu("Generate ID")]
    public void GenerateID()
    {
        id = Guid.NewGuid().ToString();
    }

    public object SaveState()
    {
        var state = new Dictionary<string, object>();
        foreach (var saveable in GetComponents<ISaveable>())
        {
            //Debug.Log(JsonConvert.SerializeObject(state));
            state[saveable.GetType().ToString()] = saveable.SaveState();
        }
        return state;
    }

    public void LoadState(object state)
    {
        string temp = JsonConvert.SerializeObject(state);
        Dictionary<string, object> stateDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(temp);
        foreach (var saveable in GetComponents<ISaveable>())
        {
            string typeName = saveable.GetType().ToString();
            if (stateDictionary.TryGetValue(typeName, out object value))
            {
                saveable.LoadState(value);
            }
        }
    }
}
