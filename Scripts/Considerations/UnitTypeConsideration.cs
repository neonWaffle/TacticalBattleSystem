using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Considerations/UnitType")]
public class UnitTypeConsideration : ConsiderationSO
{
    [SerializeField] UnitType unitType;
    [SerializeField, Range(0.0f, 1.0f)] float isPreferredTypeScore = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] float isNotPreferredTypeScore = 0.5f;

    public override float Evaluate(AIContext context)
    {
        if (context.TargetNode.Unit == null)
        {
            return 0.0f;
        }

        return context.TargetNode.Unit.DataSO.UnitType == unitType ? isPreferredTypeScore : isNotPreferredTypeScore;
    }
}
