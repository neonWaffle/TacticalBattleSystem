using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Considerations/Health")]
public class HealthConsiderationSO : ConsiderationSO
{
    [SerializeField] AnimationCurve responseCurve;
    [SerializeField] bool evaluateTargetUnit;

    public override float Evaluate(AIContext context)
    {
        if (evaluateTargetUnit && context.TargetNode.Unit == null)
        {
            return 0.0f;
        }

        float score = evaluateTargetUnit
            ? responseCurve.Evaluate(context.TargetNode.Unit.HealthHandler.CurrentHP / (float)(context.TargetNode.Unit.DataSO.HitPoints * context.TargetNode.Unit.HealthHandler.MaxUnitAmount))
            : responseCurve.Evaluate(context.ExecutingUnit.HealthHandler.CurrentHP / (float)(context.ExecutingUnit.DataSO.HitPoints * context.ExecutingUnit.HealthHandler.MaxUnitAmount));

        return Mathf.Clamp01(score);
    }
}
