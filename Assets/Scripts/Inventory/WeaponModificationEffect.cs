using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponModificationEffect
{
    public ModificationType modType;
    public ModifierValueType valueType;
    public float value;
}

public enum ModifierValueType
{
    Percentage,
    FlatValue
}