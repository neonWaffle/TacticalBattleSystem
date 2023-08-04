using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataHandler : MonoBehaviour
{
    public static DataHandler Instance { get; private set; }

    [field: Header("Grid Data")]
    [field: SerializeField, Min(1)] public int GridHeight { get; private set; }
    [field: SerializeField, Min(1)] public int GridWidth { get; private set; }

    [field: Header("Starting Data")]
    [field: SerializeField] public BattleDataSO StartingBattleSO { get; private set; }
    [field: SerializeField, Min(0)] public int StartingMoney { get; private set; }
    [field: SerializeField, Min(1)] public int StartingRosterSize { get; private set; }
    [field: SerializeField] public List<UnitFormation> StartingUnitFormations { get; private set; }

    public BattleDataSO SelectedBattleDataSO { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SelectBattle(BattleDataSO battleDataSO)
    {
        SelectedBattleDataSO = battleDataSO;
    }

    public void InitSave(SaveData saveData)
    {
        saveData.UnlockedBattles.Add(StartingBattleSO.ID);
        saveData.Money = StartingMoney;
        saveData.RosterSize = StartingRosterSize;
        saveData.Level = 1;
        saveData.XP = 0;

        foreach (var formation in StartingUnitFormations)
        {
            var formationData = new UnitFormationSaveData();

            var unitData = new UnitSaveData();
            unitData.UnitID = formation.UnitStack.UnitSO.ID;
            unitData.Amount = formation.UnitStack.Amount;
            formationData.UnitData = unitData;

            formationData.X = formation.SpawnX;
            formationData.Z = formation.SpawnZ;

            saveData.UnitFormationData.Add(formationData);
        }
    }
}
