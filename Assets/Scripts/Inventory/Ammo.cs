using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Items/Create Ammo")]
public class Ammo : Item
{
    public int damage;
    public float effectTime = 1.0f;
    public DamageModifier[] statModifiers;
    [JsonIgnore]
    public GameObject projectile;
    [JsonIgnore]
    public GameObject impactEffect;
}

public enum DamageEffect
{
    None,
    Fire,
    EMP,
    Poison,
    Knockout,
    Explosive
}
