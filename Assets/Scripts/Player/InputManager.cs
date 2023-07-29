using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;

    public static InputManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public PlayerInput playerInput;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        // playerInput = GetComponent<PlayerControls>();
    }

    public InputActionAsset GetControlsList()
    {
        return playerInput.actions;
    }

    //private void OnEnable()
    //{
    //    playerInput.Enable();
    //}

    //private void OnDisable()
    //{
    //    playerControls.Disable();
    //}

    public Vector2 GetPlayerMovement()
    {
        return playerInput.actions["Move"].ReadValue<Vector2>();
    }

    public Vector2 GetLookDelta()
    {
        return playerInput.actions["Look"].ReadValue<Vector2>();
    }

    public bool JumpedThisFrame()
    {
        return playerInput.actions["Jump"].triggered;
    }

    public bool DashedThisFrame()
    {
        return playerInput.actions["Dash"].triggered;
    }

    public bool GroundSlammedThisFrame()
    {
        return playerInput.actions["Slam"].triggered;
    }

    public bool AttackPressed()
    {
        return playerInput.actions["Attack"].triggered;
    }

    public bool AttackHeld()
    {
        return playerInput.actions["Attack"].activeControl != null;
    }

    public bool AttackReleased()
    {
        return playerInput.actions["Attack"].WasReleasedThisFrame();
    }

    public bool AltFirePressed()
    {
        return playerInput.actions["Alt Fire"].triggered;
    }

    public bool AltFireHeld()
    {
        return playerInput.actions["Alt Fire"].activeControl != null;
    }

    public bool AltFireReleased()
    {
        return playerInput.actions["Alt Fire"].WasReleasedThisFrame();
    }

    public bool ReloadPressed()
    {
        return playerInput.actions["Reload"].triggered;
    }

    public bool ChangeAmmoPressed()
    {
        return playerInput.actions["ChangeAmmo"].triggered;
    }

    public bool AbilityKeyPressed()
    {
        return playerInput.actions["ActivateAbility"].triggered;
    }

    //public bool GrappleKeyPressed()
    //{
    //    return playerInput.actions["Grapple"].triggered;
    //}

    public bool Hotkey1Pressed()
    {
        return playerInput.actions["Weapon 1"].triggered;
    }

    public bool Hotkey2Pressed()
    {
        return playerInput.actions["Weapon 2"].triggered;
    }

    public bool Hotkey3Pressed()
    {
        return playerInput.actions["Weapon 3"].triggered;
    }

    public bool Hotkey4Pressed()
    {
        return playerInput.actions["Weapon 4"].triggered;
    }

    public bool Hotkey5Pressed()
    {
        return playerInput.actions["Weapon 5"].triggered;
    }

    public bool Hotkey6Pressed()
    {
        return playerInput.actions["Weapon 6"].triggered;
    }

    public bool InteractPressed()
    {
        return playerInput.actions["Interact"].triggered;
    }

    public bool InteractHeld()
    {
        return playerInput.actions["Interact"].activeControl != null;
    }

    public bool InteractReleased()
    {
        return playerInput.actions["Interact"].WasReleasedThisFrame();
    }

    public bool LastWeaponPressed()
    {
        return playerInput.actions["Last Weapon"].triggered;
    }

    public bool ScrolledUp()
    {
        return playerInput.actions["Switch Weapon"].ReadValue<float>() > 0;
    }

    public bool ScrolledDown()
    {
        return playerInput.actions["Switch Weapon"].ReadValue<float>() < 0;
    }

    public bool QuicksavePressed()
    {
        return playerInput.actions["Quicksave"].triggered;
    }

    public bool QuickloadPressed()
    {
        return playerInput.actions["Quickload"].triggered;
    }
}
