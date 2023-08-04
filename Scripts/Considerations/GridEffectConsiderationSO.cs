using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "ScriptableObjects/Considerations/GridEffect")]
public class GridEffectConsiderationSO : ConsiderationSO
{
    [SerializeField, Range(0.0f, 1.0f)] float unaffectedScore = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] float affectedScore = 0.0f;

    public override float Evaluate(AIContext context)
    {
        float score = context.TargetNode.GridEffects.Where(effect => effect.TargetAlliance == context.ExecutingUnit.Alliance).FirstOrDefault() == null
            ? unaffectedScore : affectedScore;

        return score;
    }
}
