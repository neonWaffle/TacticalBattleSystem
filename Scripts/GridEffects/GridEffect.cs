using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GridEffect : MonoBehaviour
{
    Unit executingUnit;

    public int TurnOrderID { get; set; }
    public int RemainingTurns { get; private set; }
    List<GridNode> affectedNodes;

    [SerializeField] EffectDurationType durationType;
    [field: SerializeField] public Alliance TargetAlliance{ get; private set; }
    [SerializeField, Min(1)] int duration;
    [SerializeField, Min(1)] int areaSize;
    [SerializeField] bool useSavingThrow;
    [SerializeField, Range(1, 20), ShowIf("useSavingThrow")] int dc;
    [SerializeField, ShowIf("useSavingThrow")] SavingThrowType savingThrowType;

    [SerializeField] BehaviourSO[] behaviours;

    public void Setup(Unit executingUnit, GridNode targetNode)
    {
        this.executingUnit = executingUnit;

        BattleManager.Instance.AddGridEffect(this);
        RemainingTurns = duration;
        
        affectedNodes = Grid.Instance.GetNodesInRange(targetNode, Mathf.FloorToInt(areaSize * 0.5f));
        affectedNodes.Add(targetNode);

        foreach (var node in affectedNodes)
        {
            node.GridEffects.Add(this);
        }

        Apply();
    }

    public void Apply()
    {
        foreach (var node in affectedNodes)
        {
            if (node.Unit != null && Utilities.IsAllianceCompatible(executingUnit, node.Unit, TargetAlliance))
            {
                if ((useSavingThrow && MakeSavingThrowCheck(node.Unit)) || !useSavingThrow)
                {
                    foreach (var behaviour in behaviours)
                    {
                        behaviour.Use(executingUnit, node);
                    }
                }
            }
        }

        if (durationType == EffectDurationType.Expiring)
        {
            RemainingTurns--;
        }
    }

    public void Remove()
    {
        foreach (var node in affectedNodes)
        {
            node.GridEffects.Remove(this);
        }
    }

    bool MakeSavingThrowCheck(Unit targetUnit)
    {
        int roll = Dice.Roll(DiceType.D20, 1);
        int bonus = targetUnit.StatHandler.CalculateSavingThrowBonus(savingThrowType);

        if ((roll > 1 && roll + bonus >= dc) || roll == 20)
        {
            BattleLogger.Instance.DisplayMessage(new BattleLogMessage(
                $"{targetUnit.DataSO.Title.Bold()}: {savingThrowType.ToString().Bold()} saving throw succeeded.", $"Roll: {roll}, Bonus: {bonus}, DC: {dc}"));

            return false;
        }
        else
        {
            BattleLogger.Instance.DisplayMessage(new BattleLogMessage(
                $"{targetUnit.DataSO.Title.Bold()}: {savingThrowType.ToString().Bold()} saving throw failed.", $"Roll: {roll}, Bonus: {bonus}, DC: {dc}"));
            
            return true;
        }
    }
}
