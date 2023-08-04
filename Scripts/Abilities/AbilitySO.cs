using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum TargetType { None, Unit, Any, EmptyNode }
public enum CastingTime { Standard, Swift }
public enum AimArea { SingleTile, Square }
public enum AbilityType { Buff, Heal, Offensive, Control, GridEffect }

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities")]
public class AbilitySO : ScriptableObject
{
    [field: SerializeField, ResizableTextArea] public string Title { get; private set; }
    [field: SerializeField, ResizableTextArea] public string Description { get; private set; }
    [field: SerializeField, ShowAssetPreview] public Sprite Icon { get; private set; }

    [field: SerializeField] public BehaviourSO[] Behaviours { get; private set; }
    [field: SerializeField] public bool IsTouchRange { get; private set; }
    [field: SerializeField, HideIf("TargetType", TargetType.None)] public AimArea AimArea { get; private set; }
    [field: SerializeField] public TargetType AimTargetType { get; private set; }
    [field: SerializeField, ShowIf("AimTargetType", TargetType.Unit)] public Alliance AimTargetAlliance { get; private set; }
    [field: SerializeField, Range(2, 7), HideIf("AimArea", AimArea.SingleTile)] public int AffectedAreaSize { get; private set; }
    [SerializeField] bool isAffectedNodeSameAsAimNode;
    [field: SerializeField, HideIf("AimArea", AimArea.SingleTile), Tooltip("For things like grid effects, only the target node should be affected, as the rest will be handled by the grid effect itself")] public bool AffectTargetNodeOnly { get; private set; }
    [field: SerializeField, HideIf("isAffectedNodeSameAsAimNode"), Tooltip("Some abilities may be cast on any tile, but should only affect specific units")] public TargetType TargetType { get; private set; }
    [field: SerializeField, HideIf("isAffectedNodeSameAsAimNode"), ShowIf("TargetType", TargetType.Unit)] public Alliance TargetAlliance { get; private set; }
    [field: SerializeField] public CastingTime CastingTime { get; private set; }
    [field: SerializeField] public bool IsInfiniteUse { get; private set; }
    [field: SerializeField, HideIf("IsInfiniteUse"), Min(1)] public int Uses { get; private set; }
    [field: SerializeField] public bool UseSaveThrow { get; private set; }
    [field: SerializeField, Range(1, 20), ShowIf("UseSaveThrow")] public int DC { get; private set; }
    [field: SerializeField, ShowIf("UseSaveThrow")] public SavingThrowType SavingThrowType { get; private set; }

    [field: SerializeField] public bool ExecuteOnAnimation { get; private set; }
    [field: SerializeField] public AbilityParticle Particle { get; private set; }
    [field: SerializeField] public bool SpawnParticleOnAnimation { get; private set; }
    [field: SerializeField] public bool SpawnParticleOnTarget { get; private set; }
    [field: SerializeField] public SpawnPointType ParticleSpawnPointType { get; private set; }
    [field: SerializeField] public AudioClip SFX { get; private set; }
    [field: SerializeField] public string Animation { get; private set; }
    [field: SerializeField, ShowAssetPreview, HideIf("TargetType", TargetType.None)] public Texture2D CursorIcon { get; private set; }

    [field: Header("AI Data")]
    [field: SerializeField] public ConsiderationSO[] Considerations { get; private set; }
    [field: SerializeField] public AbilityType AbilityType { get; private set; }

    void OnValidate()
    {
        if (isAffectedNodeSameAsAimNode)
        {
            TargetType = AimTargetType;
            TargetAlliance = AimTargetAlliance;
        } 

        if (AffectTargetNodeOnly)
        {
            isAffectedNodeSameAsAimNode = true;
        }

        if (AimArea == AimArea.SingleTile)
        {
            AffectTargetNodeOnly = true;
        }
    }

    public void Use(Unit executingUnit, GridNode targetNode)
    {
        foreach (var abilityBehaviour in Behaviours)
        {
            abilityBehaviour.Use(executingUnit, targetNode);
        }
    }
}
