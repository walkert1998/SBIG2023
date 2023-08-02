using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class DialogueTree
{
    public string characterName;
    public DialogueNode[] dialogueNodes;

    public DialogueTree(DialogueNode[] nodes)
    {
        dialogueNodes = nodes;
    }
    
    public static DialogueTree LoadDialogue(string content)
    {
        // string fileTextContent = File.ReadAllText(path);
        DialogueTree tree = new DialogueTree(null);
        List<DialogueNode> newNodes = new List<DialogueNode>();
        DialogueTreeSaveData treeData = JsonConvert.DeserializeObject<DialogueTreeSaveData>(content);
        treeData.dialogueNodes = treeData.dialogueNodes.ToList().OrderBy(x => x.nodeIndex).ToList();
        foreach (DialogueNodeSaveData nodeSaveData in treeData.dialogueNodes)
        {
            DialogueNode newNode = new DialogueNode();
            newNode.characterSpeaking = nodeSaveData.characterSpeaking;
            newNode.destinationNodeIndex = nodeSaveData.destinationNodeIndex;
            newNode.dialogueAudioFileName = nodeSaveData.dialogueAudioFileName;
            newNode.dialogueText = nodeSaveData.dialogueText;
            newNode.nodeIndex = nodeSaveData.nodeIndex;
            newNode.graphPosition = nodeSaveData.graphPosition;
            newNode.silenceLength = nodeSaveData.silenceLength;
            
            if (nodeSaveData.dialogueOptions != null && nodeSaveData.dialogueOptions.Count > 0)
            {
                newNode.dialogueOptions = new List<DialogueOption>();
                foreach (DialogueOptionSaveData optionSaveData in nodeSaveData.dialogueOptions)
                {
                    DialogueOption newOption = new DialogueOption();
                    newOption.destinationNodeIndex = optionSaveData.destinationNodeIndex;
                    newOption.dialogueOptionText = optionSaveData.dialogueOptionText;
                    if (optionSaveData.optionEffects != null && optionSaveData.optionEffects.Count > 0)
                    {
                        newOption.optionEffects = new List<DialogueOptionEffect>();
                        foreach (DialogueOptionEffectSaveData optionEffectSaveData in optionSaveData.optionEffects)
                        {
                            DialogueOptionEffect newOptionEffect = new DialogueOptionEffect();
                            newOptionEffect.effectType = optionEffectSaveData.effectType;
                            newOptionEffect.faction = optionEffectSaveData.faction;
                            newOptionEffect.intValue = optionEffectSaveData.intValue;
                            if (optionEffectSaveData.itemBaseID != null)
                            {
                                newOptionEffect.item =  Resources.LoadAll<Item>("ScriptableObject/").Where(x => x.baseItemID == optionEffectSaveData.itemBaseID).First();
                            }
                            newOption.optionEffects.Add(newOptionEffect);
                        }
                    }
                    if (optionSaveData.optionRequirements != null && optionSaveData.optionRequirements.Count > 0)
                    {
                        newOption.optionRequirements = new List<DialogueOptionRequirement>();
                        foreach (DialogueOptionRequirementSaveData requirementSaveData in optionSaveData.optionRequirements)
                        {
                            DialogueOptionRequirement newRequirement = new DialogueOptionRequirement();
                            newRequirement.comparisonOperator = requirementSaveData.comparisonOperator;
                            newRequirement.factionRequirement = requirementSaveData.factionRequirement;
                            newRequirement.intValue = requirementSaveData.intValue;
                            newRequirement.requiredItem = Resources.LoadAll<Item>("ScriptableObject/").Where(x => x.baseItemID == requirementSaveData.requiredItemBaseID).First();
                            newRequirement.requirementType = requirementSaveData.requirementType;
                            newOption.optionRequirements.Add(newRequirement);
                        }
                    }
                    newNode.dialogueOptions.Add(newOption);
                }
            }

            if (nodeSaveData.dialogueEvents != null && nodeSaveData.dialogueEvents.Count > 0)
            {
                newNode.dialogueEvents = nodeSaveData.dialogueEvents;
            }
            newNodes.Add(newNode);
        }
        tree.dialogueNodes = newNodes.ToArray();
        return tree;
    }
}
