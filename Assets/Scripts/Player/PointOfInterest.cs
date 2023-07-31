using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PointOfInterest : MonoBehaviour, IInteractable
{
    public bool focused { get; set; }
    public bool holdToInteract { get; set; }

    public CinemachineVirtualCamera vCam;
    public GameObject promptIndicator;
    [TextArea]
    public string description;

    public void EndInteraction()
    {
        CameraSwitcher.SwitchToPlayerCamera();
        promptIndicator.SetActive(true);
        PlayerInteraction.SetPrompt(System.String.Empty);
    }

    public void Examine()
    {
    }

    public void Focus()
    {
        promptIndicator.SetActive(true);
    }

    public void StartInteraction()
    {
        CameraSwitcher.SwitchCameraTo(vCam);
        promptIndicator.SetActive(false);
        PlayerInteraction.SetPrompt(description);
    }

    public void UnFocus()
    {
        promptIndicator.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerInteraction>())
        {
            PlayerInteraction.SetFocusObject_Static(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerInteraction>())
        {
            PlayerInteraction.SetFocusObject_Static(null);
        }
    }

    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    // void Update()
    // {
        
    // }
}
