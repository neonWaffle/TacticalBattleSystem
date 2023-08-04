using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "ScriptableObjects/Considerations/AffectedArea")]
public class AffectedAreaConsiderationSO : ConsiderationSO
{
    [SerializeField] AnimationCurve responseCurve;
    [SerializeField] bool excludeCharmedUnits = true;

    public override float Evaluate(AIContext context)
    {
        if (context.Ability == null)
        {
            return 0.0f;
        }

        var affectedNodes = context.Ability.GetNodesInAimArea(context.TargetNode);
        var affectedUnits = affectedNodes.Where(node => node.Unit != null 
            && ((excludeCharmedUnits && !node.Unit.StatusEffectHandler.IsCharmed) || !excludeCharmedUnits)
            && Utilities.IsAllianceCompatible(context.ExecutingUnit, node.Unit, context.Ability.AbilitySO.TargetAlliance)).ToList();

        float score = affectedUnits.Count / (float)Mathf.Pow(context.Ability.AbilitySO.AffectedAreaSize, 2);
        return Mathf.Clamp01(responseCurve.Evaluate(score));
    }
}
