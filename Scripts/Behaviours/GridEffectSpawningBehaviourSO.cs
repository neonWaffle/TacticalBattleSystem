using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridEffectSpawningBehaviour", menuName = "ScriptableObjects/Behaviours/GridEffectSpawning")]
public class GridEffectSpawningBehaviourSO : BehaviourSO
{
    [SerializeField] GridEffect gridEffectPrefab;

    public override void Use(Unit executingUnit, GridNode targetNode)
    {
        var gridEffect = Instantiate(gridEffectPrefab, targetNode.transform.position, Quaternion.identity);
        gridEffect.Setup(executingUnit, targetNode);
    }
}
