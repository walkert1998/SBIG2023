using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView graphView;
    private string fileName;

    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        DialogueGraph window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    private void ConstructGraphView()
    {
        graphView = new DialogueGraphView();
        graphView.name = "Dialogue Graph";
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    private void GenerateToolbar()
    {
        Toolbar toolbar = new Toolbar();

        Button saveButton = new Button(() => SaveData());
        saveButton.text = "Save Dialogue Tree";
        toolbar.Add(saveButton);

        Button loadButton = new Button(() => LoadData());
        loadButton.text = "Load Dialogue Tree";
        toolbar.Add(loadButton);

        Button nodeCreateButton = new Button(() => { graphView.CreateNode("Dialogue Node", Vector2.zero); } );
        nodeCreateButton.text = "Create Node";
        toolbar.Add(nodeCreateButton);

        rootVisualElement.Add(toolbar);
    }

    private void LoadData()
    {
        string filePath = EditorUtility.OpenFilePanel("Select Dialogue Graph", "Assets/Resources/Dialogue/", "JSON");
        if (!File.Exists(filePath))
        {
            return;
        }

        string fileContent = File.ReadAllText(filePath);
        DialogueTree tree = DialogueTree.LoadDialogue(fileContent);
        Debug.Log(tree.dialogueNodes.Length);
        foreach (DialogueNode node in tree.dialogueNodes)
        {
            DialogueGraphNode newNode = graphView.CreateNode("Dialogue Node", node.graphPosition, node);
        }
        graphView.ConnectAllPorts();
    }

    private void SaveData()
    {
        string newFilePath = EditorUtility.SaveFilePanel("Save Dialogue Graph As", "Assets/Resources/Dialogue/", "", "JSON");
        List<DialogueGraphNode> dialogueNodes = graphView.nodes.ToList().Cast<DialogueGraphNode>().ToList();
        DialogueTreeSaveData treeSaveData = new DialogueTreeSaveData();
        treeSaveData.dialogueNodes = new List<DialogueNodeSaveData>();
        for (int i = 1; i < dialogueNodes.Count; i++)
        {
            DialogueNodeSaveData nodeSaveData = new DialogueNodeSaveData();
            nodeSaveData.characterSpeaking = dialogueNodes[i].dialogueNode.characterSpeaking;
            nodeSaveData.dialogueText = dialogueNodes[i].dialogueNode.dialogueText;
            nodeSaveData.nodeIndex = dialogueNodes[i].dialogueNode.nodeIndex;
            nodeSaveData.destinationNodeIndex = dialogueNodes[i].dialogueNode.destinationNodeIndex;
            nodeSaveData.graphPosition = dialogueNodes[i].GetPosition().position;
            nodeSaveData.silenceLength = dialogueNodes[i].dialogueNode.silenceLength;
            if (dialogueNodes[i].dialogueNode.dialogueAudio != null)
            {
                nodeSaveData.dialogueAudioFileName = dialogueNodes[i].dialogueNode.dialogueAudio.name;
            }
            if (dialogueNodes[i].dialogueNode.dialogueOptions != null)
            {
                nodeSaveData.dialogueOptions = new List<DialogueOptionSaveData>();
                for (int j = 0; j < dialogueNodes[i].dialogueNode.dialogueOptions.Count; j++)
                {
                    DialogueOptionSaveData optionSaveData = new DialogueOptionSaveData();
                    optionSaveData.dialogueOptionText = dialogueNodes[i].dialogueNode.dialogueOptions[j].dialogueOptionText;
                    optionSaveData.destinationNodeIndex = dialogueNodes[i].dialogueNode.dialogueOptions[j].destinationNodeIndex;
                    // dialogueNodes[i].outputContainer.;
                    // Debug.Log(dialogueNodes[i].dialogueNode.dialogueOptions[j].dialogueOptionText);
                    // Debug.Log(dialogueNodes[i].dialogueNode.dialogueOptions[j].optionEffects.ToArray());
                    if (dialogueNodes[i].dialogueNode.dialogueOptions[j].optionEffects.Count > 0)
                    {
                        optionSaveData.optionEffects = new List<DialogueOptionEffectSaveData>();
                        foreach (DialogueOptionEffect effect in dialogueNodes[i].dialogueNode.dialogueOptions[j].optionEffects)
                        {
                            DialogueOptionEffectSaveData effectSaveData = new DialogueOptionEffectSaveData();
                            effectSaveData.effectType = effect.effectType;
                            effectSaveData.faction = effect.faction;
                            effectSaveData.intValue = effect.intValue;
                            effectSaveData.stringValue = effect.stringValue;
                            if (effect.item != null)
                            {
                                effectSaveData.itemBaseID = effect.item.baseItemID;
                            }
                            optionSaveData.optionEffects.Add(effectSaveData);
                        }
                    }
                    if (dialogueNodes[i].dialogueNode.dialogueOptions[j].optionRequirements.Count > 0)
                    {
                        optionSaveData.optionRequirements = new List<DialogueOptionRequirementSaveData>();
                        foreach (DialogueOptionRequirement requirement in dialogueNodes[i].dialogueNode.dialogueOptions[j].optionRequirements)
                        {
                            DialogueOptionRequirementSaveData requirementSaveData = new DialogueOptionRequirementSaveData();
                            requirementSaveData.requirementType = requirement.requirementType;
                            requirementSaveData.factionRequirement = requirement.factionRequirement;
                            requirementSaveData.comparisonOperator = requirement.comparisonOperator;
                            requirementSaveData.intValue = requirement.intValue;
                            if (requirement.requiredItem != null)
                            {
                                requirementSaveData.requiredItemBaseID = requirement.requiredItem.baseItemID;
                            }
                            optionSaveData.optionRequirements.Add(requirementSaveData);
                        }
                    }
                    nodeSaveData.dialogueOptions.Add(optionSaveData);
                }
            }
            if (dialogueNodes[i].dialogueNode.dialogueEvents != null && dialogueNodes[i].dialogueNode.dialogueEvents.Count > 0)
            {
                nodeSaveData.dialogueEvents = new List<DialogueEvent>();
                foreach (DialogueEvent evt in dialogueNodes[i].dialogueNode.dialogueEvents)
                {
                    nodeSaveData.dialogueEvents.Add(evt);
                }
            }
            treeSaveData.dialogueNodes.Add(nodeSaveData);
        }
        // string newFilePath = Application.persistentDataPath + "/Resources/Dialogue/" + fileName;
        // string newFilePath = Application.persistentDataPath + "/" + fileName + ".json";
        treeSaveData.dialogueNodes.Sort((p1,p2) => p1.nodeIndex.CompareTo(p2.nodeIndex));
        string jsonSaveinfo = JsonConvert.SerializeObject(treeSaveData, Formatting.Indented, new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
        File.WriteAllText(newFilePath, jsonSaveinfo);
    }
}
