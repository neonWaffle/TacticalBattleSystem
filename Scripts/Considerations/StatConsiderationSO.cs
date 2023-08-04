using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Considerations/Stat")]
public class StatConsiderationSO : ConsiderationSO
{
    [SerializeField] StatType statType;
    [SerializeField] AnimationCurve responseCurve;
    [SerializeField] bool checkTargetUnit;

    public override float Evaluate(AIContext context)
    {
        if (checkTargetUnit && context.TargetNode == null)
        {
            return 0.0f;
        }

        var score = responseCurve.Evaluate(checkTargetUnit
            ? context.TargetNode.Unit.StatHandler.Stats[statType].Value / 20.0f
            : context.ExecutingUnit.StatHandler.Stats[statType].Value / 20.0f);

        return Mathf.Clamp01(score);
    }
}
