using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ModifierType { Flat, Percentage }

[Serializable]
public class StatModifier
{
    [field: SerializeField] public StatType StatType { get; private set; }
    [field: SerializeField] public int Amount { get; private set; }
    [field: SerializeField] public ModifierType ModifierType { get; private set; }

    public void Apply(Unit targetUnit)
    {
        targetUnit.StatHandler.Stats[StatType].AddModifier(this);
    }

    public void Remove(Unit targetUnit)
    {
        targetUnit.StatHandler.Stats[StatType].RemoveModifier(this);
    }
}
