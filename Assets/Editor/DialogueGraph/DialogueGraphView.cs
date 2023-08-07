using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    private readonly Vector2 defaultNodeSize = new Vector2(150,200);
    StyleSheet dialogueOptionStyleSheet;

    public DialogueGraphView()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        GridBackground grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement(GenerateEntryPointNode());
    }

    public DialogueGraphNode GenerateEntryPointNode()
    {
        DialogueGraphNode node = new DialogueGraphNode();
        node.dialogueNode = new DialogueNode();
        node.title = "Start";
        node.dialogueNode.nodeIndex = 0;

        Port generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }

    public Port GeneratePort(DialogueGraphNode node, Direction portDir, Port.Capacity capacity=Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDir, capacity, typeof(float));
    }



    public DialogueGraphNode CreateDialogueNode(string nodeName, Vector2 position, DialogueNode saveFileNode=null)
    {
        DialogueGraphNode graphNode = new DialogueGraphNode();
        graphNode.dialogueNode = new DialogueNode();
        graphNode.dialogueNode.dialogueOptions = new List<DialogueOption>();
        graphNode.dialogueNode.dialogueAudioFileName = string.Empty;
        graphNode.dialogueNode.nodeIndex = nodes.Count();
        graphNode.dialogueNode.destinationNodeIndex = -1;
        if (saveFileNode != null)
        {
            // dialogueNode = saveFileNode;
            graphNode.dialogueNode.dialogueAudioFileName = saveFileNode.dialogueAudioFileName;
            graphNode.dialogueNode.nodeIndex = saveFileNode.nodeIndex;
            graphNode.dialogueNode.destinationNodeIndex = saveFileNode.destinationNodeIndex;
            graphNode.dialogueNode.characterSpeaking = saveFileNode.characterSpeaking;
            graphNode.dialogueNode.graphPosition = saveFileNode.graphPosition;
            // dialogueNode.dialogueEvents = saveFileNode.dialogueEvents;
            // if (dialogueNode.dialogueEvents == null)
            // {
            //     dialogueNode.dialogueEvents = new List<DialogueEvent>();
            // }
        }
        graphNode.dialogueNode.dialogueEvents = new List<DialogueEvent>();
        // Debug.Log(graphNode.dialogueNode.dialogueEvents.Count);


        Port inputPort = GeneratePort(graphNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Source";
        graphNode.inputContainer.Add(inputPort);

        Port outputPort = GeneratePort(graphNode, Direction.Output, Port.Capacity.Multi);
        outputPort.portName = "Default Output";
        outputPort.AddManipulator(new EdgeConnector<Edge>(new DialogueEdgeConnectorListener(graphNode)));
        graphNode.outputContainer.Add(outputPort);

        Button button = new(() => { AddChoicePort(graphNode); })
        {
            text = "New Dialogue Option"
        };
        graphNode.titleContainer.Add(button);

        TextField characterSpeaking = new("Character Speaking")
        {
            multiline = true,
            maxLength = 100
        };
        characterSpeaking.style.whiteSpace = WhiteSpace.Normal;
        characterSpeaking.style.maxWidth = 500;
        if (saveFileNode != null)
        {
            characterSpeaking.value = saveFileNode.characterSpeaking;
        }
        characterSpeaking.RegisterValueChangedCallback((evt) => graphNode.dialogueNode.characterSpeaking = characterSpeaking.value);
        graphNode.mainContainer.Add(characterSpeaking);

        TextField nodeText = new("Dialogue Node Text")
        {
            // Make textfield multiline
            multiline = true
        };
        // Give it a size so it doesn't stretch too far.
        nodeText.style.maxWidth = 500;
        // This is a buried property that has so far
        // been the only one that works in making the text wrap.
        nodeText.style.whiteSpace = WhiteSpace.Normal;
        if (saveFileNode != null)
        {
            Debug.Log(saveFileNode.nodeIndex + " " + saveFileNode.dialogueText);
            nodeText.value = saveFileNode.dialogueText;
            graphNode.dialogueNode.dialogueText = saveFileNode.dialogueText;
        }
        nodeText.RegisterValueChangedCallback((evt) => { graphNode.dialogueNode.dialogueText = nodeText.value; Debug.Log(graphNode.dialogueNode.dialogueText); });
        graphNode.mainContainer.Add(nodeText);

        FloatField silenceLengthField = new("Silence Length")
        {
            name = "Silence Length",
            tooltip = "For use when there's no audio file but we want the node to last a certain time."
        };
        if (saveFileNode != null)
        {
            if (saveFileNode.silenceLength > 0)
            {
                silenceLengthField.value = saveFileNode.silenceLength;
            }
        }
        else
        {
            silenceLengthField.value = 3.0f;
        }
        silenceLengthField.RegisterValueChangedCallback((evt) => graphNode.dialogueNode.silenceLength = silenceLengthField.value);
        graphNode.mainContainer.Add(silenceLengthField);

        ObjectField nodeAudio = new ObjectField("Dialogue Voice Line");
        nodeAudio.objectType = typeof(AudioClip);
        nodeAudio.name = "Dialogue Voice Line";
        Label audioLength = new Label();
        Image texture = new Image();
        if (saveFileNode != null)
        {
            if (graphNode.dialogueNode.dialogueAudioFileName != string.Empty && graphNode.dialogueNode.dialogueAudioFileName != null)
            {
                nodeAudio.value = Resources.Load<AudioClip>("Voiceover/" + graphNode.dialogueNode.characterSpeaking + "/" + graphNode.dialogueNode.dialogueAudioFileName);
                graphNode.dialogueNode.dialogueAudio = (AudioClip)nodeAudio.value;
                texture.sprite = PaintWaveformSpectrum(graphNode.dialogueNode.dialogueAudio, 1, 300, 50, Color.yellow);
                Debug.Log(graphNode.dialogueNode.dialogueAudioFileName);
                audioLength.text = "Audio File Length: " + graphNode.dialogueNode.dialogueAudio.length;
            }
        }
        nodeAudio.RegisterValueChangedCallback((evt) => graphNode.dialogueNode.dialogueAudio = (AudioClip)nodeAudio.value);
        nodeAudio.RegisterValueChangedCallback((evt) =>
        {
            if (graphNode.dialogueNode.dialogueAudio != null)
            {
                audioLength.text = ("Audio File Length: " + graphNode.dialogueNode.dialogueAudio.length);
            }
            else
            {
                audioLength.text = string.Empty;
            }
        });
        nodeAudio.RegisterValueChangedCallback((evt) => texture.sprite = PaintWaveformSpectrum(graphNode.dialogueNode.dialogueAudio, 1, 300, 50, Color.yellow));
        
        graphNode.mainContainer.Add(nodeAudio);
        graphNode.mainContainer.Add(audioLength);
        graphNode.mainContainer.Add(texture);

        
        if (saveFileNode != null)
        {
            // Debug.Log("Number of dialogue events " + saveFileNode.dialogueEvents.Count);
            if (saveFileNode.dialogueOptions != null)
            {
                foreach (DialogueOption option in saveFileNode.dialogueOptions)
                {
                    AddChoicePort(graphNode, option);
                }
            }
            if (saveFileNode.dialogueEvents != null && saveFileNode.dialogueEvents.Count > 0)
            {
                Debug.Log("Number of dialogue events " + saveFileNode.dialogueEvents.Count);
                foreach (DialogueEvent dialogueEvent in saveFileNode.dialogueEvents)
                {
                    AddDialogueEvent(graphNode, dialogueEvent);
                }
            }
        }

        Button addAnimationEventOption = new Button(() => { AddDialogueEvent(graphNode); });
        addAnimationEventOption.text = "Add Dialogue Event";
        graphNode.mainContainer.Add(addAnimationEventOption);


        // Add node index to name to make it more clear
        graphNode.title = nodeName + " " + graphNode.dialogueNode.nodeIndex;
        Debug.Log(graphNode.title);

        graphNode.RefreshExpandedState();
        graphNode.RefreshPorts();
        graphNode.SetPosition(new Rect(position, defaultNodeSize));
        if (saveFileNode != null)
        {
            graphNode.SetPosition(new Rect(graphNode.dialogueNode.graphPosition, defaultNodeSize));
        }
        return graphNode;
    }

    public void AddDialogueEvent(DialogueGraphNode graphNode, DialogueEvent savedEvent=null)
    {
        DialogueEvent dialogueEvent = new DialogueEvent();
        Foldout animationEventObject = new Foldout();
        Button removeEventButton = new Button(() => { RemoveAnimationEvent(graphNode, animationEventObject, dialogueEvent); });
        removeEventButton.text = "Remove Event";
        EnumField eventType = new EnumField("Event Type:", DialogueEventType.PlayAnimation);
        eventType.tooltip = "Determines what function type params correspond to.";
        eventType.RegisterValueChangedCallback((evt) => dialogueEvent.eventType = (DialogueEventType)eventType.value);
        FloatField timeStampValue = new FloatField("Timestamp:");
        timeStampValue.tooltip = "Time event is played during audio";
        timeStampValue.RegisterValueChangedCallback((evt) => dialogueEvent.invokeTime = timeStampValue.value);
        TextField methodName = new TextField("Method Name:");
        methodName.tooltip = "Use this for methods being called or animation names";
        methodName.RegisterValueChangedCallback((evt) => dialogueEvent.eventName = methodName.value);
        TextField stringParam = new TextField("String Value:");
        stringParam.RegisterValueChangedCallback((evt) => dialogueEvent.stringParameter = stringParam.value);
        IntegerField intParam = new IntegerField("Integer Value:");
        intParam.RegisterValueChangedCallback((evt) => dialogueEvent.intParameter = intParam.value);
        FloatField floatParam = new FloatField("Float Value:");
        floatParam.RegisterValueChangedCallback((evt) => dialogueEvent.floatParameter = floatParam.value);
        if (savedEvent != null)
        {
            dialogueEvent.eventName = savedEvent.eventName;
            dialogueEvent.eventType = savedEvent.eventType;
            dialogueEvent.floatParameter = savedEvent.floatParameter;
            dialogueEvent.intParameter = savedEvent.intParameter;
            dialogueEvent.invokeTime = savedEvent.invokeTime;
            dialogueEvent.stringParameter = savedEvent.stringParameter;
            eventType.value = dialogueEvent.eventType;
            timeStampValue.value = dialogueEvent.invokeTime;
            methodName.value = dialogueEvent.eventName;
            stringParam.value = dialogueEvent.stringParameter;
            floatParam.value = dialogueEvent.floatParameter;
            intParam.value = dialogueEvent.intParameter;
        }
        animationEventObject.Add(eventType);
        animationEventObject.Add(timeStampValue);
        animationEventObject.Add(methodName);
        animationEventObject.Add(stringParam);
        animationEventObject.Add(intParam);
        animationEventObject.Add(floatParam);
        animationEventObject.Add(removeEventButton);
        graphNode.mainContainer.Add(animationEventObject);
        graphNode.dialogueNode.dialogueEvents.Add(dialogueEvent);
    }

    private void RemoveAnimationEvent(DialogueGraphNode node, VisualElement animationEventObject, DialogueEvent evt)
    {
        node.mainContainer.Remove(animationEventObject);
        node.dialogueNode.dialogueEvents.Remove(evt);
    }

    public Port AddChoicePort(DialogueGraphNode graphNode, DialogueOption saveFileOption=null)
    {
        DialogueOption option = new DialogueOption();
        option.optionRequirements = new List<DialogueOptionRequirement>();
        option.optionEffects = new List<DialogueOptionEffect>();
        option.destinationNodeIndex = -1;
        if (saveFileOption != null)
        {
            option.destinationNodeIndex = saveFileOption.destinationNodeIndex;
            option.dialogueOptionText = saveFileOption.dialogueOptionText;
        }

        Port generatedPort = GeneratePort(graphNode, Direction.Output);
        generatedPort.AddManipulator(new EdgeConnector<Edge>(new DialogueEdgeConnectorListener(option)));

        int outputPortcount = graphNode.outputContainer.Query("connector").ToList().Count;
        string outputPortName = string.Empty;
        generatedPort.portName = outputPortName;

        Button deleteOptionButton = new Button(() => RemovePort(graphNode, generatedPort, option));
        deleteOptionButton.text = "X";

        VisualElement optionContainer = new VisualElement();
        VisualElement preReqContainer = new VisualElement();
        VisualElement optionEffectsContainer = new VisualElement();


        dialogueOptionStyleSheet = (StyleSheet) EditorGUIUtility.Load("DialogueOption.uss");
        optionContainer.styleSheets.Add(dialogueOptionStyleSheet);
        optionContainer.AddToClassList("dialogue-option");

        EnumField preReqType = new EnumField("Option Pre-Requisites", DialogueOptionRequirementType.PlayerHasItem);
        preReqType.tooltip = "Pre-requisite types for dialogue option to be visible to player.";
        preReqType.style.flexDirection = FlexDirection.Column;

        Button addOptionPreReqButton = new Button(() => AddOptionPreRequisite(graphNode, option, preReqContainer, preReqType));
        addOptionPreReqButton.text = "Add";
        addOptionPreReqButton.tooltip = "Add selected option pre-requisite.";

        EnumField effectType = new EnumField("Option Effects", DialogueOptionEffectType.AddItem);
        effectType.tooltip = "Possible effects of player selecting this option.";
        effectType.style.flexDirection = FlexDirection.Column;

        Button addOptionEffectButton = new Button(() => AddOptionEffect(graphNode, option, optionEffectsContainer, effectType));
        addOptionEffectButton.text = "Add";
        addOptionEffectButton.tooltip = "Add selected option effect.";

        TextField text = new TextField();
        text.value = $"Option {outputPortcount}";
        // Make textfield multiline
        text.multiline = true;
        // Give it a size so it doesn't stretch too far.
        text.style.maxWidth = 350;
        text.style.minWidth = text.style.maxWidth;
        generatedPort.style.height = -1;
        generatedPort.style.alignItems = Align.FlexStart;
        // This is a buried property that has so far
        // been the only one that works in making the text wrap.
        text.style.whiteSpace = WhiteSpace.Normal;
        if (saveFileOption != null)
        {
            text.value = saveFileOption.dialogueOptionText;
        }
        option.dialogueOptionText = text.value;
        text.RegisterValueChangedCallback((evt) => option.dialogueOptionText = text.value);

        
        if (saveFileOption != null)
        {
            if (saveFileOption.optionRequirements != null && saveFileOption.optionRequirements.Count > 0)
            {
                foreach (DialogueOptionRequirement requirement in saveFileOption.optionRequirements)
                {
                    AddOptionPreRequisite(graphNode, option, preReqContainer, preReqType, requirement);
                }
            }
            if (saveFileOption.optionEffects != null && saveFileOption.optionEffects.Count > 0)
            {
                foreach (DialogueOptionEffect effect in saveFileOption.optionEffects)
                {
                    AddOptionEffect(graphNode, option, optionEffectsContainer, effectType, effect);
                }
            }
        }


        Label preRequisiteLabel = new Label("Option Pre-Requisites");
        Label optionEffectsLabel = new Label("Option Effects");

        generatedPort.Add(addOptionEffectButton);
        generatedPort.Add(effectType);
        generatedPort.Add(addOptionPreReqButton);
        generatedPort.Add(preReqType);
        generatedPort.Add(optionContainer);
        optionContainer.Add(text);
        optionContainer.Add(preReqContainer);
        optionContainer.Add(optionEffectsContainer);
        preReqContainer.Add(preRequisiteLabel);
        optionEffectsContainer.Add(optionEffectsLabel);
        generatedPort.Add(deleteOptionButton);

        graphNode.dialogueNode.dialogueOptions.Add(option);
        graphNode.outputContainer.Add(generatedPort);
        graphNode.RefreshPorts();
        graphNode.RefreshExpandedState();

        return generatedPort;
    }

    public void AddOptionEffect(DialogueGraphNode graphNode, DialogueOption option, VisualElement optionEffectsContainer, EnumField effectType, DialogueOptionEffect saveFileEffect=null)
    {
        DialogueOptionEffect optionEffect = new DialogueOptionEffect();
        optionEffect.AddToClassList("dialogue-option-pre-req");
        optionEffect.AddToClassList("dialogue-option-pre-req-row");
        optionEffect.effectType = (DialogueOptionEffectType)effectType.value;
        if (saveFileEffect != null)
        {
            optionEffect.faction = saveFileEffect.faction;
            optionEffect.effectType = saveFileEffect.effectType;
            optionEffect.intValue = saveFileEffect.intValue;
            optionEffect.item = saveFileEffect.item;
            optionEffect.stringValue = saveFileEffect.stringValue;
            // if (saveFileEffect.item != null)
            // {
            //     Debug.Log(saveFileEffect.item.baseItemID);
            //     optionEffect.item = Resources.LoadAll<Item>("ScriptableObject/").Where(x => x.baseItemID == saveFileEffect.item.baseItemID).First();
            // }
        }
        VisualElement leftPanel = new VisualElement();
        VisualElement rightPanel = new VisualElement();


        Button removeEffectButton = new Button(() => RemoveOptionEffect(graphNode, option, optionEffectsContainer, optionEffect));
        removeEffectButton.text = "X";
        leftPanel.Add(removeEffectButton);

        VisualElement optionPreReqContent = new VisualElement();
        optionPreReqContent = PopulateDialogueOptionEffectContent(optionEffect);
        rightPanel.Add(optionPreReqContent);

        optionEffect.Add(leftPanel);
        optionEffect.Add(rightPanel);
        optionEffectsContainer.Add(optionEffect);
        option.optionEffects.Add(optionEffect);

        graphNode.RefreshPorts();
        graphNode.RefreshExpandedState();
    }

    public VisualElement PopulateDialogueOptionEffectContent(DialogueOptionEffect effect)
    {
        DialogueOptionEffectType value = effect.effectType;
        VisualElement effectContainer = new VisualElement();
        Label title = new Label(value.ToString());
        effectContainer.Add(title);
        switch(value)
        {
            case DialogueOptionEffectType.AddItem:
                ObjectField itemField = new ObjectField("Base Item:");
                itemField.objectType = typeof(Item);
                itemField.RegisterValueChangedCallback((evt) => effect.item = (Item)itemField.value);
                IntegerField quantityField = new IntegerField("Quantity:");
                quantityField.RegisterValueChangedCallback((evt) => effect.intValue = quantityField.value);
                if (effect.item != null)
                {
                    itemField.value = effect.item;
                    quantityField.value = effect.intValue;
                }
                effectContainer.Add(itemField);
                effectContainer.Add(quantityField);
            break;
            case DialogueOptionEffectType.RemoveItem:
                itemField = new ObjectField("Base Item:");
                itemField.objectType = typeof(Item);
                itemField.RegisterValueChangedCallback((evt) => effect.item = (Item)itemField.value);
                quantityField = new IntegerField("Quantity:");
                quantityField.RegisterValueChangedCallback((evt) => effect.intValue = quantityField.value);
                if (effect.item != null)
                {
                    Debug.Log(effect.item);
                    itemField.value = effect.item;
                    quantityField.value = effect.intValue;
                }
                effectContainer.Add(itemField);
                effectContainer.Add(quantityField);
            break;
            case DialogueOptionEffectType.ChangeOpinion:
                IntegerField opinionValue = new IntegerField("Opinion Change:");
                opinionValue.RegisterValueChangedCallback((evt) => effect.intValue = opinionValue.value);
                if (effect.intValue != 0)
                {
                    opinionValue.value = effect.intValue;
                }
                effectContainer.Add(opinionValue);
            break;
            case DialogueOptionEffectType.ChangeFactionStanding:
                EnumField faction = new EnumField(Factions.Player);
                faction.RegisterValueChangedCallback((evt) => effect.faction = (Factions)faction.value);
                opinionValue = new IntegerField("Opinion Change:");
                opinionValue.RegisterValueChangedCallback((evt) => effect.intValue = opinionValue.value);
                if (effect.intValue != 0)
                {
                    faction.value = effect.faction;
                    opinionValue.value = effect.intValue;
                }
                effectContainer.Add(faction);
                effectContainer.Add(opinionValue);
            break;
            case DialogueOptionEffectType.ChangeDialogueTree:
                TextField dialogueTreePath = new TextField("Dialogue Tree Path:");
                dialogueTreePath.RegisterValueChangedCallback((evt) => effect.stringValue = dialogueTreePath.value);
                if (effect.stringValue != null && effect.stringValue != System.String.Empty)
                {
                    dialogueTreePath.value = effect.stringValue;
                }
                effectContainer.Add(dialogueTreePath);
            break;
            case DialogueOptionEffectType.ChangeNPCStat:
                EnumField statTypefield = new EnumField(StatType.Health);
                if (effect.stringValue != null && effect.stringValue != System.String.Empty)
                {
                    StatType typeValue;
                    Enum.TryParse<StatType>(effect.stringValue, out typeValue);
                    statTypefield.value = typeValue;
                }
                statTypefield.RegisterValueChangedCallback((evt) => effect.stringValue = statTypefield.value.ToString());
                IntegerField statValueChangeField = new IntegerField("Stat Change Value:");
                statValueChangeField.RegisterValueChangedCallback((evt) => effect.intValue = statValueChangeField.value);
                effectContainer.Add(statTypefield);
                effectContainer.Add(statValueChangeField);
            break;
        }
        return effectContainer;
    }

    public void RemoveOptionEffect(DialogueGraphNode graphNode, DialogueOption option, VisualElement optionEffectsContainer, DialogueOptionEffect optionEffect)
    {
        option.optionEffects.Remove(optionEffect);
        optionEffectsContainer.Remove(optionEffect);
        graphNode.RefreshPorts();
        graphNode.RefreshExpandedState();
    }

    public void RemoveOptionPreRequisite(DialogueGraphNode node, DialogueOption option, VisualElement customCont, DialogueGraphOptionRequirement optionRequirement)
    {
        option.optionRequirements.Remove(optionRequirement.dialogueOptionRequirement);
        customCont.Remove(optionRequirement);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    public void AddOptionPreRequisite(DialogueGraphNode node, DialogueOption option, VisualElement container, EnumField preReqType, DialogueOptionRequirement requirement=null)
    {
        DialogueGraphOptionRequirement optionRequirement = new DialogueGraphOptionRequirement();
        optionRequirement.dialogueOptionRequirement = new DialogueOptionRequirement();
        optionRequirement.AddToClassList("dialogue-option-pre-req");
        optionRequirement.AddToClassList("dialogue-option-pre-req-row");
        if (requirement != null)
        {
            optionRequirement.dialogueOptionRequirement = requirement;
            preReqType.value = requirement.requirementType;
            // if (requirement.requiredItem != null)
            // {
            //     optionRequirement.dialogueOptionRequirement.requiredItem = Resources.LoadAll<Item>("ScriptableObject/").Where(x => x.baseItemID == requirement.requiredItem.baseItemID).First();
            // }
        }
        else
        {
            optionRequirement.dialogueOptionRequirement.requirementType = (DialogueOptionRequirementType)preReqType.value;
        }
        VisualElement leftPanel = new VisualElement();
        VisualElement rightPanel = new VisualElement();


        Button removePreReqButton = new Button(() => RemoveOptionPreRequisite(node, option, container, optionRequirement));
        removePreReqButton.text = "X";
        leftPanel.Add(removePreReqButton);

        VisualElement optionPreReqContent = new VisualElement();
        optionPreReqContent = PopulateDialogueOptionPreReqContent(optionRequirement);
        rightPanel.Add(optionPreReqContent);

        optionRequirement.Add(leftPanel);
        optionRequirement.Add(rightPanel);
        container.Add(optionRequirement);

        option.optionRequirements.Add(optionRequirement.dialogueOptionRequirement);

        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    public VisualElement PopulateDialogueOptionPreReqContent(DialogueGraphOptionRequirement optionRequirement)
    {
        DialogueOptionRequirementType value = optionRequirement.dialogueOptionRequirement.requirementType;
        VisualElement preReqContainer = new VisualElement();
        Label title = new Label(value.ToString());
        preReqContainer.Add(title);
        switch (value)
        {
            case DialogueOptionRequirementType.PlayerHasItem:
                ObjectField itemField = new ObjectField("Base Item:");
                itemField.objectType = typeof(Item);
                itemField.RegisterValueChangedCallback((evt) => optionRequirement.dialogueOptionRequirement.requiredItem = (Item)itemField.value);
                IntegerField quantityField = new IntegerField("Quantity:");
                quantityField.RegisterValueChangedCallback((evt) => optionRequirement.dialogueOptionRequirement.intValue = quantityField.value);
                if (optionRequirement.dialogueOptionRequirement.requiredItem != null)
                {
                    itemField.value = optionRequirement.dialogueOptionRequirement.requiredItem;
                    quantityField.value = optionRequirement.dialogueOptionRequirement.intValue;
                }
                preReqContainer.Add(itemField);
                preReqContainer.Add(quantityField);
            break;
            case DialogueOptionRequirementType.FactionStanding:
                EnumField faction = new EnumField("Faction:", Factions.Player);
                faction.RegisterValueChangedCallback((evt) => optionRequirement.dialogueOptionRequirement.factionRequirement = (Factions)faction.value);
                EnumField operatorField = new EnumField("Operator:", DialogueOptionOperators.EqualTo);
                operatorField.RegisterValueChangedCallback((evt) => optionRequirement.dialogueOptionRequirement.comparisonOperator = (DialogueOptionOperators)operatorField.value);
                IntegerField factionOpinion = new IntegerField("Opinion Value:");
                factionOpinion.RegisterValueChangedCallback((evt) => optionRequirement.dialogueOptionRequirement.intValue = factionOpinion.value);
                // if (optionRequirement.dialogueOptionRequirement.factionRequirement != Factions.Player)
                // {
                    faction.value = optionRequirement.dialogueOptionRequirement.factionRequirement;
                    operatorField.value = optionRequirement.dialogueOptionRequirement.comparisonOperator;
                    factionOpinion.value = optionRequirement.dialogueOptionRequirement.intValue;
                // }
                preReqContainer.Add(faction);
                preReqContainer.Add(operatorField);
                preReqContainer.Add(factionOpinion);
                Debug.Log("Added faction standing requirement");
            break;
            case DialogueOptionRequirementType.CharacterOpinion:
                operatorField = new EnumField("Operator:", DialogueOptionOperators.EqualTo);
                operatorField.RegisterValueChangedCallback((evt) => optionRequirement.dialogueOptionRequirement.comparisonOperator = (DialogueOptionOperators)operatorField.value);
                IntegerField characterOpinion = new IntegerField("Opinion Value:");
                characterOpinion.RegisterValueChangedCallback((evt) => optionRequirement.dialogueOptionRequirement.intValue = characterOpinion.value);
                if (optionRequirement.dialogueOptionRequirement.intValue != null)
                {
                    operatorField.value = optionRequirement.dialogueOptionRequirement.comparisonOperator;
                    characterOpinion.value = optionRequirement.dialogueOptionRequirement.intValue;
                }
                preReqContainer.Add(operatorField);
                preReqContainer.Add(characterOpinion);
            break;
        }
        return preReqContainer;
    }

    public void RemovePort(DialogueGraphNode node, Port generatedPort, DialogueOption option)
    {
        IEnumerable<Edge> targetEdge = edges.ToList().Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);
        // Debug.Log(generatedPort.portName);
        

        if (targetEdge.Any())
        {
            Edge edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }
        node.dialogueNode.dialogueOptions.Remove(option);
        node.outputContainer.Remove(generatedPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    public DialogueGraphNode CreateNode(string nodeName, Vector2 position, DialogueNode saveFileNode=null)
    {
        DialogueGraphNode createdNode = CreateDialogueNode(nodeName, position, saveFileNode);
        if (createdNode != null)
        {
            AddElement(createdNode);
        }
        return createdNode;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();

        ports.ForEach((port) => 
        { 
            if (startPort != port && startPort.node != port.node)
            {
                compatiblePorts.Add(port);
            }
        });

        return compatiblePorts;
    }

    public Sprite PaintWaveformSpectrum(AudioClip audio, float saturation, int width, int height, Color col) {
        if (audio is null)
        {
            return null;
        }
        Sprite newSprite;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        float[] samples = new float[audio.samples * audio.channels];
        float[] waveform = new float[width];
        audio.GetData(samples, 0);
        int packSize = ( samples.Length / width ) + 1;
        int s = 0;
        for (int i = 0; i < width; i++) {
            waveform[i] = Mathf.Abs(samples[i * packSize]);
            s++;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(x, y, Color.gray);
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < waveform[x] * height; y++)
            {
                tex.SetPixel(x, (height / 2) + y, col);
                tex.SetPixel(x, (height / 2) - y, col);
            }
        }
        tex.Apply();
        Rect rec = new Rect(0,0,width,height);
        newSprite = Sprite.Create(tex, rec, new Vector2(0,0),1);
        return newSprite;
    }

    public int ConnectAllPorts()
    {
        Edge newEdge;
        foreach (Port port in ports)
        {
            DialogueGraphNode dn = port.node as DialogueGraphNode;
            if (dn != null && dn.dialogueNode.destinationNodeIndex != -1 && port.direction == Direction.Output)
            {
                // This if statement is for connecting the entry node.
                // There's a better way but I just want this to work.
                if (dn.dialogueNode.nodeIndex > 0  && dn.dialogueNode.destinationNodeIndex != -1)
                {
                    Debug.Log("dest node index " + dn.dialogueNode.nodeIndex + " " + dn.dialogueNode.destinationNodeIndex);
                    newEdge = port.ConnectTo(nodes.ToList()[dn.dialogueNode.destinationNodeIndex].inputContainer.Q<Port>());
                }
                else
                {
                    newEdge = port.ConnectTo(nodes.ToList()[1].inputContainer.Q<Port>());
                }
                if (newEdge == null)
                {
                    return 1;
                }
                dn.Add(newEdge);
            }
        }
        return 0;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        base.BuildContextualMenu(evt);
        if (evt.target is DialogueGraphView || evt.target is DialogueGraphNode)
        {
            Vector2 localMousePos = evt.localMousePosition;
            Vector2 actualGraphPosition = viewTransform.matrix.inverse.MultiplyPoint(localMousePos );
            evt.menu.AppendAction("New Dialogue Node", (e) => { CreateNode("Dialogue Node", actualGraphPosition, null); });
            evt.menu.AppendAction("New Start Node", (e) => { AddElement(GenerateEntryPointNode()); });
        }
    }
}
