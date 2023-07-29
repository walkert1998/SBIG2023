using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Items/Create Weapon Modifcation")]
public class WeaponModification : Item
{
    public List<WeaponModificationEffect> modificationEffects;
    public string onWeaponModelName;
    public bool applyEffectsOnInstall = true;
    [JsonIgnore]
    public Sprite installedIconHorizontal;
    [JsonIgnore]
    public Sprite installedIconVertical;
}


public enum ModificationType
{
    Silencer,
    AmmoChange,
    DamageChange,
    RateOfFireChange,
    ReloadSpeedChange,
    RecoilChange,
    Scope,
    ProjectilesPerShotChange,
    Grapple
}
