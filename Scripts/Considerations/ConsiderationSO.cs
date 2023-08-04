using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsiderationSO : ScriptableObject
{
    public abstract float Evaluate(AIContext context);
}
