using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class StatusEffectHandler : MonoBehaviour
{
    Unit unit;

    public List<StatusEffect> StatusEffects { get; private set; }
    public bool IsIncapacitated { get; set; }
    public bool IsCharmed { get; set; }

    public event Action OnUpdated;

    void Awake()
    {
        unit = GetComponent<Unit>();

        StatusEffects = new List<StatusEffect>();
    }

    public void AddStatusEffect(StatusEffect statusEffect)
    {
        if (unit.HealthHandler.IsDead)
            return;

        var existingStatusEffect = StatusEffects.FirstOrDefault(effect => effect.StatusEffectSO == statusEffect.StatusEffectSO);
        if (existingStatusEffect != null)
        {
            RemoveStatusEffect(existingStatusEffect);
        }

        StatusEffects.Add(statusEffect);
        if (statusEffect.StatusEffectSO.ApplyImmediately)
        {
            statusEffect.Apply();
        }

        OnUpdated?.Invoke();
    }

    public void ApplyStatusEffects()
    {
        for (int i = StatusEffects.Count - 1; i >= 0; i--)
        {
            StatusEffects[i].Apply();
        }
    }

    public void RemoveAllStatusEffects()
    {
        for (int i = StatusEffects.Count - 1; i >= 0; i--)
        {
            RemoveStatusEffect(StatusEffects[i]);
        }
    }

    public void RemoveExpiredStatusEffects()
    {
        for (int i = StatusEffects.Count - 1; i >= 0; i--)
        {
            if (StatusEffects[i].RemainingTurns <= 0)
            {
                RemoveStatusEffect(StatusEffects[i]);
            }
        }
    }

    void RemoveStatusEffect(StatusEffect statusEffect)
    {
        statusEffect.Remove();
        StatusEffects.Remove(statusEffect);

        OnUpdated?.Invoke();
    }
}
