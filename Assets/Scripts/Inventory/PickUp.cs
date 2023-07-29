using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PickUp : MonoBehaviour, IInteractable
{
    public string pickup_name;
    public bool picked_up = false;
    public Item item;
    private GameObject world_item;
    public int quantity;
    Inventory playerInventory;
    public AudioClip pickUpSound;
    public UnityEvent onPickup;
    bool added;

    public bool focused { get; set; }
    public bool holdToInteract { get; set; }

    // Use this for initialization
    void Start ()
    {
        world_item = gameObject;
        playerInventory = Resources.Load<ScriptableObject>("ScriptableObjects/Inventory/PlayerInventory") as Inventory;
        PickUpItemsManager.AddPickUpToList(this);
    }

    [ContextMenu("PickUp")]
    public void Pickup()
    {
        if (playerInventory != null)
        {
            //if (pickup_name == "Backpack")
            //{
            //    playerInventory.SetSize(playerInventory.size.x + 4, playerInventory.size.y);
            //    added = true;
            //    picked_up = true;
            //}
            //else
            //{
                added = playerInventory.AddItem(item, quantity);
                onPickup.Invoke();
                picked_up = true;
            //}
            if (added)
            {
                world_item.SetActive(false);
            }
        }
        else
            Debug.Log("Inventory is null");
    }

    public void StartInteraction()
    {
        Pickup();
    }

    public void EndInteraction()
    {
        focused = false;
    }

    public void Examine()
    {
        throw new System.NotImplementedException();
    }

    public void Focus()
    {
        if (quantity > 1)
        {
            // Tooltip.DisplayToolTip_Static(pickup_name + " (x" + quantity + ")");
        }
        else
        {
            // Tooltip.DisplayToolTip_Static(pickup_name);
        }
        // Debug.Log(pickup_name);
        focused = true;
    }

    public void UnFocus()
    {
        //PlayerInteraction.SetPrompt("");
        // Tooltip.HideToolTip_Static();
        focused = false;
    }

    //private void OnMouseEnter()
    //{
    //    if (Vector3.Distance(transform.position, player.transform.position) < 3)
    //    {
    //        DynamicCursor.ChangeCursor_Static(CursorType.Pickup);
    //        PlayerInteraction.SetPrompt("Pick Up " + item.itemName);
    //    }
    //}

    //private void OnMouseExit()
    //{
    //    DynamicCursor.ChangeCursor_Static(CursorType.Target);
    //    PlayerInteraction.SetPrompt("");
    //}
}
