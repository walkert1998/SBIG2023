using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIClickNotifier : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    public UnityEvent onLeft;
    public UnityEvent onRight;
    public UnityEvent onMiddle;
    public UnityEvent onLeftDoubleClick;
    public UnityEvent onLeftHold;

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount == 2)
        {
            onLeftDoubleClick.Invoke();
            // Debug.Log("double click");
        }
        else if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount == 1)
        {
            onLeft.Invoke();
            // Debug.Log("left click");
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            onRight.Invoke();
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            onMiddle.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount < 2)
        {
            onLeftHold.Invoke();
            //Debug.Log("left click");
        }
    }
}
