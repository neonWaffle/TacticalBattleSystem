using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObjects/Considerations/EnemyQuantity")]
public class EnemyQuantityConsiderationSO : ConsiderationSO
{
    [SerializeField, Min(0)] int minQuantity = 1;
    [SerializeField, Min(0)] int maxQuantity = 3;
    [SerializeField] bool excludeCharmedEnemies;
    [SerializeField, Range(0.0f, 1.0f)] float inRangeScore = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] float outOfRangeScore = 0.0f;

    public override float Evaluate(AIContext context)
    {
        var enemyAlliance = Utilities.GetAlliance(context.ExecutingUnit, Alliance.Enemy);
        var enemies = BattleManager.Instance.OverallUnitOrder.Where(unit => excludeCharmedEnemies
        ? unit.Alliance == enemyAlliance && !unit.StatusEffectHandler.IsCharmed
        : unit.Alliance == enemyAlliance);

        return enemies.Count() >= minQuantity && enemies.Count() <= maxQuantity ? inRangeScore : outOfRangeScore;
    }
}
