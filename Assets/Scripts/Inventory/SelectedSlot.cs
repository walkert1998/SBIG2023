using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedSlot : MonoBehaviour
{
    private static SelectedSlot instance;
    private GridHighlight selectedSlot;
    private IntPair dimensions;

    void Start()
    {
        instance = this;
        ClearSelectedSlot_Static();
    }

    private void SetSelectedSlot(GridHighlight slot)
    {
        selectedSlot = slot;
    }

    private void ClearSelectedSlot()
    {
        selectedSlot = null;
    }

    private GridHighlight GetSelectedSlot()
    {
        return selectedSlot;
    }

    private void SetDimensions(int x, int y)
    {
        dimensions = new IntPair(x, y);
    }

    private IntPair GetDimensions()
    {
        return dimensions;
    }

    public static void SetSelectedSlot_Static(GridHighlight slot)
    {
        instance.SetSelectedSlot(slot);
    }

    public static void ClearSelectedSlot_Static()
    {
        //Debug.Log("Hiding tooltip");
        instance.ClearSelectedSlot();
    }

    public static GridHighlight GetSelectedSlot_static()
    {
        return instance.selectedSlot;
    }

    public static void SetDimensions_Static(int x, int y)
    {
        instance.SetDimensions(x, y);
    }

    public static IntPair GetDimensions_Static()
    {
        return instance.dimensions;
    }
}
