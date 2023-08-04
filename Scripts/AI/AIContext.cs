using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIContext
{
    public Unit ExecutingUnit { get; private set; }
    public GridNode TargetNode { get; private set; }
    public Ability Ability { get; private set; }

    public AIContext(Unit executingUnit, GridNode targetNode, Ability ability)
    {
        ExecutingUnit = executingUnit;
        TargetNode = targetNode;
        Ability = ability;
    }
}
