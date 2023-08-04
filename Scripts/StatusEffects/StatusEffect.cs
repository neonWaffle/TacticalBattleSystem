using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect
{
    public StatusEffectSO StatusEffectSO { get; private set; }

    Unit executingUnit;
    Unit targetUnit;

    public int RemainingTurns { get; private set; }
    bool wasApplied;

    GameObject vfx;

    public StatusEffect(StatusEffectSO statusEffectSO, Unit executingUnit, Unit targetUnit)
    {
        StatusEffectSO = statusEffectSO;

        this.executingUnit = executingUnit;
        this.targetUnit = targetUnit;

        RemainingTurns = statusEffectSO.Duration;
        wasApplied = false;

        if (statusEffectSO.VFX != null)
        {
            vfx = Object.Instantiate(statusEffectSO.VFX, targetUnit.GetSpawnPoint(statusEffectSO.SpawnPointType));
            vfx.transform.localScale = targetUnit.VFXScale * Vector3.one;
        }
    }

    public void Apply()
    {
        if (StatusEffectSO.DurationType == EffectDurationType.Expiring && wasApplied)
        {
            RemainingTurns--;
            if (RemainingTurns <= 0)
            {
                return;
            }
        }

        if (StatusEffectSO.VFX != null && StatusEffectSO.RespawnVFXEachTurn)
        {
            if (vfx != null)
            {
                Object.Destroy(vfx);
            }
            vfx = Object.Instantiate(StatusEffectSO.VFX, targetUnit.GetSpawnPoint(StatusEffectSO.SpawnPointType));
            vfx.transform.localScale = targetUnit.VFXScale * Vector3.one;
        }

        if (!wasApplied || StatusEffectSO.ReapplyEachTurn)
        {
            wasApplied = true;
            StatusEffectSO.Apply(executingUnit, targetUnit);
        }
    }

    public void Remove()
    {
        if (vfx != null)
        {
            Object.Destroy(vfx);
        }
        StatusEffectSO.Remove(executingUnit, targetUnit);
    }
}
