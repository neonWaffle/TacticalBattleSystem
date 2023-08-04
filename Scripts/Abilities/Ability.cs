using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Ability
{
    public AbilitySO AbilitySO { get; private set; }
    public int RemainingUses { get; private set; }

    public Unit AbilityOwner { get; private set; }

    public bool CanBeUsed { get; private set; }

    public event Action<Ability> OnAbilityUsed;
    public event Action OnStatusChanged;

    public Ability(AbilitySO abilitySO, Unit abilityOwner)
    {
        AbilitySO = abilitySO;
        AbilityOwner = abilityOwner;

        if (!AbilitySO.IsInfiniteUse)
        {
            RemainingUses = AbilitySO.Uses;
        }

        CanBeUsed = true;
    }

    public void AdvanceRound()
    {
        if (RemainingUses > 0 || AbilitySO.IsInfiniteUse)
        {
            ChangeStatus(true);
        }
    }

    public void ChangeStatus(bool canBeUsed)
    {
        CanBeUsed = canBeUsed;
        OnStatusChanged?.Invoke();
    }

    public void Use(GridNode targetNode)
    {
        if (!AbilitySO.IsInfiniteUse)
        {
            RemainingUses--;
        }
        
        if (AbilitySO.AffectTargetNodeOnly)
        {
            if (!AbilitySO.UseSaveThrow || (AbilitySO.UseSaveThrow && targetNode.Unit != null && MakeSavingThrowCheck(targetNode.Unit)))
            {
                AbilitySO.Use(AbilityOwner, targetNode);
            }
        }
        else
        {
            var affectedNodes = GetAffectedNodes(targetNode);
            foreach (var node in affectedNodes)
            {
                if (!AbilitySO.UseSaveThrow || (AbilitySO.UseSaveThrow && node.Unit != null && MakeSavingThrowCheck(node.Unit)))
                {
                    AbilitySO.Use(AbilityOwner, node);
                }
            }
        }

        ChangeStatus(false);
        OnAbilityUsed?.Invoke(this);
    }

    public List<GridNode> GetNodesInAimArea(GridNode targetNode)
    {
        List<GridNode> nodes = new List<GridNode>((int)Mathf.Pow(AbilitySO.AffectedAreaSize, 2));
        switch (AbilitySO.AimArea)
        {
            case AimArea.SingleTile:
                nodes.Add(targetNode);
                break;
            case AimArea.Square:
                nodes = Grid.Instance.GetNodesInRange(targetNode, Mathf.FloorToInt(AbilitySO.AffectedAreaSize * 0.5f));
                nodes.Add(targetNode);
                break;
        }
        return nodes;
    }

    public List<GridNode> GetAffectedNodes(GridNode targetNode)
    {
        var nodesInAimArea = GetNodesInAimArea(targetNode);
        return nodesInAimArea.Where(node => IsNodeTargetCompatible(node, AbilitySO.TargetType, AbilitySO.TargetAlliance)).ToList();
    }

    public bool CanBeCastOnTarget(GridNode targetNode)
    {
        return IsNodeTargetCompatible(targetNode, AbilitySO.AimTargetType, AbilitySO.AimTargetAlliance) && IsTargetNodeInRange(AbilityOwner, targetNode);
    }

    bool IsNodeTargetCompatible(GridNode node, TargetType targetType, Alliance targetAlliance)
    {
        return targetType == TargetType.Any
            || (targetType == TargetType.EmptyNode && node.Unit == null)
            || (targetType == TargetType.Unit
            && node.Unit != null && Utilities.IsAllianceCompatible(AbilityOwner, node.Unit, targetAlliance));
    }

    bool IsTargetNodeInRange(Unit unit, GridNode targetNode)
    {
        if (!AbilitySO.IsTouchRange)
            return true;

        var closestEmptyNode = Grid.Instance.GetClosestWalkableEmptyNeighbourNode(unit.Node, targetNode);
        return closestEmptyNode != null && Utilities.IsPathToTargetNodeInUnitSpeedRange(unit, closestEmptyNode);
    }

    bool MakeSavingThrowCheck(Unit targetUnit)
    {
        int roll = Dice.Roll(DiceType.D20, 1);
        int bonus = targetUnit.StatHandler.CalculateSavingThrowBonus(AbilitySO.SavingThrowType);

        if ((roll > 1 && roll + bonus >= AbilitySO.DC) || roll == 20)
        {
            BattleLogger.Instance.DisplayMessage(new BattleLogMessage(
                $"{targetUnit.DataSO.Title.Bold()}: { AbilitySO.SavingThrowType.ToString().Bold()} saving throw succeeded.", $"Roll: {roll}, Bonus: {bonus}, DC: { AbilitySO.DC}"));

            return false;
        }
        else
        {
            BattleLogger.Instance.DisplayMessage(new BattleLogMessage(
                $"{targetUnit.DataSO.Title.Bold()}: { AbilitySO.SavingThrowType.ToString().Bold()} saving throw failed.", $"Roll: {roll}, Bonus: {bonus}, DC: { AbilitySO.DC}"));

            return true;
        }
    }
}
