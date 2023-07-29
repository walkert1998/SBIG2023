using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color baseColor;
    public Color moveAllowedColour = Color.green;
    public Color moveDisallowedColour = Color.red;
    public Color combineColour = Color.cyan;
    public bool slotEligible;
    public bool canCombine;
    public bool movingItems;
    public IntPair slotPosition;
    public IntPair size;
    public bool hovered = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("pointer enters");
        if (movingItems)
        {
            SelectedSlot.SetSelectedSlot_Static(this);
            Highlight();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SelectedSlot.ClearSelectedSlot_Static();
        //DynamicCursor.ChangeCursor_Static(CursorType.None);
        UnHighlight();
    }

    public void Highlight()
    {
        if (slotEligible)
        {
            //DynamicCursor.ChangeCursor_Static(CursorType.None);
            GetComponent<Image>().color = moveAllowedColour;
        }

        else if (canCombine)
        {
            //DynamicCursor.ChangeCursor_Static(CursorType.Combine);
            //DynamicCursor.HideCursor_Static();
            GetComponent<Image>().color = combineColour;
        }

        else
        {
            //DynamicCursor.ChangeCursor_Static(CursorType.None);
            GetComponent<Image>().color = moveDisallowedColour;
        }
    }

    public void UnHighlight()
    {
        GetComponent<Image>().color = baseColor;
    }
}
