using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Considerations/ValidPath")]
public class ValidPathConsiderationSO : ConsiderationSO
{
    [SerializeField] bool checkClosestEmptyNode; //Melee abilities require the unit to move next to the target node instead of the node itself
    [SerializeField, Range(0.0f, 1.0f)] float isValidScore = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] float isNotValidScore = 0.0f;

    public override float Evaluate(AIContext context)
    {
        var targetNode = checkClosestEmptyNode ?
            Grid.Instance.GetClosestWalkableEmptyNeighbourNode(context.ExecutingUnit.Node, context.TargetNode) : context.TargetNode;
        if (targetNode == null)
        {
            return 0.0f;
        }

        var path = Pathfinder.Instance.FindPath(context.ExecutingUnit.Node.transform.position, targetNode.transform.position);
        return path != null && path.Count > 0 ? isValidScore : isNotValidScore;
    }
}
