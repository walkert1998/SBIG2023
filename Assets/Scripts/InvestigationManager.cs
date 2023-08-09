using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InvestigationManager : MonoBehaviour
{
    public static InvestigationManager instance;
    public TMP_Text objectiveText;
    public bool arrestedMurderer = false;
    public bool allSuspectsDead = false;
    public int livingSuspects = 4;
    public CharacterInstance timmy;
    public CharacterInstance marvin;
    
    public CharacterInstance redHerring;
    public CharacterInstance violet;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public static void SetObjectiveText(string objectiveText)
    {
        instance.objectiveText.text = objectiveText;
    }

    public static void ArrestMurderer()
    {
        Debug.Log("ARRESTED MURDER!!! " + instance.livingSuspects);
        instance.livingSuspects--;
        instance.arrestedMurderer = true;
        if (instance.livingSuspects > 0)
        {
            SetObjectiveText("Murderer 'found', eliminate remaining residents");
        }
        else
        {
            SetObjectiveText("Case solved and targets eliminated, speak to Officer Wilson");
        }
    }

    public static bool MurdererFound()
    {
        return instance.arrestedMurderer;
    }

    public static void KillSuspect()
    {
        instance.livingSuspects--;
        if (!instance.arrestedMurderer)
        {
            if (instance.livingSuspects == 0)
            {
                instance.allSuspectsDead = true;
                SetObjectiveText("All suspects dead, speak to Officer Wilson");
            }
        }
        else if (instance.livingSuspects == 0)
        {
            SetObjectiveText("Case solved and targets eliminated, speak to Officer Wilson");
        }
    }
}
