using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourSO : ScriptableObject
{
    public abstract void Use(Unit executingUnit, GridNode targetNode);
}
