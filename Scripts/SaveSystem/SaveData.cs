using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<UnitSaveData> UnitReserveData = new List<UnitSaveData>();
    public List<UnitFormationSaveData> UnitFormationData = new List<UnitFormationSaveData>();
    public List<string> UnlockedBattles = new List<string>();
    public int Money;
    public int Level;
    public int XP;
    public int RosterSize;
}

[System.Serializable]
public class UnitFormationSaveData
{
    public UnitSaveData UnitData;
    public int X;
    public int Z;
}

[System.Serializable]
public class UnitSaveData
{
    public string UnitID;
    public int Amount;
}
