using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Inventory/Create Inventory")]
[System.Serializable]
public class Inventory : ObservableSO
{
    [SerializeReference]
    public List<StoredItem> items;
    public IntPair size;
    //public GameObject assignedCharacter;

    [System.Serializable]
    private struct SaveData
    {
        public List<StoredItem> items;
        public IntPair size;
    }

    public void AddItem(StoredItem item)
    {
        item.itemName = item.item.name;
        items.Add(item);
        Notify();
    }

    public void RemoveItem(StoredItem item, bool drop=false)
    {
        items.Remove(item);
        if (item.item.droppedItem != null && drop)
        {
            Vector2 dropPoint = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(dropPoint);
            RaycastHit hit;
            bool hitSomething = Physics.Raycast(ray, out hit, 4.0f);
            Vector3 dropPos;
            if (hitSomething)
            {
                dropPos = Camera.main.ScreenToWorldPoint(new Vector3(dropPoint.x, dropPoint.y, hit.distance));
            }
            else
            {
                dropPos = Camera.main.ScreenToWorldPoint(new Vector3(dropPoint.x, dropPoint.y, 3.0f));
            }
            dropPos.y = item.item.droppedItem.transform.position.y;
            GameObject newDrop = Instantiate(item.item.droppedItem, dropPos, Quaternion.identity);
            newDrop.GetComponent<PickUp>().quantity = item.quantity;
        }
        Notify();
    }

    public void RemoveItem(string itemID, bool drop = false)
    {
        foreach (StoredItem item in items)
        {
            if (item.item.itemID == itemID)
            {
                //Debug.Log("Item: " + item.item);
                items.Remove(item);
                if (item.item.droppedItem != null && drop)
                {
                    // GameObject newDrop = Instantiate(item.item.droppedItem, InventoryManager.GetInventoryView_Static().itemDropLocation.position, Quaternion.identity);
                    // if (item.item.itemType == ItemType.RangedWeapon)
                    // {
                    //     newDrop.GetComponent<WeaponDropModel>().DisplayModels((RangedWeapon)item.item);
                    // }
                    // newDrop.GetComponent<PickUp>().item = item.item;
                    // newDrop.GetComponent<PickUp>().quantity = item.quantity;
                    // PickUpItemsManager.AddPickUpToList(newDrop.GetComponent<PickUp>());
                }
                break;
            }
        }
        Notify();
    }

    public void AddItemQuantity(Item item, int quantity)
    {
        StoredItem itemChanged = items.Find(StoredItem => StoredItem.item == item);
        itemChanged.quantity += quantity;
        Notify();
    }

    public void RemoveItemQuantity(Item item, int quantity, bool drop=false)
    {
        StoredItem itemChanged = items.Find(StoredItem => StoredItem.item.baseItemID == item.baseItemID);
        if (itemChanged != null)
        {
            //Debug.Log("Removing " + quantity + " " + item.itemName);
            if (quantity >= itemChanged.quantity)
                items.Remove(itemChanged);
            else
                itemChanged.quantity -= quantity;

            //if (item.droppedItem != null && drop)
            //{
            //    GameObject newDrop = Instantiate(item.droppedItem, InventoryManager.GetInventoryView_Static().itemDropLocation.position, Quaternion.identity);
            //    newDrop.GetComponent<PickUp>().quantity = quantity;
            //}
            Notify();
        }
    }

    public void RemoveItemQuantity(string itemID, int quantity, bool drop = false)
    {
        StoredItem itemChanged = items.Find(StoredItem => StoredItem.item.itemID == itemID);
        if (itemChanged != null)
        {
            // Debug.Log("Removing " + quantity + " " + itemChanged.itemName + "'s");
            if (quantity >= itemChanged.quantity)
                items.Remove(itemChanged);
            else
                itemChanged.quantity -= quantity;

            //if (item.droppedItem != null && drop)
            //{
            //    GameObject newDrop = Instantiate(item.droppedItem, InventoryManager.GetInventoryView_Static().itemDropLocation.position, Quaternion.identity);
            //    newDrop.GetComponent<PickUp>().quantity = quantity;
            //}
            Notify();
        }
    }

    public bool MoveItem(StoredItem toMove, IntPair newPos)
    {
        if (IsPositionValid(toMove, newPos.x, newPos.y, toMove) ||
            CombinationValid(toMove.item, newPos.x, newPos.y, toMove))
        {
            toMove.position = newPos;
            Notify();
            return true;
        }
        else
        {
            return false;
        }
    }

    private int FreeSlotsCount()
    {
        int occupied = 0;

        foreach (StoredItem item in items)
        {
            occupied += item.size.x * item.size.y;
        }

        return size.x * size.y - occupied;
    }

    private Item IsColliding (IntPair itemSize, int row, int col, StoredItem ignoreWith = null)
    {
        foreach (StoredItem item in items)
        {
            if (
                ABBintersectsABB(
                    item.position.y, item.position.x, item.size.y, item.size.x,
                    col, row, itemSize.y, itemSize.x
                )
                &&
                item != ignoreWith
            )
            {
                return item.item;
            }
        }
        return null;
    }

    private bool ABBintersectsABB(int aX, int aY, float aWidth, float aHeight, int bX, int bY, float bWidth, float bHeight)
    {
        return (aX < bX + bWidth &&
                aX + aWidth > bX &&
                aY < bY + bHeight &&
                aHeight + aY > bY);
    }

    public bool IsPositionValid (StoredItem item, int row, int col, StoredItem ignoreWith = null)
    {
        return InBounds(item.size, row, col) && IsColliding(item.size, row, col, ignoreWith) == null;
    }

    public bool CombinationValid(Item item, int row, int col, StoredItem ignoreWith = null)
    {
        return InBounds(item.size, row, col) && item.CanCombineWith(IsColliding(item.size, row, col, ignoreWith));
    }

    private IntPair FindValidPosition (StoredItem item)
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (IsPositionValid(item, i, j))
                {
                    return new IntPair(i, j);
                }
            }
        }

        return null;
    }

    public bool AddItem(Item item, int itemQuantity=1)
    {
        Debug.Log("Adding " + item.itemName);
        Item newItem = ScriptableObject.Instantiate(item);
        if (newItem.itemID == newItem.baseItemID)
        {
            newItem.GenerateID();
        }
        newItem.name = newItem.name.Replace("(Clone)",System.String.Empty).Trim();
        int totalSize = item.size.x * item.size.y;
        //if (firstTimePickup)
        //{
        //    //HelpText._DisplayHelpText("[TAB] Items screen\n[1] Equip Revolver", KeyCode.None, null, 4.0f);
        //    EventLog.AddEventText("Weapon Wheel [Tab]\nEquip Weapon [1]-[5]\nSwitch to Last Weapon [Q]");
        //    firstTimePickup = false;
        //}

        if (FreeSlotsCount() >= totalSize)
        {
            StoredItem updatedItem = items.Find(foundItem => foundItem.item.itemName == newItem.itemName);
            if (newItem.stackable && updatedItem != null && items.Contains(updatedItem))
            {
                Debug.Log("Adding " + itemQuantity + " item: " + newItem.itemName);
                // EventLog.AddEventText("Picked Up " + itemQuantity + " " + newItem.itemName);
                updatedItem.quantity += itemQuantity;
                Notify();
                return true;
            }
            else
            {
                StoredItem itemToAdd = new StoredItem(newItem, new IntPair(0,0), Quantity: itemQuantity);
                IntPair position = FindValidPosition(itemToAdd);
                if (position != null)
                {
                    itemToAdd.position = position;
                    if (newItem.stackable)
                    {
                        // EventLog.AddEventText("Picked Up " + itemQuantity + " " + newItem.itemName);
                    }
                    else
                    {
                        // EventLog.AddEventText("Picked Up " + newItem.itemName);
                    }
                    Debug.Log("Adding " + itemQuantity + " item: " + newItem.itemName);
                    items.Add(itemToAdd);
                    Notify();
                    return true;
                }
                else
                {
                    Debug.Log("Move items to make space");
                    return false;
                }
            }
        }
        else
        {
            Debug.Log("Inventory full");
            return false;
        }
    }

    public StoredItem FindItem(Item itemToReturn)
    {
        StoredItem item = items.Find(StoredItem => StoredItem.item.baseItemID == itemToReturn.baseItemID);
        if (item != null)
        {
            return item;
        }
        return null;
    }

    public StoredItem FindExactItem(Item itemToReturn)
    {
        StoredItem item = items.Find(StoredItem => StoredItem.item.itemID == itemToReturn.itemID);
        if (item != null)
        {
            return item;
        }
        return null;
    }

    public StoredItem FindExactItemByID(string itemIDToReturn)
    {
        StoredItem item = items.Find(StoredItem => StoredItem.item.itemID == itemIDToReturn);
        if (item != null)
        {
            return item;
        }
        return null;
    }

    public bool InBounds (IntPair itemSize, int row, int col)
    {
        return row >= 0 && row < size.x &&
               row + itemSize.x <= size.x &&
               col >= 0 && col < size.y &&
               col + itemSize.y <= size.y;
    }

    public void SetSize(int rows, int columns)
    {
        size = new IntPair(rows, columns);
        Notify();
    }

    public void PopulateInventory(Inventory savedInventory)
    {
        items = savedInventory.items;
    }

    public void RemoveItem_EasyAcces(Item itemToRemove)
    {
        StoredItem itemFound = FindItem(itemToRemove);
        Debug.Log(itemFound.itemName);
        RemoveItem(itemFound);
    }
}
