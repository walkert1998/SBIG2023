using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryView : MonoSOObserver
{
    public RectTransform itemsGrid;
    public Inventory inventory;
    public GameObject itemPrefab;
    public GameObject gridSlotPrefab;
    public Transform itemDropLocation;
    public bool expandWithSlots = false;
    public Image itemDisplayIcon;
    public TMP_Text itemDescription;
    public Item healthItem;
    public TMP_Text healthItemText;

    [SerializeField]
    private bool combining = false;

    // public WeaponManager weaponManager;
    // public Player player;

    private FloatPair unitSlot;

    GridHighlight[,] gridSlots;
    List<GridHighlight> allSlots;
    List<GridHighlight> highlightedSlots;
    public AudioSource source;
    public AudioClip removeSound;
    public AudioClip equipSound;
    public AudioClip invalidMoveSound;
    public AudioClip validMoveSound;
    public AudioClip beginMoveSound;

    [SerializeField]
    public GameObject movingObject;
    private StoredItem movingItem;
    private ItemOrientation startingOrientation;

    [System.Serializable]
    private struct SaveData
    {
        public List<StoredItem> items;
        public Inventory inventory;
    }

    //private void Start()
    //{
    //    healthItemText.text = "x0";
    //    Debug.Log(healthItemText.text);
    //}

    private void CalcSlotDimensions()
    {
        float gridWidth = itemsGrid.rect.width;
        float gridHeight = itemsGrid.rect.height;

        unitSlot = new FloatPair(gridHeight / inventory.size.x, gridWidth / inventory.size.y);
    }

    private FloatPair GetSlotPosition (int row, int col)
    {
        return new FloatPair(row * -unitSlot.x, col * unitSlot.y);
    }

    private void PositionInGrid (GameObject obj, IntPair position, IntPair size)
    {
        RectTransform trans = obj.transform as RectTransform;
        FloatPair gridPostion = GetSlotPosition(position.x, position.y);
        trans.sizeDelta = new Vector2(unitSlot.y * size.y, unitSlot.x * size.x);
        trans.localPosition = new Vector3(gridPostion.y, gridPostion.x, 0.0f);
    }

    private void DrawGrid()
    {
        GameObject gridCell;
        gridSlots = new GridHighlight[inventory.size.x, inventory.size.y];
        allSlots = new List<GridHighlight>();
        IntPair unitPair = new IntPair(1, 1);
        for (int i = 0; i < inventory.size.x; i++)
        {
            for (int j = 0; j < inventory.size.y; j++)
            {
                gridCell = Instantiate(gridSlotPrefab, itemsGrid);
                gridCell.GetComponent<GridHighlight>().baseColor = gridCell.GetComponent<Image>().color;
                gridCell.GetComponent<GridHighlight>().slotPosition = new IntPair(i, j);
                gridSlots[i, j] = gridCell.GetComponent<GridHighlight>();
                allSlots.Add(gridCell.GetComponent<GridHighlight>());
                //Debug.Log("Draw cell");
                PositionInGrid(gridCell, new IntPair(i, j), unitPair);
                //Debug.Log(gridCell);
            }
        }
        highlightedSlots = new List<GridHighlight>();
    }

    public override void Notify()
    {
        // weaponManager.Notify();
        //if (inventory.FindItem(weaponManager.quickHealItem) != null)
        //{
        //    healthItemText.text = "x" + inventory.FindItem(weaponManager.quickHealItem).quantity;
        //    Debug.Log("Setting health item text" + healthItemText.text);
        //}
        //else
        //{
        //    healthItemText.text = "x0";
        //    Debug.Log("Setting health item text" + healthItemText.text);
        //}
        DrawInventory();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Notify();
    }

    private void DrawInventory()
    {
        CleanUpGrid();
        if (expandWithSlots)
        {
            ExpandGridSize();
        }
        CalcSlotDimensions();
        DrawGrid();
        DrawItems();
    }

    private void CleanUpGrid()
    {
        for (int i = 0; i < itemsGrid.childCount; i++)
        {
            // if (!itemsGrid.GetChild(i).GetComponent<ItemMenu>())
                Destroy(itemsGrid.GetChild(i).gameObject);
        }
    }

    private void DrawItems()
    {
        foreach (StoredItem item in inventory.items)
        {
            DrawItem(item);
        }
    }

    private void ExpandGridSize()
    {
        itemsGrid.sizeDelta = new Vector2(100 * inventory.size.y, 100 * inventory.size.x);
    }

    private void DrawItem(StoredItem storedItem)
    {
        Item item = storedItem.item;
        GameObject itemView = Instantiate(itemPrefab, itemsGrid);
        PositionInGrid(itemView, storedItem.position, storedItem.size);
        itemView.transform.SetParent(itemsGrid, false);
        if (item.stackable)
        {
            TMP_Text quantityText = itemView.transform.Find("QuantityText").GetComponent<TMP_Text>();
            quantityText.text = "x" + storedItem.quantity;
        }
        else
        {
            TMP_Text quantityText = itemView.transform.Find("QuantityText").GetComponent<TMP_Text>();
            quantityText.text = "";
        }
        if (item is RangedWeapon)
        {
            RangedWeapon wep = item as RangedWeapon;
            // TMP_Text ammoText = itemView.transform.Find("QuantityText").GetComponent<TMP_Text>();
            // if (!wep.canBeReloaded)
            // {
            //     ammoText.text = wep.currentAmmo + "%";
            // }
            // else
            // {
            //     ammoText.text = wep.currentAmmo + "/" + wep.clipSize;
            // }
            if (wep.installedModifications.Count > 0)
            {
                for (int i = 0; i < wep.installedModifications.Count; i++)
                {
                    Image weaponModIcon = itemView.transform.Find("WeaponAttachmentIcon${i + 1}").GetComponent<Image>();
                    if (storedItem.orientation == ItemOrientation.Horizontal)
                    {
                        weaponModIcon.sprite = wep.installedModifications[i].installedIconHorizontal;
                    }
                    else
                    {
                        weaponModIcon.sprite = wep.installedModifications[i].installedIconVertical;
                    }
                    weaponModIcon.gameObject.name = wep.installedModifications[i].itemName;
                    weaponModIcon.gameObject.SetActive(true);
                }
            }
        }
        Image img = itemView.transform.Find("Icon").GetComponent<Image>();
        if (item.horizontalIcon != null)
        {
            if (storedItem.orientation.Equals(ItemOrientation.Horizontal))
            {
                img.sprite = item.horizontalIcon;
            }
            else
            {
                img.sprite = item.verticalIcon;
            }
        }
        UpdateHoveredItem hovUpdater = itemView.GetComponent<UpdateHoveredItem>();
        hovUpdater.item = item;
        UIClickNotifier itemClicks = itemView.GetComponent<UIClickNotifier>();
        //itemView.transform.Find("EquippedIcon").gameObject.SetActive(item == weaponManager.GetEquippedItem());
        // if (item is Ammo && weaponManager.currentRangedWeapon != null && item == weaponManager.currentRangedWeapon.loadedAmmoType)
        // {
        //     weaponManager.CheckAmmo();
        // }
        /*
        
        */
        //if (item is RangedWeapon)
        //{
            // itemView.transform.Find("HotkeyNumber").gameObject.SetActive(weaponManager.ItemIsHotkeyed(item));
            // if (weaponManager.ItemIsHotkeyed(item))
            // {
            //     itemView.transform.Find("HotkeyNumber").GetComponentInChildren<TMP_Text>().text = (weaponManager.GetHotkeyIndex(item) + 1).ToString();
            // }
        //}
        //else
        //{
        //    itemView.transform.Find("HotkeyNumber").gameObject.SetActive(false);
        //}
        //itemView.transform.Find("ItemHighlight").GetComponent<RectTransform>().sizeDelta = itemView.GetComponent<RectTransform>().sizeDelta;
        // itemView.transform.Find("ItemHighlight").gameObject.SetActive(hovUpdater.hovering);
        if (storedItem.orientation == ItemOrientation.Vertical)
        {
            if (storedItem.item.verticalIcon!= null)
            {
                itemView.GetComponent<Image>().sprite = storedItem.item.verticalIcon;
            }
        }
        itemClicks.onLeft.AddListener(
            () =>
            {
                if (movingItem == null)
                {
                    Debug.Log("Attempting to move");
                    SelectedSlot.SetSelectedSlot_Static(gridSlots[storedItem.position.x, storedItem.position.y]);
                    if (beginMoveSound != null)
                    {
                        source.PlayOneShot(beginMoveSound);
                    }
                    MoveItem(itemView, storedItem);
                }
                //DisplayItemInfo(storedItem);
            }
        );
        itemClicks.onRight.AddListener(
            () =>
            {
                if (movingItem == null)
                {
                    if (item.itemType == ItemType.Consumable)
                    {
                        
                        Consumable consumable = (Consumable) item;
                        // consumable.Use();
                        // player.ApplyEffects(consumable.statModifiers);
                        if (consumable.onUseSound != null)
                        {
                            source.PlayOneShot(consumable.onUseSound, 0.75f);
                        }
                        Notify();
                        if (storedItem.quantity > 1)
                        {
                            inventory.RemoveItemQuantity(consumable, 1);
                        }
                        else
                        {
                            RemoveItem(storedItem);
                            Tooltip.HideToolTip_Static();
                        }
                        if (consumable.itemToSpawnAfterConsumption != null)
                        {
                            inventory.AddItem(consumable.itemToSpawnAfterConsumption, 1);
                        }
                    }
                }
            }
        );
    }

    private void RemoveItem(StoredItem item, bool drop=false)
    {
        inventory.RemoveItem(item, drop);
        // if (weaponManager.ItemIsHotkeyed(item.item))
        // {
        //     Hotkey.RemoveHotkeyItem_static(item.item);
        //     weaponManager.RemoveItemFromHotkey(item.item);
        // }
        DrawInventory();
    }

    private void MoveItem(GameObject gridObj, StoredItem item)
    {
        // PlayerInteraction.LockInteraction();
        movingObject = gridObj;
        movingItem = item;
        movingObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        movingObject.transform.Find("Icon").GetComponent<Image>().color = new Color(movingObject.GetComponent<Image>().color.r, movingObject.GetComponent<Image>().color.b, movingObject.GetComponent<Image>().color.g, 0.5f);
        movingObject.transform.SetAsLastSibling();
        gridObj.transform.Find("QuantityText").gameObject.SetActive(false);
        // gridObj.transform.Find("ItemHighlight").gameObject.SetActive(false);
        gridObj.transform.Find("EquippedIcon").gameObject.SetActive(false);
        // gridObj.transform.Find("HotkeyNumber").gameObject.SetActive(false);
        Canvas addedCanvas = movingObject.AddComponent<Canvas>();
        addedCanvas.overrideSorting = true;
        addedCanvas.sortingOrder = 1;
        // foreach (GridHighlight slot in allSlots)
        // {
        //     slot.movingItems = true;
        // }
        // for (int i = 0; i < itemsGrid.childCount; i++)
        // {
        //     if (itemsGrid.GetChild(i).GetComponent<UpdateHoveredItem>())
        //     {
        //         itemsGrid.GetChild(i).GetComponent<CanvasGroup>().blocksRaycasts = false;
        //     }
        // }
        // SelectedSlot.SetDimensions_Static(item.size.x, item.size.y);
        // StartCoroutine(ItemMouseFollow());
    }

    public void AllowMove(GameObject obj, StoredItem item)
    {
        movingObject = obj;
        movingItem = item;
        foreach (GridHighlight slot in allSlots)
        {
            slot.movingItems = true;
        }
        for (int i = 0; i < itemsGrid.childCount; i++)
        {
            if (itemsGrid.GetChild(i).GetComponent<UpdateHoveredItem>())
            {
                itemsGrid.GetChild(i).GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }
        // movingObject.transform.SetParent(GetComponentInParent<Canvas>().transform);
        SelectedSlot.SetDimensions_Static(movingItem.size.x, movingItem.size.y);
        StartCoroutine(ItemMouseFollow());
    }

    IEnumerator CombinationHighlights()
    {
        while (!Input.GetMouseButtonDown(1))
        {
            yield return null;
        }
        Notify();
    }

    IEnumerator ItemMouseFollow()
    {
        startingOrientation = movingItem.orientation;
        //yield return new WaitForSeconds(0.5f);
        Debug.Log("Clicked in");
        //if (Input.GetMouseButton(0))
        //{
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Tooltip.HideToolTip_Static();
        GridHighlight selectedSlot = SelectedSlot.GetSelectedSlot_static();
        while (!Mouse.current.leftButton.isPressed)
        {
            // if (Mouse.current.rightButton.wasPressedThisFrame && movingItem.size.x != movingItem.size.y)
            // {
            //     ToggleOrientation();
            // }
            //if (mousePosition != Input.mousePosition)
            //{
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(itemsGrid, Mouse.current.position.ReadValue(), itemsGrid.parent.GetComponentInParent<Canvas>().worldCamera, out pos);
            //Debug.Log(movingItem.item.itemName);
                movingObject.transform.position = itemsGrid.TransformPoint(pos);
            //}
            //mousePosition = Input.mousePosition;
            //worldPosition.z = itemsGrid.localPosition.z;
            //movingObject.transform.localPosition = new Vector3(mousePosition.x, mousePosition.y, mousePosition.z);
            //movingObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, movingObject.transform.position.z);
            //movingObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, movingObject.transform.position.z);
            if (selectedSlot != SelectedSlot.GetSelectedSlot_static())
            {
                DetectOpenSlots();
            }
            
            // int row = (int)(movingObject.transform.localPosition.y / unitSlot.x) * -1;
            // int col = (int)(movingObject.transform.localPosition.x / unitSlot.y);
            // Debug.Log(row + " " + col);
            selectedSlot = SelectedSlot.GetSelectedSlot_static();
            yield return null;
        }
        RepositionMovingObject();
        foreach (GridHighlight slot in allSlots)
        {
            slot.movingItems = false;
        }
        for (int i = 0; i < itemsGrid.childCount; i++)
        {
            if (itemsGrid.GetChild(i).GetComponent<UIClickNotifier>())
            {
                itemsGrid.GetChild(i).GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }
        //}
        //else
        //{
        //    yield return null;
        //}
    }

    public bool CursorOverGrid()
    {
        if (movingObject == null)
        {
            return false;
        }
        Vector2 movingObjPos = itemsGrid.InverseTransformPoint(movingObject.transform.position);
        if (itemsGrid.rect.Contains(movingObjPos))
        {
            return true;
        }
        return false;
    }

    private void DetectOpenSlots()
    {
        GridHighlight selection = SelectedSlot.GetSelectedSlot_static();

        foreach (GridHighlight slot in allSlots)
        {
            if (selection == null || !CursorOverGrid())
            {
                slot.UnHighlight();
                continue;
            }
            if (slot.slotPosition.x < (SelectedSlot.GetDimensions_Static().x + selection.slotPosition.x) && slot.slotPosition.x >= selection.slotPosition.x && slot.slotPosition.y < (SelectedSlot.GetDimensions_Static().y + selection.slotPosition.y) && slot.slotPosition.y >= selection.slotPosition.y)
            {
                slot.slotEligible = inventory.IsPositionValid(movingItem, selection.slotPosition.x, selection.slotPosition.y, movingItem);
                if (movingItem.item.itemCombinationsPossible.Count > 0)
                {
                    slot.canCombine = inventory.CombinationValid(movingItem.item, selection.slotPosition.x, selection.slotPosition.y, movingItem);
                }
                slot.Highlight();
            }
            else
            {
                slot.UnHighlight();
            }
        }
        /*
        int endPositionX = (selection.slotPosition.x + movingItem.item.size.x);
        if (inventory.size.x < endPositionX)
        {
            endPositionX = inventory.size.x - (selection.slotPosition.x + movingItem.item.size.x);
        }
        int endPositionY = (selection.slotPosition.y + movingItem.item.size.y);
        if (inventory.size .y < endPositionY)
        {
            endPositionY = inventory.size.y - (selection.slotPosition.y + movingItem.item.size.y);
        }

        if (movingObject != null)
        {
            for (int i = selection.slotPosition.x; i < endPositionX; i++)
            {
                for (int j = selection.slotPosition.y; j < endPositionY; j++)
                {
                    
                    Debug.Log(inventory.IsPositionValid(movingItem.item, i, j));
                }
            }
        }
        */
    }

    public bool MouseOutsideUI()
    {
        Debug.Log("Checking if mouse over UI");
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Mouse.current.position.ReadValue();
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (RaycastResult result in results)
        {
            //Debug.Log(result.gameObject);
            if (result.gameObject != movingObject)
            {
                return false;
            }
        }
        return true;
    }

    private void RepositionMovingObject()
    {
        if (movingObject != null)
        {
            int row = (int)(movingObject.transform.localPosition.y / unitSlot.x) * -1;
            int col = (int)(movingObject.transform.localPosition.x / unitSlot.y);

            if (!inventory.MoveItem(movingItem, new IntPair(row, col)))
            {
                if (invalidMoveSound != null)
                {
                    source.PlayOneShot(invalidMoveSound);
                }
                if (startingOrientation != movingItem.orientation)
                {
                    ToggleOrientation();
                }
                else
                {
                    inventory.MoveItem(movingItem, new IntPair(movingItem.position.x, movingItem.position.y));
                }
            }
            else
            {
                if (inventory.CombinationValid(movingItem.item, row, col, movingItem))
                {
                    Debug.Log("Item combo valid");
                    foreach (ItemCombination combo in movingItem.item.itemCombinationsPossible)
                    {
                        StoredItem itemToFind = inventory.FindItem(combo.otherItemRequired);
                        if (itemToFind.position.x == movingItem.position.x && itemToFind.position.y == movingItem.position.y)
                        {
                            if (combo.combinationSound != null)
                            {
                                source.PlayOneShot(combo.combinationSound);
                            }
                            Combine(movingItem, itemToFind, combo.itemResult);
                            //DynamicCursor.ChangeCursor_Static(CursorType.None);
                        }
                    }
                }
                else
                {
                    if (validMoveSound != null)
                    {
                        source.PlayOneShot(validMoveSound);
                    }
                    // if (InventoryManager.IsItemOverGrid() == 1 && !CursorOverGrid())
                    // {
                    //     InventoryManager.TransferItem(inventory, movingItem);
                    // }
                    // else
                    // {
                    //     inventory.MoveItem(movingItem, new IntPair(row, col));
                    // }
                    inventory.MoveItem(movingItem, new IntPair(row, col));
                    // if (inventory.FindItem(movingItem.item) == null)
                    // {
                    //     inventory.AddItem(movingItem.item, movingItem.quantity);
                    // }
                }
                if (movingItem.orientation == ItemOrientation.Vertical && movingItem.item.verticalIcon != null)
                {
                    movingObject.transform.Find("Icon").GetComponent<Image>().sprite = movingItem.item.verticalIcon;
                }
                else if (startingOrientation != movingItem.orientation && movingItem.orientation == ItemOrientation.Horizontal)
                {
                    movingObject.GetComponent<Image>().sprite = movingItem.item.horizontalIcon;
                }
                movingObject.transform.Find("QuantityText").gameObject.SetActive(true);
                // movingObject.transform.Find("EquippedIcon").gameObject.SetActive(true);
                // movingObject.transform.Find("HotkeyNumber").gameObject.SetActive(true);
            }
            Destroy(movingObject.GetComponent<Canvas>());
            movingObject = null;
            movingItem = null;
            // PlayerInteraction.UnlockInteraction();
        }
    }

    public void ToggleOrientation()
    {
        if (movingItem.orientation == ItemOrientation.Horizontal)
        {
            movingObject.transform.Find("Icon").GetComponent<Image>().sprite = movingItem.item.horizontalIcon;
            movingObject.transform.localEulerAngles = new Vector3(0, 0, -90);
            movingObject.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
            movingItem.orientation = ItemOrientation.Vertical;
            movingItem.size = new IntPair(movingItem.size.y, movingItem.size.x);
            SelectedSlot.SetDimensions_Static(movingItem.size.x, movingItem.size.y);
            if (movingItem.item is RangedWeapon)
            {
                RangedWeapon wep = movingItem.item as RangedWeapon;
                foreach (Transform child in movingObject.transform)
                {
                    foreach (WeaponModification mod in wep.installedModifications)
                    {
                        if (mod.itemName == child.name)
                        {
                            //child.localEulerAngles = new Vector3(0, 0, -90);
                            child.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
                            child.GetComponent<Image>().sprite = mod.installedIconHorizontal;
                            //child.GetComponent<Image>().sprite = mod.installedIconVertical;
                        }
                    }
                }
            }
        }
        else
        {
            movingObject.transform.localEulerAngles = new Vector3(0, 0, 0);
            movingObject.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            movingItem.orientation = ItemOrientation.Horizontal;
            movingItem.size = new IntPair(movingItem.size.y, movingItem.size.x);
            movingObject.GetComponent<RectTransform>().sizeDelta = new Vector2(unitSlot.y * movingItem.size.y, unitSlot.x * movingItem.size.x);
            SelectedSlot.SetDimensions_Static(movingItem.size.x, movingItem.size.y);
            movingObject.transform.Find("Icon").GetComponent<Image>().sprite = movingItem.item.horizontalIcon;
            if (movingItem.item is RangedWeapon)
            {
                RangedWeapon wep = movingItem.item as RangedWeapon;
                foreach (Transform child in movingObject.transform)
                {
                    foreach (WeaponModification mod in wep.installedModifications)
                    {
                        Debug.Log(mod.itemName);
                        if (mod.itemName == child.name)
                        {
                            //child.localEulerAngles = new Vector3(0, 0, 0);
                            child.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                            child.GetComponent<Image>().sprite = mod.installedIconHorizontal;
                            //child.GetComponent<Image>().sprite = mod.installedIconVertical;
                        }
                    }
                }
            }
        }
        DetectOpenSlots();
    }

    private void HighlightCombined(StoredItem storedItem)
    {
        foreach (UpdateHoveredItem itemObject in itemsGrid.GetComponentsInChildren<UpdateHoveredItem>())
        {
            itemObject.gameObject.GetComponent<UIClickNotifier>().onLeft.RemoveAllListeners();
            foreach (ItemCombination combo in storedItem.item.itemCombinationsPossible)
            {
                if (combo.otherItemRequired != itemObject.item)
                {
                    itemObject.gameObject.GetComponent<Image>().color = Color.gray;
                }
                else
                {
                    itemObject.gameObject.GetComponent<UIClickNotifier>().onLeft.AddListener(
                        () =>
                        {
                            StoredItem itemToFind = inventory.FindItem(itemObject.item);
                            source.PlayOneShot(combo.combinationSound);
                            Combine(storedItem, itemToFind, combo.itemResult);
                        }
                    );
                }
            }
        }
        StartCoroutine(CombinationHighlights());
    }

    private void Combine(StoredItem item1, StoredItem item2, Item result)
    {
        if (item1.item is WeaponModification)
        {
            if (item2.item.itemType.Equals(ItemType.RangedWeapon))
            {
                RangedWeapon rWep = item2.item as RangedWeapon;
                rWep.InstallMod((WeaponModification)item1.item);
                // weaponManager.EquipItem(rWep);
                RemoveItem(item1);
            }
        }
        else if (item2.item is WeaponModification)
        {
            if (item1.item.itemType.Equals(ItemType.RangedWeapon))
            {
                RangedWeapon rWep = item1.item as RangedWeapon;
                rWep.InstallMod((WeaponModification)item2.item);
                // weaponManager.EquipItem(rWep);
                RemoveItem(item2);
            }
        }
        else
        {
            RemoveItem(item1);
            RemoveItem(item2);
            inventory.AddItem(result);
        }
        foreach (UpdateHoveredItem itemObject in itemsGrid.GetComponentsInChildren<UpdateHoveredItem>())
        {
            itemObject.gameObject.GetComponent<Image>().color = Color.white;
        }
    }

    public GameObject GetMovingObject()
    {
        return movingObject;
    }

    public void CloseView()
    {
        if (movingObject != null)
        {
            RepositionMovingObject();
        }
        //else if (weaponManager.currentItem != null && !(weaponManager.currentItem is MeleeWeaponItem))
        //{
        //    weaponManager.equippedItemCursor.gameObject.SetActive(true);
        //}
    }

    public void OpenView()
    {
        //if (weaponManager.currentRangedWeapon != null && weaponManager.currentRangedWeapon.twoHanded)
        //{
        //    weaponManager.HideWeapon();
        //}
        //else if (weaponManager.currentItem != null)
        //{
        //    weaponManager.equippedItemCursor.gameObject.SetActive(false);
        //}
    }

    public void ClearInventory()
    {
        inventory.items = new List<StoredItem>();
    }

    public void DisplayItemInfo (StoredItem itemToDisplay)
    {
        itemDisplayIcon.sprite = itemToDisplay.item.horizontalIcon;
        itemDescription.text = itemToDisplay.item.description;
        //if (itemToDisplay.item is RangedWeapon)
        //{
        //    RangedWeapon wep = (RangedWeapon)itemToDisplay.item;
        //    itemDescription.text += "\nDamage: " + wep.loadedAmmoType.damage;
        //    itemDescription.text += "\Clip Size: " + wep.loadedAmmoType.damage;
        //}
        itemDescription.text += "\nQuantity: " + itemToDisplay.quantity;
    }

    // public object SaveState()
    // {
    //     return new SaveData
    //     {
    //         inventory = inventory,
    //     };
    // }

    // public void LoadState(object saveState)
    // {
    //     string json = JsonConvert.SerializeObject(saveState);
    //     SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json, new JsonSerializerSettings
    //     {
    //         PreserveReferencesHandling = PreserveReferencesHandling.Objects,
    //         TypeNameHandling = TypeNameHandling.Auto
    //         //ReferenceResolverProvider = () => new GenericResolver<Item>(p => p.itemName)
    //     });
    // }
}
