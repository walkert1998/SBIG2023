using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    bool focused { get; set; }
    bool holdToInteract { get; set; }
    void StartInteraction();
    void EndInteraction();
    void Examine();
    void Focus();
    void UnFocus();
}
