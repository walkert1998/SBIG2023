using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CharacterInstance : MonoBehaviour, IInteractable
{
    public Character characterTemplate;
    public NPCConversation activeConversation;
    public Faction faction;
    public AudioSource source;
    public Animator animator;
    public CinemachineVirtualCamera vCam;
    public Transform headTarget;
    public NPCHeadLook headLook;
    public Color subtitlecolour;
    public NPCColliders colliders;
    public GameObject weaponModel;
    public AudioClip weaponSound;
    public int health = 1;

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
            other.GetComponent<NPCHeadLook>().SetTarget(headTarget);
            if (headLook != null)
            {
                headLook.SetTarget(other.GetComponent<CharacterInstance>().headTarget);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerInteraction>())
        {
            PlayerInteraction.SetFocusObject_Static(null);
            other.GetComponent<NPCHeadLook>().SetTarget(null);
            if (headLook != null)
            {
                headLook.SetTarget(null);
            }
        }
    }

    public void KillNPC()
    {
        if (activeConversation.characterName == "Marvin Green" || activeConversation.characterName == "Sir Red Herring" || activeConversation.characterName == "Timmy Cadaver" || activeConversation.characterName == "Violet Cadaver")
        {
            InvestigationManager.KillSuspect();
        }
        health = 0;
        if (animator != null)
        {
            animator.enabled = false;
        }
        colliders?.EnableRagdoll();
        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().enabled = false;
        }
        activeConversation.enabled = false;
        if (headLook != null)
        {
            headLook.enabled = false;
        }
        // DialogueScreen.instance.characters.Remove(this);
        // DialogueScreen.instance.characterAudioSources.Remove(source);
        Debug.Log("NPC killed!");
    }

    public void DrawWeapon()
    {
        weaponModel?.SetActive(true);
    }

    public void HolsterWeapon()
    {
        weaponModel?.SetActive(false);
    }

    public void UseWeapon()
    {
        if (weaponModel != null)
        {
            if (weaponSound != null && weaponModel.GetComponent<AudioSource>())
            {
                weaponModel.GetComponent<AudioSource>().PlayOneShot(weaponSound);
                Debug.Log("Played shot sound");
            }
        }
    }
}
