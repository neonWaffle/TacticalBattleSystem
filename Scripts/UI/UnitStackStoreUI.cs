using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(StoreUnitUIInfoElement))]
public class UnitStackStoreUI : UnitStackUI
{
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] CanvasGroup panel;
    Button button;

    StoreUnitUIInfoElement storeUnitUIInfoElement;
    BattleSelectionHandler battleSelectionHandler;
    Player player;

    void Awake()
    {
        button = GetComponentInChildren<Button>();
        storeUnitUIInfoElement = GetComponentInChildren<StoreUnitUIInfoElement>();
    }
    
    void Start()
    {
        battleSelectionHandler = FindObjectOfType<BattleSelectionHandler>();
        player = FindObjectOfType<Player>();

        player.OnMoneyUpdated += OnPlayerMoneyUpdate;
    }

    void OnDestroy()
    {
        if (player != null)
        {
            player.OnMoneyUpdated -= OnPlayerMoneyUpdate;
        }
    }

    public override void AssignStack(UnitStack unitStack)
    {
        base.AssignStack(unitStack);
        costText.text = unitStack.UnitSO.Cost.ToString();

        storeUnitUIInfoElement.UnitDataSO = UnitStack.UnitSO;
    }

    public void Buy()
    {
        battleSelectionHandler.SelectUnitStackToBuy(this);
    }

    public void OnPlayerMoneyUpdate(int playerMoney)
    {
        button.interactable = UnitStack.UnitSO.Cost <= playerMoney;
        panel.alpha = button.interactable ? 1.0f : 0.5f;
    }
}
