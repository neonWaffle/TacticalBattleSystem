using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class BattleSelectionHandler : MonoBehaviour
{
    [Header("Battle Selection")]
    [SerializeField] CanvasGroup battleSelectionPanel;
    [SerializeField] BattleInfoPanel battleInfoPanel;
    BattleSelectionButton[] battleButtons;

    BattleSelectionButton selectedBattleButton;

    [Header("Unit Selection")]
    [SerializeField] CanvasGroup unitSelectionPanel;
    [SerializeField] CanvasGroup unitReservePanel;
    [SerializeField] CanvasGroup unitFormationPanel;
    [SerializeField] UnitStackSelectionUI unitStackUIPrefab;
    [SerializeField] TextMeshProUGUI playerUnitFormationAmountText;
    [SerializeField] CanvasGroup unitSplitQuantityPanel;
    [SerializeField] Slider unitSplitQuantitySlider;
    [SerializeField] CanvasGroup rosterWarningPopup;

    [Header("Unit Purchase")]
    [SerializeField] CanvasGroup unitPurchaseQuantityPanel;
    [SerializeField] Slider unitPurchaseQuantitySlider;
    [SerializeField] TextMeshProUGUI playerMoneyText;
    [SerializeField] TextMeshProUGUI unitStackCostText;

    UnitStackStoreUI selectedStoreUnitStackUI;
    UnitStackUISlot selectedSplitUnitStackSlot;

    UnitStackStoreUI[] unitStacksStoreUI;
    UnitStackUISlot[] unitFormationSlots;
    UnitStackUISlot[] unitReserveSlots;

    List<UnitStackSelectionUI> unitStackUI;

    [SerializeField] UnitStack[] unitStacksForSale;

    Player player;
    Dictionary<UnitStackUISlot, UnitFormation> currentUnitFormation = new Dictionary<UnitStackUISlot, UnitFormation>();

    void Awake()
    {
        unitStacksStoreUI = GetComponentsInChildren<UnitStackStoreUI>();
        unitFormationSlots = unitFormationPanel.GetComponentsInChildren<UnitStackUISlot>();
        unitReserveSlots = unitReservePanel.GetComponentsInChildren<UnitStackUISlot>();

        battleButtons = GetComponentsInChildren<BattleSelectionButton>();

        rosterWarningPopup.gameObject.SetActive(false);
    }

    void Start()
    {
        for (int i = 0; i < unitFormationSlots.Length; i++)
        {
            unitFormationSlots[i].Setup(false, i / DataHandler.Instance.GridHeight, i % DataHandler.Instance.GridHeight);
        }

        foreach (var slot in unitReserveSlots)
        {
            slot.Setup(true);
        }

        player = FindObjectOfType<Player>();
        player.OnMoneyUpdated += UpdateMoneyText;
        UpdateMoneyText(player.Money);

        SetupUnitStackSlots();
        SetupBattleButtons();
        SetupStoreSlots();

        unitPurchaseQuantityPanel.gameObject.SetActive(false);
        unitSplitQuantityPanel.gameObject.SetActive(false);

        var lastUnlockedBattleButton = battleButtons.TakeWhile(button =>player.UnlockedBattles.Contains(button.BattleDataSO)).FirstOrDefault();
        if (lastUnlockedBattleButton != null)
        {
            lastUnlockedBattleButton.Select();
        }
        else
        {
            battleButtons.First().Select();
        }
        OpenBattleSelection();
    }

    void OnDestroy()
    {
        if (player != null)
        {
            player.OnMoneyUpdated -= UpdateMoneyText;
        }
    }

    public void OpenUnitSelection()
    {
        unitSelectionPanel.gameObject.SetActive(true);
        battleSelectionPanel.gameObject.SetActive(false);
    }

    public bool IsPlayerRosterFull()
    {
        return currentUnitFormation.Count >= player.RosterSize;
    }

    public void AddUnitFormation(UnitStackUISlot unitStackUISlot)
    {
        if (currentUnitFormation.ContainsKey(unitStackUISlot))
        {
            currentUnitFormation[unitStackUISlot] = new UnitFormation(unitStackUISlot.UnitStackUI.UnitStack, unitStackUISlot.GridX, unitStackUISlot.GridZ);
        }
        else
        {
            currentUnitFormation.Add(unitStackUISlot, new UnitFormation(unitStackUISlot.UnitStackUI.UnitStack, unitStackUISlot.GridX, unitStackUISlot.GridZ));
        }
        UpdatePlayerUnitFormationAmountText();
    }

    public void RemoveUnitFormation(UnitStackUISlot unitStackUISlot)
    {
        currentUnitFormation.Remove(unitStackUISlot);
        UpdatePlayerUnitFormationAmountText();
    }

    public void SplitUnitFormation(UnitStackUISlot unitStackUISlot)
    {
        selectedSplitUnitStackSlot = unitStackUISlot;
        unitSplitQuantitySlider.value = unitSplitQuantitySlider.minValue;
        unitSplitQuantitySlider.maxValue = unitStackUISlot.UnitStackUI.UnitStack.Amount;

        unitSplitQuantityPanel.gameObject.SetActive(true);
    }

    public void ConfirmUnitFormationSplit()
    {
        unitSplitQuantityPanel.gameObject.SetActive(false);
        int quantity = (int)unitSplitQuantitySlider.value;

        foreach (var slot in unitReserveSlots)
        {
            if (slot.UnitStackUI == null)
            {
                var stackUI = Instantiate(unitStackUIPrefab, slot.transform);
                stackUI.AssignSlot(slot);
                stackUI.AssignStack(new UnitStack(selectedSplitUnitStackSlot.UnitStackUI.UnitStack.UnitSO, quantity));
                break;
            }
            else if (slot.UnitStackUI.UnitStack.UnitSO == selectedSplitUnitStackSlot.UnitStackUI.UnitStack.UnitSO)
            {
                slot.UnitStackUI.UnitStack.UpdateAmount(slot.UnitStackUI.UnitStack.Amount + selectedSplitUnitStackSlot.UnitStackUI.UnitStack.Amount);
                break;
            }
        }

        int remainingAmount = selectedSplitUnitStackSlot.UnitStackUI.UnitStack.Amount - quantity;
        if (remainingAmount > 0)
        {
            selectedSplitUnitStackSlot.UnitStackUI.UnitStack.UpdateAmount(remainingAmount);
        }
        else
        {
            Destroy(selectedSplitUnitStackSlot.UnitStackUI.gameObject);
            selectedSplitUnitStackSlot.RemoveStackUI();
        }
        selectedSplitUnitStackSlot = null;
    }

    public void CloseUnitSelection()
    {
        if (currentUnitFormation.Count == 0)
        {
            rosterWarningPopup.gameObject.SetActive(true);
            return;
        }

        player.UnitFormations = currentUnitFormation.Values.ToList();

        var storedUnits = new List<UnitStack>();
        foreach (var slot in unitReserveSlots)
        {
            if (slot.UnitStackUI != null)
            {
                storedUnits.Add(slot.UnitStackUI.UnitStack);
            }
        }
        player.UpdateUnitReserves(storedUnits);

        SaveHandler.Instance.Save();
        OpenBattleSelection();
    }

    public void SelectBattle(BattleSelectionButton button)
    {
        if (selectedBattleButton != null)
        {
            selectedBattleButton.Deselect();
        }
        selectedBattleButton = button;
        DataHandler.Instance.SelectBattle(button.BattleDataSO);
        battleInfoPanel.AssignBattle(button.BattleDataSO);
        battleInfoPanel.gameObject.SetActive(true);
    }

    public void StartBattle()
    {
        if (player.UnitFormations != null && player.UnitFormations.Count > 0)
        {
            SceneLoader.Instance.LoadScene(selectedBattleButton.BattleDataSO.SceneTitle);
        }
        else
        {
            rosterWarningPopup.gameObject.SetActive(true);
        }
    }

    public void RefreshStore()
    {
        SetupStoreSlots();
    }

    public void SelectUnitStackToBuy(UnitStackStoreUI selectedStoreUnitStackUI)
    {
        unitPurchaseQuantitySlider.GetComponent<LimitedSlider>().SetLimit(selectedStoreUnitStackUI.UnitStack.UnitSO.Cost == 0
            ? selectedStoreUnitStackUI.UnitStack.Amount
            : player.Money / selectedStoreUnitStackUI.UnitStack.UnitSO.Cost);

        this.selectedStoreUnitStackUI = selectedStoreUnitStackUI;
        unitPurchaseQuantitySlider.value = unitPurchaseQuantitySlider.minValue;
        unitPurchaseQuantitySlider.maxValue = selectedStoreUnitStackUI.UnitStack.Amount;
        unitPurchaseQuantityPanel.gameObject.SetActive(true);
    }

    public void UpdateUnitStackCost(float sliderValue)
    {
        if (selectedStoreUnitStackUI != null)
        {
            unitStackCostText.text = (unitPurchaseQuantitySlider.value * selectedStoreUnitStackUI.UnitStack.UnitSO.Cost).ToString();
        }
    }

    public void ConfirmPurchase()
    {
        unitPurchaseQuantityPanel.gameObject.SetActive(false);
        int amount = (int)unitPurchaseQuantitySlider.value;

        var existingSlot = unitReserveSlots
            .Where(slot => slot.UnitStackUI != null && slot.UnitStackUI.UnitStack.UnitSO == selectedStoreUnitStackUI.UnitStack.UnitSO).FirstOrDefault();
        if (existingSlot != null)
        {
            existingSlot.UnitStackUI.UnitStack.UpdateAmount(existingSlot.UnitStackUI.UnitStack.Amount + amount);
        }
        else
        {
            var emptySlot = unitReserveSlots.Where(slot => slot.UnitStackUI == null).FirstOrDefault();
            var stackUI = Instantiate(unitStackUIPrefab, emptySlot.transform);
            stackUI.AssignStack(new UnitStack(selectedStoreUnitStackUI.UnitStack.UnitSO, amount));
            stackUI.AssignSlot(emptySlot);
        }

        player.RemoveMoney(selectedStoreUnitStackUI.UnitStack.UnitSO.Cost * amount);

        int remainingUnits = (int)unitPurchaseQuantitySlider.maxValue - amount;
        if (remainingUnits > 0)
        {
            selectedStoreUnitStackUI.UnitStack.UpdateAmount(remainingUnits);
        }
        else
        {
            selectedStoreUnitStackUI.RemoveStack();
        }

        selectedStoreUnitStackUI = null;
    }

    public void ReturnToMainMenu()
    {
        SceneLoader.Instance.LoadScene("MainMenu");
    }

    void UpdatePlayerUnitFormationAmountText()
    {
        playerUnitFormationAmountText.text = $"{currentUnitFormation.Count}/{player.RosterSize}";
    }

    void UpdateMoneyText(int money)
    {
        playerMoneyText.text = money.ToString();
    }

    void SetupStoreSlots()
    {
        for (int i = 0; i < unitStacksStoreUI.Length; i++)
        {
            unitStacksStoreUI[i].AssignStack(unitStacksForSale[Random.Range(0, unitStacksForSale.Length)]);
        }
    }

    void SetupUnitStackSlots()
    {
        unitStackUI = new List<UnitStackSelectionUI>(player.UnitFormations.Count + player.UnitReserves.Count);
        foreach (var formation in player.UnitFormations)
        {
            int id = formation.SpawnX * DataHandler.Instance.GridHeight + formation.SpawnZ;
            var stackUI = Instantiate(unitStackUIPrefab, unitFormationSlots[id].transform);
            stackUI.AssignStack(formation.UnitStack);
            stackUI.AssignSlot(unitFormationSlots[id]);

            unitStackUI.Add(stackUI);
        }

        for (int i = 0; i < player.UnitReserves.Count; i++)
        {
            var stackUI = Instantiate(unitStackUIPrefab, unitReserveSlots[i].transform);
            stackUI.AssignStack(player.UnitReserves[i]);
            stackUI.AssignSlot(unitReserveSlots[i]);

            unitStackUI.Add(stackUI);
        }
    }

    void SetupBattleButtons()
    {
        foreach (var button in battleButtons)
        {
            if (player.UnlockedBattles.Contains(button.BattleDataSO))
            {
                button.Unlock();
            }
        }
    }

    void OpenBattleSelection()
    {
        unitSelectionPanel.gameObject.SetActive(false);
        battleSelectionPanel.gameObject.SetActive(true);
    }
}
