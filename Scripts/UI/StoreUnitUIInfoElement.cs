using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreUnitUIInfoElement : UIInfoElement
{
    public UnitDataSO UnitDataSO { get; set; }

    protected override void Awake()
    {
        base.Awake();
        infoDisplayHandler = transform.root.GetComponentInChildren<StoreUnitInfoDisplayHandler>();
    }
}
