using System.Collections;
using System.Collections.Generic;

public class DialogueOption
{
    public int destinationNodeIndex;
    public string dialogueOptionText;
    public List<DialogueOptionRequirement> optionRequirements;
    public List<DialogueOptionEffect> optionEffects;
}
