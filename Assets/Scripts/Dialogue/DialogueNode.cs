using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNode
{
    public int nodeIndex;
    public int destinationNodeIndex;
    public string dialogueText;
    public string characterSpeaking;
    public string dialogueAudioFileName;
    public AudioClip dialogueAudio;
    public Vector2 graphPosition;
    public List<DialogueOption> dialogueOptions;
    public List<DialogueEvent> dialogueEvents;
}
