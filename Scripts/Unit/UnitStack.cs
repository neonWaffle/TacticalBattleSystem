using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UnitStack
{
    [field: SerializeField] public UnitDataSO UnitSO { get; private set; }
    [field: SerializeField, Min(0)] public int Amount { get; private set; }

    public event Action<int> OnAmountUpdated;

    public UnitStack(UnitDataSO unitSO, int amount)
    {
        UnitSO = unitSO;
        Amount = amount;
    }

    public void UpdateAmount(int amount)
    {
        Amount = amount;

        OnAmountUpdated?.Invoke(amount);
    }
}
