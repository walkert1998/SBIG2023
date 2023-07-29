using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string text;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.DisplayToolTip_Static(text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.HideToolTip_Static();
    }
}
