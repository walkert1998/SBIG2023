using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class StoredItem
{
    [SerializeReference]
    public Item item;
    public string baseItemID;
    public string itemName;
    public int quantity = 1;
    public IntPair position;
    public IntPair size;
    [JsonIgnore]
    public Sprite horizontalIcon;
    [JsonIgnore]
    public Sprite verticalIcon;
    public ItemOrientation orientation;

    public StoredItem(Item Item, IntPair Position, ItemOrientation Orientation = ItemOrientation.Horizontal, int Quantity = 1)
    {
        itemName = Item.itemName;
        item = Item;
        baseItemID = item.itemID;
        position = Position;
        orientation = Orientation;
        size = item.size;
        quantity = Quantity;
    }
}

[System.Serializable]
public enum ItemOrientation
{
    Horizontal,
    Vertical
}
