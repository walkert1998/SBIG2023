using Newtonsoft.Json;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
//using UnityStandardAssets.Characters.FirstPerson;

public class InventoryManager : MonoBehaviour, ISaveable
{
    private static InventoryManager instance;
    public bool notePadOpen;
    public static bool persistInventory = true;
    [SerializeField]
    private InventoryView playerInventory;
    public Inventory inventory;
    public ThirdPersonController playerController;
    public GameObject movingObject;
    // public PlayerControlScheme controls;
    InputAction toggleInventory;
    InputAction toggleNotePad;
    InputAction toggleSuspects;
    public Item testItem;
    public PlayerInput playerInput;

    public static int movingItem = 0;

    [System.Serializable]
    private struct SaveData
    {
        public bool inventoryOpen;
        public Inventory inventory;
    }


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        //inventoryPanel.SetActive(false);
        //playerInventory.inventory.firstTimePickup = true;
        //inventoryOpen = false;
        if (!persistInventory)
        {
            playerInventory.ClearInventory();
        }
    }

    void Start()
    {
        playerInventory.ClearInventory();
    }

    private void OnEnable()
    {
        toggleInventory = playerInput.actions.FindAction("Inventory");
        toggleInventory.performed += ToggleInventory;
        toggleInventory.Enable();
        toggleSuspects = playerInput.actions.FindAction("Suspects");
        toggleSuspects.performed += ToggleSuspects;
        toggleSuspects.Enable();
        toggleNotePad = playerInput.actions.FindAction("NotePad");
        toggleNotePad.performed += ToggleNotePad;
        toggleNotePad.Enable();
    }
    private void OnDisable()
    {
        toggleInventory.Disable();
        toggleSuspects.Disable();
        toggleNotePad.Disable();
    }

    private void ToggleInventory(InputAction.CallbackContext obj)
    {
        if (playerController != null)
        {
            if (notePadOpen)
            {
                HideInventory();
            }
            else if (playerController.CanMove)
            {
                // playerInventory.inventory.AddItem(testItem, 1);
                ShowInventory();
            }
        }
    }

    public static void ToggleInventory_Static()
    {
        if (instance.notePadOpen)
        {
            instance.HideInventory();
        }
        else
        {
            // playerInventory.inventory.AddItem(testItem, 1);
            instance.ShowInventory();
        }
    }

    private void ToggleSuspects(InputAction.CallbackContext context)
    {
        if (notePadOpen)
        {
            HideSuspects();
        }
        else if (playerController.CanMove)
        {
            ShowSuspects();
        }
    }

    private void ToggleNotePad(InputAction.CallbackContext context)
    {
        if (notePadOpen)
        {
            HideNotePad();
        }
        else if (playerController.CanMove)
        {
            ShowNotePad();
        }
    }

    private void ShowInventory()
    {
        //DynamicCursor.ChangeCursor_Static(CursorType.None);
        // playerInventoryPanel.SetActive(true);
        CharacterPanel.ShowInventoryTab();
        playerController.LockMovement(false);
        playerInventory.OpenView();
        notePadOpen = true;
        // playerController.SetLookLock(false);
        //playerController.SetMoveLock(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // PlayerInteraction.LockInteraction();
        // TimeControl.ChangeTimeScale(0.25f);
        //phone.GetComponent<WeaponSway>().enabled = false;
        //Tooltip.HideToolTip_Static();
        //firstPersonController.m_CanMove = false;
        //firstPersonController.m_CanLook = false;
        //firstPersonController.GetMouseLook().SetCursorLock(false);
    }

    private void HideInventory()
    {
        //DynamicCursor.ChangeCursor_Static(CursorType.Target);
        CharacterPanel.HideCharacterPanel();
        // playerInventoryPanel.SetActive(false);
        playerInventory.CloseView();
        notePadOpen = false;
        // playerController.SetLookLock(true);
        //playerController.SetMoveLock(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // PlayerInteraction.UnlockInteraction();
        // TimeControl.RevertTimeScale();
        //firstPersonController.GetMouseLook().SetCursorLock(true);
        Tooltip.HideToolTip_Static();
        //firstPersonController.m_CanMove = true;
        //firstPersonController.m_CanLook = true;
        //phone.GetComponent<WeaponSway>().enabled = true;
        playerController.LockMovement(true);
    }

    private void ShowNotePad()
    {
        CharacterPanel.ShowLastTab();
        playerController.LockMovement(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // playerInventory.OpenView();
        notePadOpen = true;
    }

    private void HideNotePad()
    {
        CharacterPanel.HideCharacterPanel();
        notePadOpen = false;
        Tooltip.HideToolTip_Static();
        playerController.LockMovement(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ShowSuspects()
    {
        CharacterPanel.ShowSuspectsTab();
        playerController.LockMovement(false);
        playerInventory.CloseView();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // playerInventory.OpenView();
        notePadOpen = true;
    }

    private void HideSuspects()
    {
        CharacterPanel.HideCharacterPanel();
        notePadOpen = false;
        Tooltip.HideToolTip_Static();
        playerController.LockMovement(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private bool IsInventoryOpen()
    {
        return notePadOpen;
    }

    private InventoryView GetInventoryView()
    {
        return playerInventory;
    }

    public static void ShowInventory_Static()
    {
        instance.ShowInventory();
    }

    public static void HideInventory_Static()
    {
        instance.HideInventory();
    }

    public static void HideNotePad_Static()
    {
        instance.HideNotePad();
    }

    public static bool IsInventoryOpen_Static()
    {
        return instance.IsInventoryOpen();
    }

    public static bool IsNotePadOpen_Static()
    {
        return instance.IsInventoryOpen();
    }

    public static Inventory GetPlayerInventory_Static()
    {
        return instance.playerInventory.inventory;
    }

    public static InventoryView GetInventoryView_Static()
    {
        return instance.GetInventoryView();
    }

    public object SaveState()
    {
        return new SaveData
        {
            inventoryOpen = notePadOpen,
            inventory = playerInventory.inventory
        };
    }

    public void LoadState(object saveState)
    {
        string json = JsonConvert.SerializeObject(saveState);
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json, new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.Auto
            //ReferenceResolverProvider = () => new GenericResolver<Item>(p => p.itemName)
        });
        //Debug.Log(json);
        playerInventory.ClearInventory();
        playerInventory.inventory.PopulateInventory(saveData.inventory);
        foreach (StoredItem storedItem in playerInventory.inventory.items)
        {
            // Debug.Log("ScriptableObjects/Items/" + storedItem.item.itemType.ToString() + "/" + storedItem.itemName);
            Item baseItem = Resources.Load<Item>("ScriptableObject/Items/" + storedItem.item.itemType.ToString() + "/" + storedItem.item.name);
            storedItem.baseItemID = baseItem.baseItemID;
            // storedItem.item.itemID = baseItem.itemID;
            if (baseItem.droppedItem != null)
            {
                storedItem.item.droppedItem = baseItem.droppedItem;
            }
            if (baseItem.horizontalIcon != null)
            {
                storedItem.item.horizontalIcon = baseItem.horizontalIcon;
            }
            //storedItem.item.verticalIcon = baseItem.verticalIcon;
            if (storedItem.item.itemType == ItemType.RangedWeapon)
            {
                RangedWeapon wep = storedItem.item as RangedWeapon;
                RangedWeapon baseWep = Resources.Load<RangedWeapon>("ScriptableObject/Items/" + storedItem.item.itemType.ToString() + "/" + storedItem.item.name);
                wep.gunshotSounds = baseWep.gunshotSounds;
                wep.noAmmoSound = baseWep.noAmmoSound;
                wep.reloadSound = baseWep.reloadSound;
                wep.silencedSound = baseWep.silencedSound;
                wep.unSilencedSound = baseWep.unSilencedSound;
                wep.weaponDrawSound = baseWep.weaponDrawSound;
                wep.droppedItem = baseWep.droppedItem;
                wep.viewModel = baseWep.viewModel;
                wep.ammoTypes = baseWep.ammoTypes;
                if (baseWep.availableModifications.Count > 0)
                {
                    wep.availableModifications = baseWep.availableModifications;
                }
                foreach (WeaponModification installed in wep.installedModifications)
                {
                    WeaponModification baseMod = Resources.Load<WeaponModification>("ScriptableObjects/Items/WeaponModification/" + installed.name);
                    // Debug.Log(installed.name);
                    installed.itemType = ItemType.WeaponModification;
                    // if (baseMod.horizontalIcon != null)
                    // {
                    //     installed.horizontalIcon = baseMod.horizontalIcon;
                    // }
                    // if (baseMod.verticalIcon != null)
                    // {
                    //     installed.verticalIcon = baseMod.verticalIcon;
                    // }
                    // installed.installedIconHorizontal = baseMod.installedIconHorizontal;
                    // installed.installedIconVertical = baseMod.installedIconVertical;
                }
                //playerInventory.weaponManager.
            }
            else if (storedItem.item.itemType == ItemType.WeaponModification)
            {
                WeaponModification weaponModification = storedItem.item as WeaponModification;
                WeaponModification baseMod = Resources.Load<WeaponModification>("ScriptableObjects/Items/" + storedItem.item.itemType.ToString() + "/" + storedItem.item.name);
                weaponModification.installedIconHorizontal = baseMod.installedIconHorizontal;
                weaponModification.installedIconVertical = baseMod.installedIconVertical;
            }
            else if (storedItem.item.itemType == ItemType.Ammo)
            {
                Ammo ammo = storedItem.item as Ammo;
                Ammo baseAmmo = Resources.Load<Ammo>("ScriptableObjects/Items/" + storedItem.item.itemType.ToString() + "/" + storedItem.item.name);
                if (baseAmmo.projectile != null)
                {
                    ammo.projectile = baseAmmo.projectile;
                }
                if (baseAmmo.impactEffect != null)
                {
                    ammo.impactEffect = baseAmmo.impactEffect;
                }
            }
        }
        playerInventory.Notify();
        notePadOpen = saveData.inventoryOpen;
        if (notePadOpen)
        {
            ShowInventory();
        }
    }
}
