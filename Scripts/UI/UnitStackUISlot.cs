using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitStackUISlot : MonoBehaviour
{
    public bool IsReserveSlot { get; private set; }
    public int GridX { get; private set; } //Used for non reserve slots to determine the unit stack's position on the grid
    public int GridZ { get; private set; } //Used for non reserve slots to determine the unit stack's position on the grid

    BattleSelectionHandler battleSelectionHandler;

    public UnitStackUI UnitStackUI { get; private set; }

    void Awake()
    {
        battleSelectionHandler = transform.root.GetComponentInChildren<BattleSelectionHandler>();
    }

    public void Setup(bool isReserveSlot)
    {
        IsReserveSlot = isReserveSlot;
    }

    public void Setup(bool isReserveSlot, int gridX, int gridZ)
    {
        IsReserveSlot = isReserveSlot;
        GridX = gridX;
        GridZ = gridZ;
    }

    public void RemoveStackUI()
    {   
        if (!IsReserveSlot)
        {
            battleSelectionHandler.RemoveUnitFormation(this);
        }
        UnitStackUI = null;
    }

    public void AssignStackUI(UnitStackUI unitStackUI)
    {
        UnitStackUI = unitStackUI;
        if (!IsReserveSlot)
        {
            battleSelectionHandler.AddUnitFormation(this);
        }
    }
}
