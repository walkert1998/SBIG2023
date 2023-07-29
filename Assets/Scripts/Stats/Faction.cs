using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Factions
{
    Player,
    Wild,
    Bandit
}

[System.Serializable]
public class Faction
{
    public Factions faction;
}
