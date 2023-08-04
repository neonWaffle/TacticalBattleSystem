using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class Stat
{
    int baseValue;
    public int Value => Mathf.Max(0, baseValue + modifiers.Sum(modifier => modifier.Amount));

    List<StatModifier> modifiers = new List<StatModifier>();

    public event Action OnChanged;

    public Stat(int value)
    {
        baseValue = value;
    }

    public void AddModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);
        OnChanged?.Invoke();
    }

    public void RemoveModifier(StatModifier modifier)
    {
        modifiers.Remove(modifier);
        OnChanged?.Invoke();
    }
}
