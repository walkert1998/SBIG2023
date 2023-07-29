using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionAllegiance : MonoBehaviour
{
    public Factions faction;

    public void ChangeFactionTo(Factions newFaction)
    {
        faction = newFaction;
    }
}
