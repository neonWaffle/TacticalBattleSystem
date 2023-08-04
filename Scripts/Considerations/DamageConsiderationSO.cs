using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObjects/Considerations/Damage")]
public class DamageConsiderationSO : ConsiderationSO
{
    [SerializeField] AnimationCurve responseCurve;

    public override float Evaluate(AIContext context)
    {
        if (context.Ability == null || context.TargetNode.Unit == null)
        {
            return 0.0f;
        }

        var healthBehaviour = context.Ability.AbilitySO.Behaviours.Where(behaviour => behaviour is HealthModifyingBehaviourSO).FirstOrDefault() as HealthModifyingBehaviourSO;
        if (healthBehaviour == null)
        {
            return 0.0f;
        }

        int estimatedDamage = healthBehaviour.CalculateFinalHealthAmount(
                context.ExecutingUnit,
                context.TargetNode,
                Mathf.FloorToInt((healthBehaviour.MinBaseAmount + healthBehaviour.MaxBaseAmount) * 0.5f));

        return Mathf.Clamp01(responseCurve.Evaluate((float)estimatedDamage / context.TargetNode.Unit.HealthHandler.CurrentHP));
    }
}
