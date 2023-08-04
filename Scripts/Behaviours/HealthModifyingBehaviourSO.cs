using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

public enum DamageType { Default, Electricity, Fire, Poison, Ice }

[CreateAssetMenu(menuName = "ScriptableObjects/Behaviours/HealthModifying")]
public class HealthModifyingBehaviourSO : BehaviourSO
{
    [field: SerializeField] public int BaseAmount { get; private set; }
    [field: SerializeField] public bool IsHealing { get; private set; }
    [field: SerializeField, HideIf("IsHealing")] public DamageType DamageType { get; private set; }
    [field: SerializeField] public DiceType DiceType { get; private set; }
    [field: SerializeField] public int RollTimes { get; private set; }
    [field: SerializeField, HideIf("IsHealing")] public bool ShouldApplyModifiers { get; private set; }

    public int MinBaseAmount => BaseAmount;
    public int MaxBaseAmount => RollTimes * (int)DiceType + BaseAmount;

    void OnValidate()
    {
        if (IsHealing)
        {
            ShouldApplyModifiers = false;
        }
    }

    public override void Use(Unit executingUnit, GridNode targetNode)
    {
        if (targetNode.Unit == null)
            return;

        int finalAmount = Dice.Roll(DiceType, RollTimes) + BaseAmount;
        finalAmount = CalculateFinalHealthAmount(executingUnit, targetNode, finalAmount);

        if (IsHealing)
        {
            targetNode.Unit.HealthHandler.Heal(executingUnit, finalAmount);
        }
        else
        {
            targetNode.Unit.HealthHandler.TakeDamage(executingUnit, finalAmount);
        }
    }

    public int CalculateFinalHealthAmount(Unit executingUnit, GridNode targetNode, int healthAmount)
    {
        if (ShouldApplyModifiers)
        {
            healthAmount = ApplyModifiers(executingUnit, targetNode.Unit, healthAmount);
        }

        if (!IsHealing && DamageType != DamageType.Default)
        {
            if (targetNode.Unit.DataSO.Weaknesses.Contains(DamageType))
            {
                healthAmount = (int)(healthAmount * 1.5f);
            }
            else if (targetNode.Unit.DataSO.Resistances.Contains(DamageType))
            {
                healthAmount = (int)(healthAmount * 0.5f);
            }
        }

        healthAmount *= executingUnit.HealthHandler.UnitAmount;

        healthAmount = IsHealing
            ? Mathf.Min(healthAmount, targetNode.Unit.HealthHandler.MaxUnitAmount * targetNode.Unit.DataSO.HitPoints - targetNode.Unit.HealthHandler.CurrentHP)
            : Mathf.Min(healthAmount, targetNode.Unit.HealthHandler.CurrentHP);

        return healthAmount;
    }

    int ApplyModifiers(Unit executingUnit, Unit targetUnit, int amount)
    {
        if (IsHealing)
        {
            return amount;
        }
        amount = (int)(amount * (21.0f + executingUnit.StatHandler.Stats[StatType.Attack].Value - targetUnit.StatHandler.Stats[StatType.AC].Value) * 0.05f);
        return Mathf.Max(0, amount);
    }
}
