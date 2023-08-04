using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "ScriptableObjects/Battles")]
public class BattleDataSO : ScriptableObject
{
    [field: SerializeField] public string ID { get; private set; }
    [field: SerializeField] public string SceneTitle { get; private set; }
    [field: SerializeField] public List<UnitFormation> UnitFormations { get; private set; }
    [field: Header("Rewards")]
    [field: SerializeField] public BattleDataSO NextBattleDataSO { get; private set; }
    [field: SerializeField, Min(0)] public int Money { get; private set; }
    [field: SerializeField, Min(0)] public int XP { get; private set; }

    void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(ID))
        {
            ID = Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
