using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitUIInfoElement : UIInfoElement, IPointerClickHandler
{
    public Unit Unit { get; set; }

    protected override void Awake()
    {
        base.Awake();
        infoDisplayHandler = transform.root.GetComponentInChildren<UnitInfoDisplayHandler>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            var unitInfoDisplayHandler = infoDisplayHandler as UnitInfoDisplayHandler;
            unitInfoDisplayHandler.TogglePin();        }
    }
}
