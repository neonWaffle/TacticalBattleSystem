using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Utilities
{
    public static Alliance GetAlliance(Unit unit, Alliance requiredAlliance)
    {
        if (requiredAlliance == Alliance.Ally)
        {
            if ((unit.Alliance == Alliance.Ally && !unit.StatusEffectHandler.IsCharmed) || (unit.Alliance == Alliance.Enemy && unit.StatusEffectHandler.IsCharmed))
            {
                return Alliance.Ally;
            }
            return Alliance.Enemy;
        }
        else
        {
            if ((unit.Alliance == Alliance.Ally && !unit.StatusEffectHandler.IsCharmed) || (unit.Alliance == Alliance.Enemy && unit.StatusEffectHandler.IsCharmed))
            {
                return Alliance.Enemy;
            }
            return Alliance.Ally;
        }
    }

    public static bool IsAllianceCompatible(Unit instigatorUnit, Unit targetUnit, Alliance requiredAlliance)
    {
        var alliance = GetAlliance(instigatorUnit, requiredAlliance);
        return (targetUnit.Alliance == alliance && !targetUnit.StatusEffectHandler.IsCharmed)
            || (targetUnit.Alliance != alliance && targetUnit.StatusEffectHandler.IsCharmed);
    }

    public static bool IsPathToTargetNodeInUnitSpeedRange(Unit unit, GridNode targetNode)
    {
        var path = Pathfinder.Instance.FindPath(unit.Node.transform.position, targetNode.transform.position);
        return path != null && path.Count() <= unit.StatHandler.Stats[StatType.Speed].Value;
    }
}
