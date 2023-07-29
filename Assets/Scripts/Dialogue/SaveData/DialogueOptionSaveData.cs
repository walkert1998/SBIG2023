using System.Collections.Generic;

public class DialogueOptionSaveData
{
    public int destinationNodeIndex;
    public string dialogueOptionText;
    public List<DialogueOptionRequirementSaveData> optionRequirements;
    public List<DialogueOptionEffectSaveData> optionEffects;
}
