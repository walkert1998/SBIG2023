using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
//using UnityStandardAssets.Characters.FirstPerson;

[CreateAssetMenu(menuName = "Items/Create Ranged Weapon")]
public class RangedWeapon : Item
{
    public int damage = 0;
    [Header("Stat effects on user")]
    public StatModifier[] equipEffects;
    [Header("Timing")]
    public float reloadTime = 1.5f;
    public float timeBetweenShots = 1.0f;
    [JsonIgnore]
    public float nextShotTime = 1.0f;
    public float drawTime = 0.5f;
    public float holsterTime = 0.5f;
    [Header("Recoil")]
    public float recoil;
    [Header("Ammo")]
    public int clipSize = 1;
    public int numProjectilesPerShot = 1;
    public float projectileSpread = 0f;
    [Tooltip("When true, every projectile in a shot decreases ammo by 1.")]
    public bool countEachProjectileAsAmmo = false;
    public bool burstFire = false;
    public int currentAmmo;
    public int ammoReserve;
    [JsonIgnore]
    public List<Ammo> ammoTypes;
    public Ammo loadedAmmoType;
    public bool canBeReloaded = true;
    [Tooltip("When true, this weapon is reloaded one round at a time.")]
    public bool singleReload = false;
    [Header("Aiming")]
    public float aimingFOV = 80f;
    public float aimAssistRadius = 0.25f;
    [Header("Weapon Modifications")]
    [JsonIgnore]
    public List<WeaponModification> availableModifications;
    public List<WeaponModification> installedModifications;

    public List<WeaponAltFireTypes> altFireTypes;

    public bool modEffectsActive = false;
    //protected MouseLook mouseLook;
    //protected FirstPersonController controller;
    //MuzzleFlash muzzleFlash;
    [Header("Weapon Model")]
    public bool twoHanded = false;
    [JsonIgnore]
    public GameObject viewModel;
    //public Animator animator;
    [JsonIgnore]
    public Vector3 aimPosition;
    [JsonIgnore]
    public GameObject shellCasing;
    //[TextArea]
    public string effectLightPath;

    //private GameObject main_camera;
    [Header("Misc")]
    //public Inventory playerInventory;
    public bool crankToCharge = false;
    public float minChargeMultiplier = 0.25f;
    public float maxChargeMultiplier = 1.25f;
    public int chargeLostPerShot = 10;
    public int chargePerCrank = 20;
    public int defaultChargeValue = 80;
    [Tooltip("For defining weapons like a flamethrower")]
    public bool streamWeapon = false;
    //public GameSettings gameSettings;

    [Header("Sound Effects")]
    [JsonIgnore]
    public AudioClip[] gunshotSounds;
    [JsonIgnore]
    public AudioClip unSilencedSound;
    [JsonIgnore]
    public AudioClip silencedSound;
    [JsonIgnore]
    public AudioClip weaponDrawSound;
    [JsonIgnore]
    public AudioClip noAmmoSound;
    [JsonIgnore]
    public AudioClip reloadSound;
    //Sleep playerSleep;
    //public AmbienceController ambienceController;
    //public PauseMenu pauseMenu;

    [Header("Impact Textures")]
    public string woodBulletImpactID;
    public string metalBulletImpactID;
    public string bloodBulletImpactID;

    public void add_ammo(int amount)
    {
        ammoReserve += amount;
    }

    public int InstallMod(WeaponModification mod)
    {
        // Debug.Log("Installing mod " + mod.itemName);
        foreach (WeaponModification availableMod in availableModifications)
        {
            // Check mod is available
            if (availableMod.baseItemID == mod.baseItemID)
            {
                foreach (WeaponModification installedMod in installedModifications)
                {
                    // Check if mod already installed
                    if (installedMod.baseItemID == mod.baseItemID)
                    {
                        // Debug.Log("Mod is already installed");
                        return 1;
                    }
                }
            }
        }
        installedModifications.Add(mod);
        if (mod.applyEffectsOnInstall)
        {
            ApplyModEffects(mod);
        }
        return 0;
    }

    public WeaponModification UnInstallMod(WeaponModification mod)
    {
        if (installedModifications.Contains(mod))
        {
            installedModifications.Remove(mod);
            RemoveModEffects(mod);
            //gameModelID.transform.Find(mod.onWeaponModel.name).gameObject.SetActive(false);
            //mod.onWeaponModel.SetActive(false);
            // Debug.Log("Uninstalled " + mod.itemName);
            return mod;
        }
        return null;
    }

    public void ApplyModEffects(WeaponModification mod)
    {
        if (!modEffectsActive)
        {
            foreach (WeaponModificationEffect effect in mod.modificationEffects)
            {
                if (effect.valueType == ModifierValueType.FlatValue)
                {
                    if (effect.modType == ModificationType.AmmoChange)
                    {
                        clipSize += (int)effect.value;
                    }
                    else if (effect.modType == ModificationType.DamageChange)
                    {
                        damage += (int)effect.value;
                    }
                    else if (effect.modType == ModificationType.RateOfFireChange)
                    {
                        timeBetweenShots += effect.value;
                    }
                    else if (effect.modType == ModificationType.ReloadSpeedChange)
                    {
                        reloadTime += effect.value;
                    }
                    else if (effect.modType == ModificationType.RecoilChange)
                    {
                        recoil += effect.value;
                    }
                    else if (effect.modType == ModificationType.Scope)
                    {
                        aimingFOV += effect.value;
                    }
                    else if (effect.modType == ModificationType.ProjectilesPerShotChange)
                    {
                        numProjectilesPerShot += (int)effect.value;
                    }
                    else if (effect.modType == ModificationType.Grapple)
                    {
                        altFireTypes.Add(WeaponAltFireTypes.Grapple);
                    }
                }
                else if (effect.valueType == ModifierValueType.Percentage)
                {
                    float value = (effect.value / 100) + 1;
                    if (effect.modType == ModificationType.AmmoChange)
                    {
                        clipSize *= (int)value;
                    }
                    else if (effect.modType == ModificationType.DamageChange)
                    {
                        damage *= (int)value;
                    }
                    else if (effect.modType == ModificationType.RateOfFireChange)
                    {
                        timeBetweenShots *= value;
                    }
                    else if (effect.modType == ModificationType.ReloadSpeedChange)
                    {
                        reloadTime *= value;
                    }
                    else if (effect.modType == ModificationType.RecoilChange)
                    {
                        recoil *= value;
                    }
                    else if (effect.modType == ModificationType.Scope)
                    {
                        aimingFOV /= value;
                    }
                    else if (effect.modType == ModificationType.ProjectilesPerShotChange)
                    {
                        numProjectilesPerShot *= (int)value;
                    }
                }
                //else if (effect.modType == ModificationType.Silencer)
                //{
                //    gunshotSound = silencedSound;
                //}
            }
        }
        modEffectsActive = true;
    }

    public void RemoveModEffects(WeaponModification mod)
    {
        if (modEffectsActive)
        {
            foreach (WeaponModificationEffect effect in mod.modificationEffects)
            {
                // Debug.Log(mod.itemName + " "  + effect.modType.ToString() + " " + effect.value);
                if (effect.valueType == ModifierValueType.FlatValue)
                {
                    if (effect.modType == ModificationType.AmmoChange)
                    {
                        clipSize -= (int)effect.value;
                    }
                    else if (effect.modType == ModificationType.DamageChange)
                    {
                        damage -= (int)effect.value;
                    }
                    else if (effect.modType == ModificationType.RateOfFireChange)
                    {
                        if (timeBetweenShots - effect.value > 0)
                        {
                            timeBetweenShots -= effect.value;
                            // Debug.Log(timeBetweenShots);
                        }
                    }
                    else if (effect.modType == ModificationType.ReloadSpeedChange)
                    {
                        reloadTime -= effect.value;
                    }
                    else if (effect.modType == ModificationType.RecoilChange)
                    {
                        recoil -= effect.value;
                    }
                    else if (effect.modType == ModificationType.Scope)
                    {
                        aimingFOV -= effect.value;
                    }
                    else if (effect.modType == ModificationType.ProjectilesPerShotChange)
                    {
                        numProjectilesPerShot -= (int)effect.value;
                    }
                    else if (effect.modType == ModificationType.Grapple)
                    {
                        altFireTypes.Remove(WeaponAltFireTypes.Grapple);
                    }
                }
                else if (effect.valueType == ModifierValueType.Percentage)
                {
                    float value = (effect.value / 100) + 1;
                    if (effect.modType == ModificationType.AmmoChange)
                    {
                        clipSize /= (int)value;
                    }
                    else if (effect.modType == ModificationType.DamageChange)
                    {
                        damage /= (int)value;
                    }
                    else if (effect.modType == ModificationType.RateOfFireChange)
                    {
                        timeBetweenShots /= value;
                    }
                    else if (effect.modType == ModificationType.ReloadSpeedChange)
                    {
                        reloadTime /= value;
                    }
                    else if (effect.modType == ModificationType.RecoilChange)
                    {
                        recoil /= value;
                    }
                    else if (effect.modType == ModificationType.Scope)
                    {
                        aimingFOV *= value;
                    }
                    else if (effect.modType == ModificationType.ProjectilesPerShotChange)
                    {
                        numProjectilesPerShot /= (int)value;
                    }
                }
                //else if (effect.modType == ModificationType.Silencer)
                //{
                //    gunshotSound = unSilencedSound;
                //}
            }
        }
        modEffectsActive = false;
    }
}

public enum WeaponAltFireTypes
{
    Aim,
    ChangeAmmo,
    ApplyModificationEffect,
    Grapple,
    None
}
