using System.Collections;
using System.Collections.Generic;

public class DialogueOptionRequirement
{
    public DialogueOptionRequirementType requirementType;
    public DialogueOptionOperators comparisonOperator;
    public int intValue;
    public Item requiredItem;
    public Factions factionRequirement;
    public bool boolValue;
}
