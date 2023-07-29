using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Create Item")]
public class Item : ScriptableObject
{
    [Header("Item Info")]
    public ItemType itemType;
    public string itemName;
    [SerializeField]
    public string itemID;
    public string baseItemID;
    [TextArea]
    public string description;
    [Tooltip("Monetary value of item, used for trading.")]
    public int price;
    [Header("Icons")]
    [JsonIgnore]
    public Sprite horizontalIcon;
    [JsonIgnore]
    public Sprite verticalIcon;
    [Header("Inventory Properties")]
    public IntPair size;
    public bool stackable;
    public List<ItemCombination> itemCombinationsPossible;
    public bool destroyOnUse;
    // public EquippedItemSlotType itemSlotType;
    public float coolDownTime = 1.0f;
    [Header("Item Model")]
    [JsonIgnore]
    public GameObject droppedItem;
    [Header("Sounds")]
    [JsonIgnore]
    public AudioClip onUseSound;

    public virtual void Use() { }

    public bool CanCombineWith(Item item)
    {
        foreach (ItemCombination combo in itemCombinationsPossible)
        {
            if (combo.otherItemRequired == item)
            {
                return true;
            }
        }
        return false;
    }

    [ContextMenu("Generate ID")]
    public void GenerateID()
    {
        itemID = Guid.NewGuid().ToString();
        if (!name.Contains("Clone"))
            baseItemID = itemID;
    }
}
public enum ItemType
{
    RangedWeapon,
    MeleeWeapon,
    ThrowingWeapon,
    WeaponModification,
    SpellCastDevice,
    Consumable,
    Clothing,
    Currency,
    Key,
    Note,
    Misc,
    Ammo
}
