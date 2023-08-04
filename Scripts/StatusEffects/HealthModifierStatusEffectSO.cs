using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/StatusEffects/HealthModifying")]
public class HealthModifierStatusEffectSO : StatusEffectSO
{
    [SerializeField] HealthModifyingBehaviourSO behaviourSO;

    public override void Apply(Unit executingUnit, Unit targetUnit)
    {
        behaviourSO.Use(executingUnit, targetUnit.Node);
    }
}
