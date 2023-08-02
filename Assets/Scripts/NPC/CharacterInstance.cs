using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CharacterInstance : MonoBehaviour, IInteractable
{
    public Character characterTemplate;
    public NPCConversation activeConversation;
    public AudioSource source;
    public Animator animator;
    public CinemachineVirtualCamera vCam;
    public Transform headTarget;
    public NPCHeadLook headLook;

    public bool focused { get; set; }
    public bool holdToInteract { get; set; }

    public void EndInteraction()
    {
        
    }

    public void Examine()
    {
        
    }

    public void Focus()
    {
        focused = true;
        PlayerInteraction.SetPrompt("Talk to " + activeConversation.characterName);
    }

    public void StartInteraction()
    {
        DialogueScreen.StartConversation_Static(activeConversation);
        Debug.Log("Started interaction");
    }

    public void UnFocus()
    {
        focused = false;
        PlayerInteraction.SetPrompt(System.String.Empty);
    }

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerInteraction>())
        {
            PlayerInteraction.SetFocusObject_Static(this);
            other.GetComponent<NPCHeadLook>().target = headTarget;
            headLook.target = other.GetComponent<CharacterInstance>().headTarget;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerInteraction>())
        {
            PlayerInteraction.SetFocusObject_Static(null);
            other.GetComponent<NPCHeadLook>().target = null;
            headLook.target = null;
        }
    }
}
