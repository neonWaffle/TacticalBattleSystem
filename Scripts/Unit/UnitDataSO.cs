using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

public enum UnitType { Melee, Mage, Ranger }

[CreateAssetMenu(menuName = "ScriptableObjects/Unit")]
public class UnitDataSO : ScriptableObject
{
    [field: Header("General info")]
    [field: SerializeField] public string ID { get; private set; }
    [field: SerializeField, ResizableTextArea] public string Title { get; private set; }
    [field: SerializeField] public Unit Prefab { get; private set; }
    [field: SerializeField, ShowAssetPreview] public Sprite Portrait { get; private set; }
    [field: SerializeField, Min(0)] public int Cost { get; private set; }

    [field: Header("Stats")]
    [field: SerializeField, Min(1)] public int HitPoints { get; private set; }
    [field: SerializeField, Min(0)] public int AC { get; private set; }
    [field: SerializeField, Min(1)] public int Speed { get; private set; }
    [field: SerializeField, Min(1)] public int Attack { get; private set; }
    [field: SerializeField, Range(1, 20)] public int Dexterity { get; private set; }
    [field: SerializeField, Range(1, 20)] public int Constitution { get; private set; }
    [field: SerializeField, Range(1, 20)] public int Wisdom { get; private set; }

    [field: Header("Abilities")]
    [field: SerializeField] public AbilitySO DefaultAttackAbility { get; private set; }
    [field: SerializeField] public AbilitySO DefenceAbility { get; private set; }
    [field: SerializeField] public AbilitySO[] Abilities { get; private set; }

    [field: Header("Resistances/Weaknesses")]
    [field: SerializeField] public DamageType[] Resistances { get; private set; }
    [field: SerializeField] public DamageType[] Weaknesses { get; private set; }

    [field: Header("AI")]
    [field: SerializeField] public UnitType UnitType { get; private set; } 
    [field: SerializeField] public ConsiderationSO[] FleeConsiderations { get; private set; }
    [field: SerializeField] public ConsiderationSO[] FleeTargetConsiderations { get; private set; }

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