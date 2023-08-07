using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Factions
{
    Player,
    MarvinGreen,
    SirRedHerring,
    VioletCadaver,
    TimmyCadaver
}

[System.Serializable]
public class Faction
{
    public Factions faction;
}
