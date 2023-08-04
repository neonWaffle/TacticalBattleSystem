using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Considerations/Cooldown")]
public class CooldownConsiderationSO : ConsiderationSO
{
    [SerializeField, Min(1)] int cooldownDuration;
    [SerializeField] AnimationCurve responseCurve;

    public override float Evaluate(AIContext context)
    {
        if (context.Ability == null)
        {
            return 0.0f;
        }

        float score = !context.ExecutingUnit.AbilityHandler.AbilityTypeCooldowns.ContainsKey(context.Ability.AbilitySO.AbilityType)
            ? 1.0f
            : (BattleManager.Instance.CurrentRound - context.ExecutingUnit.AbilityHandler.AbilityTypeCooldowns[context.Ability.AbilitySO.AbilityType] - 1) / (float)cooldownDuration;

        return Mathf.Clamp01(responseCurve.Evaluate(score));
    }
}
