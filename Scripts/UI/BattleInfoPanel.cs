using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleInfoPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleText;
    UnitStackUI[] unitStackUIList;

    void Awake()
    {
        unitStackUIList = GetComponentsInChildren<UnitStackUI>();
    }

    public void AssignBattle(BattleDataSO battleSO)
    {
        for (int i = 0; i < unitStackUIList.Length; i++)
        {
            if (i < battleSO.UnitFormations.Count)
            {
                unitStackUIList[i].AssignStack(battleSO.UnitFormations[i].UnitStack);
                unitStackUIList[i].gameObject.SetActive(true);
            }
            else
            {
                unitStackUIList[i].gameObject.SetActive(false);
            }
        }
    }
}
