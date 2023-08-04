using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UtilityAI : MonoBehaviour
{
    public BaseAction[] SelectAction(Unit executingUnit)
    {
        ScoredAction bestAction = null;
        float highestScore = 0.0f;

        var abilities = !executingUnit.ActionHandler.WasSwiftActionUsed ? new List<Ability>(executingUnit.AbilityHandler.Abilities) : new List<Ability>(2);
        abilities.Add(executingUnit.AbilityHandler.DefaultAttackAbility);
        abilities.Add(executingUnit.AbilityHandler.DefenceAbility);

        foreach (var ability in abilities)
        {
            if (ability.AbilitySO.IsInfiniteUse || ability.RemainingUses > 0)
            {
                var targetNodes = FindAbilityAimTargetNodes(ability, executingUnit);
                foreach (var targetNode in targetNodes)
                {
                    float score = CalculateScore(executingUnit, ability.AbilitySO.Considerations, targetNode, ability);
                    if (score > highestScore)
                    {
                        if (ability.AbilitySO.IsTouchRange)
                        {
                            //If the unit is in range for ability execution
                            if (Grid.Instance.IsInRange(executingUnit.Node, targetNode, 1))
                            {
                                bestAction = new ScoredAction(score, new AbilityAction(ability, executingUnit, targetNode));
                            }
                            else
                            {
                                var destinationNode = Grid.Instance.GetClosestWalkableEmptyNeighbourNode(executingUnit.Node, targetNode);
                                //If the unit needs to walk to the target, but the target is close enough to be reached this turn
                                if (Utilities.IsPathToTargetNodeInUnitSpeedRange(executingUnit, destinationNode))
                                {
                                    bestAction = new ScoredAction(score, 
                                        new MoveAction(executingUnit,
                                        Pathfinder.Instance.FindPath(executingUnit.Node.transform.position, destinationNode.transform.position)),
                                        new AbilityAction(ability, executingUnit, targetNode));
                                }
                                //If the target is too far away for the unit to reach it, only move this turn
                                else
                                {
                                    bestAction = new ScoredAction(score,
                                        new MoveAction(executingUnit,
                                        Pathfinder.Instance.FindPath(executingUnit.Node.transform.position, destinationNode.transform.position)));
                                }
                            }
                        }
                        else
                        {
                            bestAction = new ScoredAction(score, new AbilityAction(ability, executingUnit, targetNode));
                        }
                        highestScore = score;
                    }
                }
            }
        }

        float fleeScore = CalculateScore(executingUnit, executingUnit.DataSO.FleeConsiderations, executingUnit.Node);
        if (fleeScore > highestScore)
        {
            var nodesInRange = Grid.Instance.GetNodesInRange(executingUnit.Node, executingUnit.DataSO.Speed);
            foreach (var node in nodesInRange)
            {
                if (node.IsWalkable && node.Unit == null)
                {
                    float fleeTargetScore = CalculateScore(executingUnit, executingUnit.DataSO.FleeTargetConsiderations, node);
                    if (fleeTargetScore > highestScore)
                    {
                        var path = Pathfinder.Instance.FindPath(executingUnit.Node.transform.position, node.transform.position);
                        if (path != null)
                        {
                            bestAction = new ScoredAction(fleeTargetScore,
                                new MoveAction(executingUnit, path));
                            highestScore = fleeTargetScore;
                        }
                    }
                }
            }
        }

        if (bestAction == null || bestAction.Actions == null || bestAction.Actions.Length == 0)
        {
            return new AbilityAction[] { new AbilityAction(executingUnit.AbilityHandler.DefenceAbility, executingUnit, executingUnit.Node) };
        }

        return bestAction.Actions;
    }

    List<GridNode> FindAbilityAimTargetNodes(Ability ability, Unit executingUnit)
    {
        switch (ability.AbilitySO.AimTargetType)
        {
            case TargetType.None:
                return new List<GridNode>() { executingUnit.Node };
            case TargetType.Unit:
                return Grid.Instance.GetOccupiedNodes()
                    .Where(node => Utilities.IsAllianceCompatible(executingUnit, node.Unit, ability.AbilitySO.AimTargetAlliance)).ToList();
            case TargetType.Any:
                return Grid.Instance.GetAllNodes();
            case TargetType.EmptyNode:
                return Grid.Instance.GetEmptyNodes();
            default:
                return null;
        }
    }

    float CalculateScore(Unit executingUnit, ConsiderationSO[] considerations, GridNode targetNode, Ability ability = null)
    {
        float finalScore = 0.0f;

        foreach (var consideration in considerations)
        {
            float considerationScore = 0.0f;

            //Affected area considerations should only be concerned with the node on which the ability is going to be cast
            if (ability != null && !ability.AbilitySO.AffectTargetNodeOnly && !(consideration is AffectedAreaConsiderationSO))
            {
                float abilityScore = 0.0f;
                var affectedNodes = ability.GetAffectedNodes(targetNode);
                foreach (var node in affectedNodes)
                {
                    float nodeScore = consideration.Evaluate(new AIContext(executingUnit, node, ability));
                    if (Mathf.Approximately(nodeScore, 0.0f))
                    {
                        return 0.0f;
                    }
                    abilityScore += nodeScore;
                }
                considerationScore = abilityScore / affectedNodes.Count();
            }
            else
            {
                considerationScore = consideration.Evaluate(new AIContext(executingUnit, targetNode, ability));
            }

            if (Mathf.Approximately(considerationScore, 0.0f))
            {
                return 0.0f;
            }
            finalScore += considerationScore;
        }
        finalScore /= considerations.Length;
        return finalScore;
    }
}
