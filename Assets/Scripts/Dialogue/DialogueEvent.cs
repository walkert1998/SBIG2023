using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEvent
{
    public DialogueEventType eventType;
    public float invokeTime;
    public string eventName;
    public string stringParameter;
    public int intParameter;
    public float floatParameter;
    public int invoked = 0;
}
