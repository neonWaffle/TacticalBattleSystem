using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour, ISaveable
{
    public List<UnitFormation> UnitFormations { get; set; }
    public int Money { get; private set; }
    public int Level { get; private set; }
    public int XP { get; private set; }
    public int RosterSize { get; private set; }
    public List<UnitStack> UnitReserves { get; private set; }
    public List<BattleDataSO> UnlockedBattles { get; private set; }

    public bool LevelledUp { get; private set; }
    [field: SerializeField] public LevelUpDataSO LevelUpSO { get; private set; }

    public event Action<int> OnMoneyUpdated;

    public void UpdateUnitReserves(List<UnitStack> unitReserves)
    {
        UnitReserves = unitReserves;
    }

    public void Load(SaveData saveData)
    {
        UnitFormations = new List<UnitFormation>(saveData.UnitFormationData.Count);
        foreach (var data in saveData.UnitFormationData)
        {
            var unitFormation = new UnitFormation(new UnitStack(SaveHandler.Instance.SaveDatabaseSO.GetUnitDataSO(data.UnitData.UnitID), data.UnitData.Amount),
                data.X, data.Z);
            UnitFormations.Add(unitFormation);
        }

        UnitReserves = new List<UnitStack>(saveData.UnitReserveData.Count);
        foreach (var data in saveData.UnitReserveData)
        {
            var unitStack = new UnitStack(SaveHandler.Instance.SaveDatabaseSO.GetUnitDataSO(data.UnitID), data.Amount);
            UnitReserves.Add(unitStack);
        }

        Money = saveData.Money;
        Level = saveData.Level;
        XP = saveData.XP;
        RosterSize = saveData.RosterSize;

        UnlockedBattles = new List<BattleDataSO>(saveData.UnlockedBattles.Count);
        foreach (var battle in saveData.UnlockedBattles)
        {
            UnlockBattle(SaveHandler.Instance.SaveDatabaseSO.GetBattleDataSO(battle));
        }

        LevelledUp = false;
    }

    public void Save(SaveData saveData)
    {
        foreach (var formation in UnitFormations)
        {
            var unitFormationData = new UnitFormationSaveData();

            var unitData = new UnitSaveData();
            unitData.UnitID = formation.UnitStack.UnitSO.ID;
            unitData.Amount = formation.UnitStack.Amount;
            unitFormationData.UnitData = unitData;

            unitFormationData.X = formation.SpawnX;
            unitFormationData.Z = formation.SpawnZ;

            saveData.UnitFormationData.Add(unitFormationData);
        }

        foreach (var unit in UnitReserves)
        {
            var unitData = new UnitSaveData();
            unitData.UnitID = unit.UnitSO.ID;
            unitData.Amount = unit.Amount;

            saveData.UnitReserveData.Add(unitData);
        }

        saveData.Money = Money;
        saveData.Level = Level;
        saveData.XP = XP;
        saveData.RosterSize = RosterSize;

        foreach (var battle in UnlockedBattles)
        {
            saveData.UnlockedBattles.Add(battle.ID);
        }
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        OnMoneyUpdated?.Invoke(Money);
    }

    public void RemoveMoney(int amount)
    {
        Money -= amount;
        OnMoneyUpdated?.Invoke(Money);
    }

    public void ClaimBattleRewards(BattleDataSO completedBattleSO)
    {
        if (completedBattleSO.NextBattleDataSO != null)
        {
            UnlockBattle(completedBattleSO.NextBattleDataSO);
        }
        AddMoney(completedBattleSO.Money);
        GainXP(completedBattleSO.XP);
    }

    void UnlockBattle(BattleDataSO unlockedBattleSO)
    {
        if (UnlockedBattles.Contains(unlockedBattleSO))
            return;

        UnlockedBattles.Add(unlockedBattleSO);
    }

    void GainXP(int xp)
    {
        if (XP < LevelUpSO.MaxXP)
        {
            XP += xp;
            XP = Mathf.Min(XP, LevelUpSO.MaxXP);

            while (XP >= LevelUpSO.GetRequiredXP(Level + 1))
            {
                LevelUp();
            }
        }
    }

    void LevelUp()
    {
        Level++;

        if (LevelUpSO.LevelUpRewards[Level - 1].IncreaseRosterSize)
        {
            RosterSize++;
        }

        LevelledUp = true;
    }
}
