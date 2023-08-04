using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "ScriptableObjects/Considerations/DamageType")]
public class DamageTypeConsiderationSO : ConsiderationSO
{
    [SerializeField] bool checkForResistance;
    [SerializeField, Range(0.0f, 1.0f)] float hasDamageTypeScore = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] float hasNoDamageTypeScore = 0.0f;
    public override float Evaluate(AIContext context)
    {
        if (context.Ability == null)
        {
            return 0.0f;
        }    

        var healthBehaviour = context.Ability.AbilitySO.Behaviours.Where(behaviour => behaviour is HealthModifyingBehaviourSO).FirstOrDefault() as HealthModifyingBehaviourSO;
        if (healthBehaviour == null)
        {
            return 0.0f;
        }

        float score = context.TargetNode.Unit != null 
            && ((checkForResistance && context.TargetNode.Unit.DataSO.Resistances.Contains(healthBehaviour.DamageType))
            || (!checkForResistance && context.TargetNode.Unit.DataSO.Weaknesses.Contains(healthBehaviour.DamageType))) 
            ? hasDamageTypeScore : hasNoDamageTypeScore;
 
        return score;
    }
}
