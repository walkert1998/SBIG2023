using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpdateHoveredItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;
    public bool hovering = false;
    InputManager inputManager;

    void Start()
    {
        inputManager = InputManager.Instance;
    }

    // void Update()
    // {
    //     if (hovering)
    //     {
    //         if (inputManager.Hotkey1Pressed())
    //         {
    //             Hotkey.HotkeyItem_static(0, item);
    //         }
    //         else if (inputManager.Hotkey2Pressed())
    //         {
    //             Hotkey.HotkeyItem_static(1, item);
    //         }
    //         else if (inputManager.Hotkey3Pressed())
    //         {
    //             Hotkey.HotkeyItem_static(2, item);
    //         }
    //         else if (inputManager.Hotkey4Pressed())
    //         {
    //             Hotkey.HotkeyItem_static(3, item);
    //         }
    //         //else if (Input.GetKeyDown(KeyCode.Alpha5))
    //         //{
    //         //    Hotkey.HotkeyItem_static(4, item);
    //         //}
    //         //else if (Input.GetKeyDown(KeyCode.Alpha6))
    //         //{
    //         //    Hotkey.HotkeyItem_static(5, item);
    //         //}
    //         //else if (Input.GetKeyDown(KeyCode.Alpha7))
    //         //{
    //         //    Hotkey.HotkeyItem_static(6, item);
    //         //}
    //         //else if (Input.GetKeyDown(KeyCode.Alpha8))
    //         //{
    //         //    Hotkey.HotkeyItem_static(7, item);
    //         //}
    //         //else if (Input.GetKeyDown(KeyCode.Alpha9))
    //         //{
    //         //    Hotkey.HotkeyItem_static(8, item);
    //         //}
    //     }
    // }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("pointer enters");
        //UISFX.PlayHoverSound_Static();
        string itemTooltip = System.String.Empty;
        
        hovering = true;
        Tooltip.DisplayToolTip_Static(item.itemName);
        // else
        // {
        //     Tooltip.DisplayToolTip_Static("<b>" + item.itemName + "</b>\n" + "Type: " + item.itemSlotType.ToString() + "\n" + item.description);
        // }
        transform.Find("ItemHighlight").gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.HideToolTip_Static();
        hovering = false;
        transform.Find("ItemHighlight").gameObject.SetActive(false);
    }
}
