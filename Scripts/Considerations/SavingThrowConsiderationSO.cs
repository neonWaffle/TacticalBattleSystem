using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Considerations/SavingThrow")]
public class SavingThrowConsiderationSO : ConsiderationSO
{
    [SerializeField] AnimationCurve responseCurve;

    public override float Evaluate(AIContext context)
    {
        if (context.Ability == null || context.TargetNode.Unit == null)
        {
            return 0.0f;
        }

        return Mathf.Clamp01(responseCurve.Evaluate(
            (context.TargetNode.Unit.StatHandler.CalculateSavingThrowBonus(context.Ability.AbilitySO.SavingThrowType) * 2.0f + 10.0f) / 20.0f));
    }
}
