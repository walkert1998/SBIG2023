using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using StarterAssets;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public static CameraSwitcher instance;
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera primaryCamera;
    public ThirdPersonController playerController;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }
        instance = this;
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }

    public static void SwitchCameraTo(CinemachineVirtualCamera newCamera)
    {
        if (newCamera != instance.playerCamera)
        {
            instance.playerController.LockMovement(false);
        }
        instance.primaryCamera.gameObject.SetActive(false);
        instance.primaryCamera = newCamera;
        instance.primaryCamera.gameObject.SetActive(true);
    }

    public static void SwitchToPlayerCamera()
    {
        SwitchCameraTo(instance.playerCamera);
        instance.playerController.LockMovement(true);
    }
}
