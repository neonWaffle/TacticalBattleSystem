using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "ScriptableObjects/Considerations/StatusEffect")]
public class StatusEffectConsiderationSO : ConsiderationSO
{
    [SerializeField] StatusEffectSO statusEffectSO;
    [SerializeField, InfoBox("If false, only checks if the unit has a status effect")] bool evaluateDuration;
    [SerializeField] AnimationCurve responseCurve;

    public override float Evaluate(AIContext context)
    {
        if (context.TargetNode.Unit == null)
        {
            return 0.0f;
        }

        var statusEffect = context.TargetNode.Unit.StatusEffectHandler.StatusEffects.Where(effect => effect.StatusEffectSO == statusEffectSO).FirstOrDefault();
        return Mathf.Clamp01(responseCurve.Evaluate(statusEffect == null ? 0.0f : evaluateDuration ? statusEffect.RemainingTurns / (float)statusEffectSO.Duration : 1.0f));
    }
}
