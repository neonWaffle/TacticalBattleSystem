using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObjects/Considerations/EnemyProximity")]
public class EnemyProximityConsiderationSO : ConsiderationSO
{
    public enum RangeType { Custom, UnitSpeed, Ability }
    [SerializeField] RangeType rangeType;
    [SerializeField, ShowIf("rangeType", RangeType.Custom)] int customRange;
    [SerializeField, InfoBox("Should the proximity be checked from the target node or the executing unit's node?")] bool startFromTargetNode;
    [SerializeField, Range(0.0f, 1.0f)] float noEnemiesInRangeScore = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] float enemiesInRangeScore = 0.0f;

    public override float Evaluate(AIContext context)
    {
        float score = 0.0f;
        int range = 0;

        switch (rangeType)
        {
            case RangeType.Custom:
                range = customRange;
                break;
            case RangeType.UnitSpeed:
                range = context.ExecutingUnit.StatHandler.Stats[StatType.Speed].Value;
                break;
            case RangeType.Ability:
                range = context.Ability != null ? context.Ability.AbilitySO.AffectedAreaSize : 0;
                break;
        }

        var nodesInRange = Grid.Instance.GetNodesInRange(startFromTargetNode ? context.TargetNode : context.ExecutingUnit.Node, range);
        score = nodesInRange.Where(node => node.Unit != null && Utilities.IsAllianceCompatible(context.ExecutingUnit, node.Unit, Alliance.Enemy)).FirstOrDefault() != null
            ? enemiesInRangeScore : noEnemiesInRangeScore;

        return score;
    }
}
