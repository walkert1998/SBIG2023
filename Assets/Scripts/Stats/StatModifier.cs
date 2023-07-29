using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatModifier
{
    public StatType stat;
    public float value;
    public StatModifierType type;
    [Tooltip("-1 = Permanent until removed.\n1 = Instant one-time effect.\n N = Effect lasts for N seconds.")]
    public float duration;
}

public enum StatModifierType
{
    FlatValue,
    Percent
}
