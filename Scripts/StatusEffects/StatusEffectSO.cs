using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum EffectDurationType { Expiring, Indefinite }
public enum SpawnPointType { Root, Head, Torso, LeftHand, RightHand }

public abstract class StatusEffectSO : ScriptableObject
{
    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField, ShowAssetPreview] public Sprite Icon { get; private set; }
    [field: SerializeField] public EffectDurationType DurationType { get; private set; }
    [field: SerializeField, Min(1), ShowIf("DurationType", EffectDurationType.Expiring)] public int Duration { get; private set; }
    [field: SerializeField] public bool ReapplyEachTurn { get; private set; }
    [field: SerializeField, Tooltip("Some effects, like control, should only be applied during the unit's turn")] public bool ApplyImmediately { get; private set; }

    [field:Header("VFX")]
    [field: SerializeField] public GameObject VFX { get; private set; }
    [field: SerializeField] public SpawnPointType SpawnPointType { get; private set; }
    [field: SerializeField] public bool RespawnVFXEachTurn { get; private set; }

    public abstract void Apply(Unit executingUnit, Unit targetUnit);
    
    public virtual void Remove(Unit executingUnit, Unit targetUnit) { }
}
