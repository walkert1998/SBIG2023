using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueScreen : MonoBehaviour
{
    public static DialogueScreen instance;
    private DialogueTree tree;
    public GameObject dialogueScreenUI;
    public TMP_Text npcName;
    public TMP_Text nodeText;
    public GameObject optionPrefab;
    public Transform optionsList;
    public AudioSource[] characterAudioSources;
    public CharacterInstance[] characters;
    AudioSource characterSource;
    private int selectedOption = -2;
    public NPCConversation conversation;
    // public float expressionTime;
    public Inventory playerInventory;
    public ThirdPersonController playerController;
    public Camera monologueCamera;
    CharacterInstance talkingCharacter;
    int validOptionCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        dialogueScreenUI.SetActive(false);
        characterSource = new AudioSource();
        // StartConversation(conversation);
        // #if UNITY_EDITOR
        // QualitySettings.vSyncCount = 0;
        // Application.targetFrameRate = 60;
        // #endif
    }

    public void RunDialogueTree(DialogueTree newTree)
    {
        tree = newTree;
        selectedOption = -2;
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        dialogueScreenUI.SetActive(true);
        ClearOptionsList();
        int nodeIndex = 0;
        while (nodeIndex != -1)
        {
            // Debug.Log(nodeIndex);
            DisplayNode(nodeIndex);
            if (selectedOption != -1)
            {
                selectedOption = -2;
            }
            while (selectedOption == -2)
            {
                if (nodeIndex == -1)
                {
                    break;
                }
                // Debug.Log("Looping " + selectedOption);
                yield return null;
            }
            nodeIndex = selectedOption;
        }
        if (characterSource != null)
        {
            characterSource.Stop();
        }
        ClearOptionsList();
        foreach (CharacterInstance character in characters)
        {
            if (character.headLook != null)
            {
                character.headLook.SetTarget(null);
            }
        }
        // Debug.Log("Hiding UI");
        dialogueScreenUI.SetActive(false);
        CameraSwitcher.SwitchToPlayerCamera();
        playerController.LockMovement(true);
        playerController.GetComponent<Animator>().Play("Idle Walk Run Blend");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DisplayNode(int index)
    {
        validOptionCount = 0;
        DialogueNode node = tree.dialogueNodes[index];
        nodeText.text = node.dialogueText;
        npcName.text = node.characterSpeaking;
        ClearOptionsList();
        optionsList.gameObject.SetActive(false);
        talkingCharacter = null;
        if (node.characterSpeaking == "Internal Monologue")
        {
            monologueCamera.gameObject.SetActive(true);
            npcName.text = "Hugh Mann";
            npcName.color = Color.cyan;
        }
        else
        {
            monologueCamera.gameObject.SetActive(false);
            foreach (CharacterInstance character in characters)
            {
                if (character.activeConversation.characterName == node.characterSpeaking)
                {
                    talkingCharacter = character;
                    CameraSwitcher.SwitchCameraTo(talkingCharacter.vCam);
                    npcName.color = talkingCharacter.subtitlecolour;

                }
                // Debug.Log("Character: " + character.activeConversation.characterName + " node: " + node.characterSpeaking + " " + node.nodeIndex);
            }

            foreach (CharacterInstance character in characters)
            {
                if (talkingCharacter != null && talkingCharacter != character && character.headLook != null)
                {
                    character.headLook.SetTarget(talkingCharacter.headTarget);
                }
            }
            // Debug.Log("Character speaking: " + talkingCharacter.activeConversation.characterName);
        }
        // Debug.Log("length of silence: " + node.silenceLength);
        if (node.dialogueAudioFileName != string.Empty && node.dialogueAudioFileName != null)
        {
            StartCoroutine(PlayDialogueLine(node));
        }
        else if (node.silenceLength > 0)
        {
            StartCoroutine(WaitForSilence(node));
        }
        // else if (node.dialogueOptions != null && node.dialogueOptions.Count > 0)
        // {
        //     PopulateOptionsList(node);
        //     optionsList.gameObject.SetActive(true);
        // }
        else if (node.dialogueOptions == null || node.dialogueOptions.Count <= 0 || validOptionCount <= 0)
        {
            selectedOption = node.destinationNodeIndex;
            if (node.destinationNodeIndex > 0)
            {
                selectedOption--;
            }
            // Debug.Log(selectedOption);
        }
    }

    IEnumerator WaitForSilence(DialogueNode node)
    {
        float eventTimer = 0;
        float timer = 0;
        List<DialogueEvent> dialogueEvents;
        bool hasEvents = (node.dialogueEvents != null && node.dialogueEvents.Count > 0);
        foreach (CharacterInstance character in characters)
        {
            if (character.transform.root.name == node.characterSpeaking)
            {
                characterSource = character.source;
                talkingCharacter = character;
            }
        }
        characterSource.Stop();
        // characterSource.PlayOneShot(voiceLine);
        // Debug.Log("Playing voiceline");
        if (hasEvents)
        {
            dialogueEvents = node.dialogueEvents;
            while (timer < node.silenceLength)
            {
                timer += Time.deltaTime * Time.timeScale;
                eventTimer += Time.deltaTime * Time.timeScale;
                foreach (DialogueEvent evt in dialogueEvents)
                {
                    if (eventTimer < evt.invokeTime && evt.invoked == 1)
                    {
                        evt.invoked = 0;
                    }
                    if (eventTimer >= evt.invokeTime && evt.invoked == 0)
                    {
                        switch (evt.eventType)
                        {
                            case DialogueEventType.PlayAnimation:
                                talkingCharacter.animator.Play(evt.eventName,0);
                                talkingCharacter.animator.Play(evt.eventName,1);
                                talkingCharacter.animator.Play(evt.eventName,2);
                                evt.invoked = 1;
                                // Debug.Log("Play animation " + evt.eventName);
                            break;
                            case DialogueEventType.SetAnimationBoolValue:
                                bool val = (evt.intParameter == 1) ? true : false;
                                talkingCharacter.animator.SetBool(evt.eventName, val);
                                evt.invoked = 1;
                                // Debug.Log("Set animation bool " + evt.eventName + " to " + val);
                            break;
                            case DialogueEventType.SetFacialPose:
                                StartCoroutine(DialogueEventFacialExpression(evt));
                                evt.invoked = 1;
                                // Debug.Log("Set facial pose " + evt.eventName);
                            break;
                        }
                    }
                }
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(node.silenceLength);
        }
        
        // Debug.Log(node.dialogueOptions);

        if (node.dialogueOptions != null && node.dialogueOptions.Count > 0)
        {
            PopulateOptionsList(node);
        }
        optionsList.gameObject.SetActive(true);
        nodeText.text = string.Empty;
        if (node.dialogueOptions == null || node.dialogueOptions.Count <= 0 || validOptionCount <= 0)
        {
            selectedOption = node.destinationNodeIndex;
            if (node.destinationNodeIndex > 0)
            {
                selectedOption--;
            }
        }
    }

    IEnumerator PlayDialogueLine(DialogueNode node)
    {
        node.dialogueAudio = Resources.Load<AudioClip>("Voiceover/" + node.characterSpeaking + "/" + node.dialogueAudioFileName);
        // Debug.Log(node.dialogueAudio);
        AudioClip voiceLine = node.dialogueAudio;
        float eventTimer = 0;
        List<DialogueEvent> dialogueEvents;
        bool hasEvents = (node.dialogueEvents != null && node.dialogueEvents.Count > 0);
        foreach (CharacterInstance character in characters)
        {
            if (character.transform.root.name == node.characterSpeaking)
            {
                characterSource = character.source;
                talkingCharacter = character;
            }
        }
        characterSource.Stop();
        characterSource.clip = voiceLine;
        // characterSource.PlayOneShot(voiceLine);
        // Debug.Log("Playing voiceline");
        characterSource.Play();
        if (hasEvents)
        {
            dialogueEvents = node.dialogueEvents;
            while (characterSource.isPlaying)
            {
                eventTimer += Time.deltaTime * Time.timeScale;
                foreach (DialogueEvent evt in dialogueEvents)
                {
                    if (eventTimer < evt.invokeTime && evt.invoked == 1)
                    {
                        evt.invoked = 0;
                    }
                    if (eventTimer >= evt.invokeTime && evt.invoked == 0)
                    {
                        switch (evt.eventType)
                        {
                            case DialogueEventType.PlayAnimation:
                                talkingCharacter.animator.Play(evt.eventName,0);
                                talkingCharacter.animator.Play(evt.eventName,1);
                                talkingCharacter.animator.Play(evt.eventName,2);
                                evt.invoked = 1;
                                // Debug.Log("Play animation " + evt.eventName);
                            break;
                            case DialogueEventType.SetAnimationBoolValue:
                                bool val = (evt.intParameter == 1) ? true : false;
                                talkingCharacter.animator.SetBool(evt.eventName, val);
                                evt.invoked = 1;
                                // Debug.Log("Set animation bool " + evt.eventName + " to " + val);
                            break;
                            case DialogueEventType.SetFacialPose:
                                StartCoroutine(DialogueEventFacialExpression(evt));
                                evt.invoked = 1;
                                // Debug.Log("Set facial pose " + evt.eventName);
                            break;
                            case DialogueEventType.KillNPC:
                            break;
                        }
                    }
                }
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(voiceLine.length);
        }
        
        // Debug.Log(node.dialogueOptions);

        if (node.dialogueOptions != null && node.dialogueOptions.Count > 0)
        {
            PopulateOptionsList(node);
        }
        optionsList.gameObject.SetActive(true);
        nodeText.text = string.Empty;
        characterSource.Stop();
        if (node.dialogueOptions == null || node.dialogueOptions.Count <= 0 || validOptionCount <= 0)
        {
            selectedOption = node.destinationNodeIndex;
            if (node.destinationNodeIndex > 0)
            {
                selectedOption--;
            }
        }
        // Debug.Log(selectedOption);
    }

    IEnumerator DialogueEventFacialExpression(DialogueEvent evt)
    {
        float timer = 0;
        while (timer < evt.floatParameter)
        {
            timer += Time.deltaTime;
            SetFacialExpression(evt, timer);
            yield return null;
        }
    }

    public void PopulateOptionsList(DialogueNode node)
    {
        for (int i = 0; i < node.dialogueOptions.Count; i++)
        {
            if (CheckOption(node.dialogueOptions[i]) == 0)
            {
                validOptionCount++;
                GameObject optionObject = Instantiate(optionPrefab, optionsList);
                SetOptionButton(node.dialogueOptions[i], optionObject, i);
            }
        }
    }

    public void SetOptionButton(DialogueOption option, GameObject button, int index)
    {
        button.GetComponent<Button>().interactable = true;
        button.transform.Find("OptionContent").GetComponent<TMP_Text>().text = option.dialogueOptionText;
        // button.transform.Find("OptionNumber").GetComponent<TMP_Text>().text = (index + 1).ToString();
        button.GetComponent<Button>().onClick.RemoveAllListeners();
        button.GetComponent<Button>().onClick.AddListener(
        delegate
        {
            if (option.optionEffects != null)
            {
                foreach (DialogueOptionEffect effect in option.optionEffects)
                {
                    switch (effect.effectType)
                    {
                        case DialogueOptionEffectType.ChangeFactionStanding:
                            FactionSystemManager.ChangeFactionOpinion(effect.faction, Factions.Player, effect.intValue);
                        break;
                        case DialogueOptionEffectType.AddItem:
                            Debug.Log(effect.item.baseItemID);
                            playerInventory.AddItem(effect.item, effect.intValue);
                        break;
                        case DialogueOptionEffectType.RemoveItem:
                            playerInventory.RemoveItemQuantity(effect.item, effect.intValue);
                        break;
                        case DialogueOptionEffectType.ChangeDialogueTree:
                            conversation.dialogueTreePath = effect.stringValue;
                        break;
                        default:
                        break;
                    }
                }
            }
        });
        button.GetComponent<Button>().onClick.AddListener(
        delegate
        {
            if (option.destinationNodeIndex > 0)
                selectedOption = option.destinationNodeIndex - 1;
            else
                selectedOption = option.destinationNodeIndex;
        });
    }

    public int CheckOption(DialogueOption option)
    {
        int optionValid = 0;
        if (option.optionRequirements != null && option.optionRequirements.Count > 0)
        {
            foreach (DialogueOptionRequirement requirement in option.optionRequirements)
            {
                switch (requirement.requirementType)
                {
                    case DialogueOptionRequirementType.PlayerHasItem:
                        if (playerInventory.FindItem(requirement.requiredItem) == null)
                        {
                            return 1;
                        }
                    break;
                    case DialogueOptionRequirementType.CharacterOpinion:
                    break;
                    case DialogueOptionRequirementType.FactionStanding:
                        return CheckFactionStanding(requirement);
                    break;
                    default:
                    break;
                }
            }
        }
        return optionValid;
    }

    public int CheckFactionStanding(DialogueOptionRequirement requirement)
    {
        int returnValue = 0;
        int value = FactionSystemManager.GetFactionOpinion(requirement.factionRequirement, Factions.Player);
        switch (requirement.comparisonOperator)
        {
            case DialogueOptionOperators.EqualTo:
                if (value != requirement.intValue)
                {
                    returnValue = 1;
                }
            break;
            case DialogueOptionOperators.GreaterThan:
                if (value < requirement.intValue)
                {
                    returnValue = 1;
                }
            break;
            case DialogueOptionOperators.LessThan:
                if (value > requirement.intValue)
                {
                    returnValue = 1;
                }
            break;
        }
        return returnValue;
    }

    public int CheckItem(Item itemToCheck)
    {
        return 1;
    }

    public void ClearOptionsList()
    {
        foreach (Transform child in optionsList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public static void StartConversation_Static(NPCConversation newConversation)
    {
        instance.StartConversation(newConversation);
    }

    public void StartConversation(NPCConversation newConversation)
    {
        conversation = newConversation;
        Debug.Log("running tree");
        PlayerInteraction.SetPrompt(System.String.Empty);
        playerController.LockMovement(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        TextAsset textAsset = Resources.Load<TextAsset>("Dialogue/" + newConversation.characterName + "/" + newConversation.dialogueTreePath);
        // Debug.Log(textAsset);
        tree = DialogueTree.LoadDialogue(textAsset.text);
        if (newConversation.GetComponent<NPCHeadLook>())
        {
            newConversation.GetComponent<CharacterInstance>().headLook.SetTarget(playerController.GetComponent<CharacterInstance>().headTarget);
        }
        playerController.GetComponent<CharacterInstance>().headLook.SetTarget(newConversation.GetComponent<CharacterInstance>().headTarget);
        RunDialogueTree(tree);
    }

    public void SetFacialExpression(DialogueEvent expressionEvent, float expressionTime)
    {
        int shapeKeyIndex = 0;
        SkinnedMeshRenderer[] skinnedMeshes = characterSource.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer skinnedMesh in skinnedMeshes)
        {
            shapeKeyIndex = skinnedMesh.sharedMesh.GetBlendShapeIndex(expressionEvent.eventName);
            if (shapeKeyIndex != -1)
            {
                float move = Mathf.Lerp(skinnedMesh.GetBlendShapeWeight(shapeKeyIndex), expressionEvent.intParameter, expressionTime / expressionEvent.floatParameter);
                // Debug.Log(expressionEvent.eventName + " " + move);
                skinnedMesh.SetBlendShapeWeight(shapeKeyIndex, move);
            }
        }
    }
}
