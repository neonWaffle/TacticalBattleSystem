using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObjects/Considerations/Threat")]
public class ThreatConsiderationSO : ConsiderationSO
{
    [SerializeField] bool evaluateTargetUnit;
    [SerializeField] bool checkIfIsThreat;
    [SerializeField, Range(0.0f, 1.0f)] float threatScore = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] float noThreatScore = 0.0f;

    public override float Evaluate(AIContext context)
    {
        if (evaluateTargetUnit && context.TargetNode.Unit == null)
        {
            return 0.0f;
        }

        var unitToCheck = evaluateTargetUnit ? context.TargetNode.Unit : context.ExecutingUnit; 
        var enemyUnits = BattleManager.Instance.OverallUnitOrder.Where(unit => Utilities.IsAllianceCompatible(unitToCheck, unit, Alliance.Enemy));
        
        if (enemyUnits.Count() == 0)
        {
            return noThreatScore;
        }

        if (checkIfIsThreat)
        {
            return unitToCheck.StatusEffectHandler.IsIncapacitated ? noThreatScore : unitToCheck.DataSO.UnitType != UnitType.Melee
                ? threatScore : enemyUnits.Where(unit => Utilities.IsPathToTargetNodeInUnitSpeedRange(unit, unitToCheck.Node)).FirstOrDefault() != null
                ? threatScore : noThreatScore;
        }
        else
        {
            return enemyUnits.Where(unit => unit.DataSO.UnitType != UnitType.Melee && !unit.StatusEffectHandler.IsIncapacitated).FirstOrDefault() != null
                || (enemyUnits.Where(unit => unit.DataSO.UnitType == UnitType.Melee && !unit.StatusEffectHandler.IsIncapacitated
                && Utilities.IsPathToTargetNodeInUnitSpeedRange(unit, unitToCheck.Node)).FirstOrDefault() != null)
                ? threatScore : noThreatScore;
        }
    }
}
