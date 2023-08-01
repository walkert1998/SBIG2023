using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEnterTrigger : MonoBehaviour
{
    public UnityEvent onEnterEvent;
    public UnityEvent onExitEvent;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerInteraction>())
        {
            onEnterEvent.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerInteraction>())
        {
            onExitEvent.Invoke();
        }
    }
}
