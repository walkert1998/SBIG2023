using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create Consumable")]
public class Consumable : Item
{
    public int value;
    public StatModifier[] statModifiers;
    public bool alcoholic = false;
    public Item itemToSpawnAfterConsumption;


    public override void Use()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (alcoholic)
        {
            //Drunkeness playerDrunkeness = player.GetComponent<Drunkeness>();
            //Debug.Log(playerDrunkeness.currentDrunkeness);
            //playerDrunkeness.IncreaseDrunkeness(value * 0.75f);
            //if (playerDrunkeness.currentDrunkeness >= 90)
            //{
            //    Health playerHealth = player.GetComponent<Health>();
            //    playerHealth.DamageCharacter(value + 5);
            //    HelpText._DisplayHelpText("You're too drunk, sober up before drinking more to avoid liver damage!");
            //}
        }
    }
}

public enum ConsumableType
{
    Health,
    Stamina
}
