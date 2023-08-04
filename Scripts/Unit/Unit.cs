using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Alliance { Ally, Enemy }

[RequireComponent(typeof(HealthHandler))]
[RequireComponent(typeof(NavigationHandler))]
[RequireComponent(typeof(StatusEffectHandler))]
[RequireComponent(typeof(StatHandler))]
[RequireComponent(typeof(ActionHandler))]
[RequireComponent(typeof(AbilityHandler))]
[RequireComponent(typeof(Animator))]
public class Unit : MonoBehaviour
{
    public Animator Animator { get; private set; }
    public HealthHandler HealthHandler { get; private set; }
    public StatHandler StatHandler { get; private set; }
    public NavigationHandler NavigationHandler { get; private set; }
    public ActionHandler ActionHandler { get; private set; }
    public AbilityHandler AbilityHandler { get; private set; }
    public StatusEffectHandler StatusEffectHandler { get; private set; }

    [field: SerializeField] public UnitDataSO DataSO { get; private set; }

    [field: Header("VFX Data")]
    [field: SerializeField] public float VFXScale { get; private set; } = 1.0f;
    [SerializeField] public Transform rootSpawnPoint;
    [SerializeField] public Transform torsoSpawnPoint;
    [SerializeField] public Transform headSpawnPoint;
    [SerializeField] public Transform leftHandSpawnPoint;
    [SerializeField] public Transform rightHandSpawnPoint;

    public Alliance Alliance { get; private set; }

    public GridNode Node { get; set; }

    void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        HealthHandler = GetComponent<HealthHandler>();
        NavigationHandler = GetComponent<NavigationHandler>();
        ActionHandler = GetComponent<ActionHandler>();
        AbilityHandler = GetComponent<AbilityHandler>();
        StatusEffectHandler = GetComponent<StatusEffectHandler>();
        StatHandler = GetComponent<StatHandler>();
    }

    public void Setup(int amount, Alliance alliance)
    {
        HealthHandler.Init(amount);
        Alliance = alliance;
    }

    public void ToggleTurn(bool isCurrentTurn)
    {
        ActionHandler.ToggleTurn(isCurrentTurn);
    }

    public void AdvanceRound()
    {
        ActionHandler.AdvanceRound();
        AbilityHandler.AdvanceRound();
    }

    public Transform GetSpawnPoint(SpawnPointType spawnPointType)
    {
        switch (spawnPointType)
        {
            case SpawnPointType.Root:
                return rootSpawnPoint;
            case SpawnPointType.Torso:
                return torsoSpawnPoint;
            case SpawnPointType.Head:
                return headSpawnPoint;
            case SpawnPointType.LeftHand:
                return leftHandSpawnPoint;
            case SpawnPointType.RightHand:
                return rightHandSpawnPoint;
            default:
                return null;
        }
    }
}
