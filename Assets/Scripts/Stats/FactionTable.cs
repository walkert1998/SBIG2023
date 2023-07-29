using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class FactionTable : ScriptableObject
{
    [SerializeField]
    private Faction[] factions;
    [SerializeField]
    private int[] opinions;

    public void PrintAllOpinions()
    {
        foreach (int op in opinions)
        {
            Debug.Log(op);
        }
    }

    public int GetOpinion(Factions faction1, Factions faction2)
    {
        for (int i = 0; i < factions.Length; i++)
        {
            if (faction1 == factions[i].faction)
            {
                for (int j = 0; j < factions.Length; j++)
                {
                    if (faction2 == factions[j].faction)
                    {
                        return opinions[i + j];
                    }
                }
            }
        }
        return -1;
    }

    public int ChangeOpinion(Factions faction1, Factions faction2, int opinionChange)
    {
        for (int i = 0; i < factions.Length; i++)
        {
            if (faction1 == factions[i].faction)
            {
                for (int j = 0; j < factions.Length; j++)
                {
                    if (faction2 == factions[j].faction)
                    {
                        opinions[i * factions.Length + j] += opinionChange;
                        opinions[j * factions.Length + i] += opinionChange;
                        return 0;
                    }
                }
            }
        }
        return -1;
    }
}
