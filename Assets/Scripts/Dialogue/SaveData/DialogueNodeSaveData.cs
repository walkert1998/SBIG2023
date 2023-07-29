using System.Collections.Generic;
using UnityEngine;

public struct DialogueNodeSaveData
{
    public int nodeIndex;
    public int destinationNodeIndex;
    public string dialogueText;
    public string characterSpeaking;
    public string dialogueAudioFileName;
    public Vector2 graphPosition;
    public List<DialogueOptionSaveData> dialogueOptions;
    public List<DialogueEvent> dialogueEvents;
}
