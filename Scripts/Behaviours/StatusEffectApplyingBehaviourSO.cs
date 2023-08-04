using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Behaviours/StatusEffectApplying")]
public class StatusEffectApplyingBehaviourSO : BehaviourSO
{
    [SerializeField] StatusEffectSO[] statusEffects;

    public override void Use(Unit executingUnit, GridNode targetNode)
    {
        if (targetNode.Unit == null)
            return;

        foreach (var statusEffect in statusEffects)
        {
            targetNode.Unit.StatusEffectHandler.AddStatusEffect(new StatusEffect(statusEffect, executingUnit, targetNode.Unit));
        }
    }
}
