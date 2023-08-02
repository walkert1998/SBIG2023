using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public float distance;
    public TMP_Text prompt;
    //NoteReader noteReader;
    //private WeaponManager weaponManager;
    public LayerMask interactionLayer;
    //public GameMenu menu;
    public static bool interactionBlocked = false;
    public bool interactionBlocked_nonStatic = false;
    RaycastHit hit;
    static PlayerInteraction instance;
    //public NPCDialogue playerDialogue;
    IInteractable focusedObject;
    public Camera playerCamera;
    // public PlayerControls playerInput;
    InputAction interact;
    public PlayerInput inputManager;
    bool overUI = false;
    public bool interacting = false;

    // Use this for initialization
    void Awake()
    {
        instance = this;
        // playerInput = new PlayerControls();
        //noteReader = GetComponent<NoteReader>();
        //weaponManager = GetComponent<WeaponManager>();
        UnlockInteraction();
        //hit = new RaycastHit();
    }

    private void OnEnable()
    {
        interact = inputManager.actions.FindAction("Interact");
        interact.performed += Interact;
        // interact.started += OnInteractionStart;
        // interact.canceled += OnInteractionEnd;
        interact.Enable();
    }

    private void OnDisable()
    {
        interact.Disable();
    }

    //Update is called once per frame
    // void Update()
    // {
    //     //Debug.DrawRay(playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
    //     if (!interactionBlocked)
    //     {
    //         if (Physics.Raycast(playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, distance, interactionLayer) && !overUI)
    //         {
    //             IInteractable newObject = hit.collider.GetComponent<IInteractable>();
    //             if (focusedObject == null)
    //             {
    //                 focusedObject = newObject;
    //             }
    //             else if (focusedObject != newObject)
    //             {
    //                 InteractionProgressBar.HideInteractionProgress_static();
    //                 focusedObject.UnFocus();
    //                 focusedObject = newObject;
    //             }
    //             if (focusedObject != null)
    //             {
    //                 if (!focusedObject.focused)
    //                 {
    //                     focusedObject.Focus();
    //                 }
    //             }
    //         }
    //         else if (focusedObject != null)
    //         {
    //             InteractionProgressBar.HideInteractionProgress_static();
    //             focusedObject.UnFocus();
    //             focusedObject = null;
    //         }
    //     }
    // }

    public static void SetFocusObject_Static(IInteractable interactable)
    {
        instance.SetFocusObject(interactable);
    }

    public void SetFocusObject(IInteractable newObject)
    {
        if (focusedObject == null)
        {
            focusedObject = newObject;
        }
        else if (focusedObject != newObject)
        {
            focusedObject.UnFocus();
            focusedObject = newObject;
        }
        if (focusedObject != null)
        {
            if (!focusedObject.focused)
            {
                focusedObject.Focus();
            }
        }
        if (newObject == null)
        {
            GetComponent<NPCHeadLook>().target = null;
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (!interacting)
        {
            OnInteractionStart(context);
        }
        else
        {
            OnInteractionEnd(context);
        }
    }

    public void OnInteractionStart(InputAction.CallbackContext context)
    {
        Debug.Log("Clicked");
        if (focusedObject != null)
        {
            interacting = true;
            focusedObject.StartInteraction();
            Tooltip.HideToolTip_Static();
        }
    }

    public void OnInteractionEnd(InputAction.CallbackContext context)
    {
        if (focusedObject != null)
        {
            interacting = false;
            focusedObject.EndInteraction();
            // InteractionProgressBar.HideInteractionProgress_static();
            // focusedObject = null;
        }
    }

    public static void LockInteraction()
    {
        interactionBlocked = true;
        // InteractionProgressBar.HideInteractionProgress_static();
        if (instance.focusedObject != null)
        {
            instance.focusedObject.UnFocus();
            instance.focusedObject = null;
        }
        //Debug.Log("Locking " + interactionBlocked);
    }

    public static void UnlockInteraction()
    {
        interactionBlocked = false;
        Debug.Log("Unlocking interaction");
    }

    public static void SetPrompt(string interactText)
    {
        instance.prompt.gameObject.SetActive(true);
        instance.prompt.text = interactText;
    }
}
