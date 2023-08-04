using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "ScriptableObjects/Considerations/TouchRange")]
public class TouchRangeConsiderationSO : ConsiderationSO
{
    [SerializeField, Range(0.0f, 1.0f)] float inRangeScore = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] float notInRangeScore = 0.5f;

    public override float Evaluate(AIContext context)
    {
        var closestNode = Grid.Instance.GetClosestWalkableEmptyNeighbourNode(context.ExecutingUnit.Node, context.TargetNode);
        float score = closestNode != null && Utilities.IsPathToTargetNodeInUnitSpeedRange(context.ExecutingUnit, closestNode)
            ? inRangeScore : notInRangeScore;      
        return score;
    }
}
