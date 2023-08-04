using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum BattleOutcome { Victory, Defeat, Surrender }

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    BattleOverUIHandler battleOverUIHandler;
    UnitOrderUIHandler unitOrderUIHandler;

    public BattleDataSO CurrentBattleDataSO { get; private set; }

    public Unit CurrentUnit { get; private set; }
    public List<Unit> OverallUnitOrder { get; private set; }
    public List<Unit> RoundUnitOrder { get; private set; }
    int currentUnitID;
    Dictionary<UnitStack, Unit> unitDictionary; //Used for checking casualties at the end of the battle

    List<GridEffect> activeGridEffects;

    int enemyUnits;
    int playerUnits;
    
    bool isBattleOver;
    public int CurrentRound { get; private set; }
    public int CurrentTurn { get; private set; }

    public bool IsAutoBattle { get; private set; }

    [SerializeField] AudioClip victoryAudio;
    [SerializeField] AudioClip lossAudio;
    [SerializeField] AudioClip turnDelayAudio;

    [SerializeField] float turnStartDelay = 1.0f;
    [SerializeField] float battleOverDelay = 2.5f;

    public event Action OnBattleStarted;
    public event Action OnBattleOver;
    public event Action OnTurnStarted;
    public event Action OnAutoBattleToggled;

    Player player;
    PlayerController playerController;
    Controller enemyController;
    public bool IsPlayerTurn { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        battleOverUIHandler = FindObjectOfType<BattleOverUIHandler>();
        unitOrderUIHandler = FindObjectOfType<UnitOrderUIHandler>();
    }

    void Start()
    {
        StartBattle();
    }

    public void StartBattle()
    {
        player = FindObjectOfType<Player>();    
        playerController = FindObjectOfType<PlayerController>();
        enemyController = FindObjectOfType<EnemyController>();

        CurrentBattleDataSO = DataHandler.Instance.SelectedBattleDataSO;

        isBattleOver = false;
        currentUnitID = -1;
        CurrentTurn = 0;
        CurrentRound = 0;

        activeGridEffects = new List<GridEffect>();

        SetupTeams();
        OnBattleStarted?.Invoke();
        StartRound();
        unitOrderUIHandler.InitUnitPortraits();

        StartCoroutine(StartTurn());
    }

    public void Surrender()
    {
        if (!isBattleOver)
        {
            if (IsPlayerTurn)
            {
                playerController.AbortTurn();
            }
            else
            {
                enemyController.AbortTurn();
            }
            EndBattle(BattleOutcome.Surrender);
        }
    }

    public void ToggleAutoBattle()
    {
        IsAutoBattle = !IsAutoBattle;
        if (IsPlayerTurn)
        {
            playerController.ToggleAutoBattle();
        }
        OnAutoBattleToggled?.Invoke();
    }

    public void DelayTurn()
    {
        if (turnDelayAudio != null)
        {
            AudioManager.Instance.PlaySFX(turnDelayAudio);
        }

        BattleLogger.Instance.DisplayMessage(new BattleLogMessage(string.Format("{0} {1}", CurrentUnit.DataSO.Title.Bold(), " have delayed their turn.")));

        RoundUnitOrder = RoundUnitOrder.OrderBy(unit => unit.ActionHandler.WasTurnDelayed ? unit.StatHandler.Initiative : -unit.StatHandler.Initiative).ToList();

        currentUnitID--;
        CompleteTurn();
    }

    public void CompleteTurn()
    {
        StartCoroutine(StartTurn());
    }

    public void AddGridEffect(GridEffect gridEffect)
    {
        activeGridEffects.Add(gridEffect);
        gridEffect.TurnOrderID = currentUnitID;
    }

    void SetupTeams()
    {
        unitDictionary = new Dictionary<UnitStack, Unit>();

        var units = new List<Unit>();

        foreach (var formation in player.UnitFormations)
        {
            var unitNode = Grid.Instance.GetNode(formation.SpawnX, formation.SpawnZ);
            var unit = Instantiate(formation.UnitStack.UnitSO.Prefab, unitNode.transform.position, Quaternion.Euler(new Vector3(0, 90, 0)));
            unitNode.Unit = unit;
            unit.Node = unitNode;
            units.Add(unit);
            unit.Setup(formation.UnitStack.Amount, Alliance.Ally);
            unitDictionary.Add(formation.UnitStack, unit);
        }

        foreach (var formation in CurrentBattleDataSO.UnitFormations)
        {
            var unitNode = Grid.Instance.GetNode(Grid.Instance.Width - formation.SpawnX - 1, formation.SpawnZ);
            var unit = Instantiate(formation.UnitStack.UnitSO.Prefab, unitNode.transform.position, Quaternion.Euler(new Vector3(0, 270, 0)));
            unitNode.Unit = unit;
            unit.Node = unitNode;
            units.Add(unit);
            unit.Setup(formation.UnitStack.Amount, Alliance.Enemy);
            unitDictionary.Add(formation.UnitStack, unit);
        }

        foreach (var unit in units)
        {
            unit.HealthHandler.OnDied += OnUnitDied;
            if (unit.Alliance == Alliance.Enemy)
            {
                enemyUnits++;
            }
            else
            {
                playerUnits++;
            }
        }

        SetAttackOrder(units);
    }

    void SetAttackOrder(List<Unit> units)
    {
        var sortedUnits = units.OrderByDescending(unit => unit.StatHandler.Initiative);
        OverallUnitOrder = new List<Unit>(sortedUnits.Count());
        foreach (var unit in sortedUnits)
        {
            OverallUnitOrder.Add(unit);
        }
    }

    void SetReachableNodes()
    {
        Grid.Instance.SetReachableNodes(CurrentUnit.Node, CurrentUnit.StatHandler.Stats[StatType.Speed].Value);
    }

    void StartRound()
    {
        CurrentRound++;
        BattleLogger.Instance.DisplayMessage(new BattleLogMessage($"Round {CurrentRound} starts."));

        RoundUnitOrder = new List<Unit>(OverallUnitOrder);

        foreach (var unit in RoundUnitOrder)
        {
            unit.AdvanceRound();
        }
    }

    IEnumerator StartTurn()
    {
        yield return new WaitForSeconds(turnStartDelay);

        if (CurrentUnit != null)
        {
            CurrentUnit.ToggleTurn(false);
            CurrentUnit.StatHandler.Stats[StatType.Speed].OnChanged -= SetReachableNodes;
        }

        if (currentUnitID >= RoundUnitOrder.Count - 1)
        {
            currentUnitID = 0;
            StartRound();
        }
        else
        {
            currentUnitID++;
        }

        CurrentTurn++;
        CurrentUnit = RoundUnitOrder[currentUnitID];

        yield return unitOrderUIHandler.UpdateTurnOrder(currentUnitID);

        CurrentUnit.StatusEffectHandler.ApplyStatusEffects();
        CurrentUnit.StatusEffectHandler.RemoveExpiredStatusEffects();

        BattleLogger.Instance.DisplayMessage(
            new BattleLogMessage(string.Format("{0} {1}", $"Turn {CurrentTurn}:".Bold().Colour(CurrentUnit.Alliance == Alliance.Ally
            ? BattleLogger.Instance.Colours[LogColour.Ally]
            : BattleLogger.Instance.Colours[LogColour.Enemy]),
            CurrentUnit.DataSO.Title.Bold())));

        ApplyGridEffects();

        if (!CurrentUnit.StatusEffectHandler.IsIncapacitated && !isBattleOver)
        {
            CurrentUnit.ToggleTurn(true);
            CurrentUnit.StatHandler.Stats[StatType.Speed].OnChanged += SetReachableNodes;

            SetReachableNodes();

            IsPlayerTurn = (CurrentUnit.Alliance == Alliance.Ally && !CurrentUnit.StatusEffectHandler.IsCharmed) 
                || (CurrentUnit.Alliance == Alliance.Enemy && CurrentUnit.StatusEffectHandler.IsCharmed);
            if (IsPlayerTurn)
            {
                playerController.StartTurn(CurrentUnit);
            }
            else
            {
                enemyController.StartTurn(CurrentUnit);
            }

            OnTurnStarted?.Invoke();
        }
        else
        {
            CompleteTurn();
        }
    }

    void ApplyGridEffects()
    {
        foreach (var gridEffect in activeGridEffects)
        {
            if (gridEffect.TurnOrderID == currentUnitID)
            {
                gridEffect.Apply();
            }
        }

        for (int i = activeGridEffects.Count - 1; i >= 0; i--)
        {
            if (activeGridEffects[i].RemainingTurns <= 0)
            {
                var effect = activeGridEffects[i];
                effect.Remove();
                activeGridEffects.Remove(effect);
                Destroy(effect.gameObject);
            }
        }
    }

    void OnUnitDied(Unit deadUnit)
    {
        deadUnit.HealthHandler.OnDied -= OnUnitDied;
        int deadUnitID = OverallUnitOrder.FindIndex(unit => unit == deadUnit);

        if (deadUnitID <= currentUnitID)
        {
            currentUnitID--;
        }

        foreach (var gridEffect in activeGridEffects)
        {
            if (deadUnitID <= gridEffect.TurnOrderID)
            {
                gridEffect.TurnOrderID = Mathf.Max(0, gridEffect.TurnOrderID - 1);
            }
        }

        OverallUnitOrder.Remove(deadUnit);
        RoundUnitOrder.Remove(deadUnit);

        if (deadUnit.Alliance == Alliance.Enemy)
        {
            enemyUnits--;
            if (enemyUnits == 0)
            {
                EndBattle(BattleOutcome.Victory);
            }
        }
        else
        {
            playerUnits--;
            if (playerUnits == 0)
            {
                EndBattle(BattleOutcome.Defeat);
            }
        }

        if (!isBattleOver)
        {
            unitOrderUIHandler.RemoveUnitPortrait(currentUnitID);
        }
    }

    void EndBattle(BattleOutcome battleOutcome)
    {
        StopAllCoroutines();
        StartCoroutine(HandleBattleEnd(battleOutcome)); 
    }

    IEnumerator HandleBattleEnd(BattleOutcome battleOutcome)
    {
        isBattleOver = true;
        yield return new WaitForSeconds(battleOverDelay);

        AudioManager.Instance.PlayMusic(battleOutcome == BattleOutcome.Victory ? victoryAudio : lossAudio, false);

        var deadPlayerUnitStacks = new List<UnitStack>();
        var deadEnemyUnitStacks = new List<UnitStack>();
        foreach (var entry in unitDictionary)
        {
            int deadUnits = entry.Key.Amount - entry.Value.HealthHandler.UnitAmount;
            if (deadUnits > 0)
            {
                if (entry.Value.Alliance == Alliance.Ally)
                {
                    deadPlayerUnitStacks.Add(new UnitStack(entry.Key.UnitSO, deadUnits));
                }
                else
                {
                    deadEnemyUnitStacks.Add(new UnitStack(entry.Key.UnitSO, deadUnits));
                }
            }
        }

        StartCoroutine(battleOverUIHandler.ShowBattleResults(battleOutcome, deadPlayerUnitStacks, deadEnemyUnitStacks));

        if (battleOutcome == BattleOutcome.Victory)
        {
            player.ClaimBattleRewards(CurrentBattleDataSO);

            SaveHandler.Instance.Save();
        }

        OnBattleOver?.Invoke();
    }
}
