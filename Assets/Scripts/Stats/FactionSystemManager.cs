using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionSystemManager : MonoBehaviour
{
    private static FactionSystemManager instance;
    public FactionTable factionTable;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public static int GetFactionOpinion(Factions faction1, Factions faction2)
    {
        return instance.factionTable.GetOpinion(faction1, faction2);
    }

    public static int ChangeFactionOpinion(Factions faction1, Factions faction2, int opinionChange)
    {
        return instance.factionTable.ChangeOpinion(faction1, faction2, opinionChange);
    }
}
