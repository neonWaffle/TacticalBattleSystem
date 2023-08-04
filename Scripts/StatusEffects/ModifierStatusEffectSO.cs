using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/StatusEffects/Modifier")]
public class ModifierStatusEffectSO : StatusEffectSO
{
    [field: SerializeField] public StatModifier StatModifier { get; private set; }

    public override void Apply(Unit executingUnit, Unit targetUnit)
    {
        StatModifier.Apply(targetUnit);
    }

    public override void Remove(Unit executingUnit, Unit targetUnit)
    {
        StatModifier.Remove(targetUnit);
    }
}
