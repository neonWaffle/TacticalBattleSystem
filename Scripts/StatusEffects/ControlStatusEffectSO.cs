using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlType { Charm, Incapacitate }

[CreateAssetMenu(menuName = "ScriptableObjects/StatusEffects/Control")]
public class ControlStatusEffectSO : StatusEffectSO
{
    [SerializeField] ControlType controlType;

    public override void Apply(Unit executingUnit, Unit targetUnit)
    {
        switch (controlType)
        {
            case ControlType.Charm:
                targetUnit.StatusEffectHandler.IsCharmed = true;
                break;
             case ControlType.Incapacitate:
                targetUnit.StatusEffectHandler.IsIncapacitated = true;
                break;
        }
    }

    public override void Remove(Unit executingUnit, Unit targetUnit)
    {
        switch (controlType)
        {
            case ControlType.Charm:
                targetUnit.StatusEffectHandler.IsCharmed = false;
                break;
            case ControlType.Incapacitate:
                targetUnit.StatusEffectHandler.IsIncapacitated = false;
                break;
        }
    }
}
