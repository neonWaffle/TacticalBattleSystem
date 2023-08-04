using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "SaveDatabase")]
public class SaveDatabaseSO : ScriptableObject
{

    [SerializeField] List<UnitDataSO> unitSOList;
    Dictionary<string, UnitDataSO> unitSODatabase;
    [SerializeField] List<BattleDataSO> battleSOList;
    Dictionary<string, BattleDataSO> battleSODatabase;

    public UnitDataSO GetUnitDataSO(string id)
    {
        unitSODatabase.TryGetValue(id, out var unitDataSO);
        return unitDataSO;
    }

    public BattleDataSO GetBattleDataSO(string id)
    {
        battleSODatabase.TryGetValue(id, out var battleDataSO);
        return battleDataSO;
    }

    public void Init()
    { 
        unitSODatabase = unitSOList.ToDictionary(unit => unit.ID, unit => unit);
        battleSODatabase = battleSOList.ToDictionary(unit => unit.ID, unit => unit);
    }
}
