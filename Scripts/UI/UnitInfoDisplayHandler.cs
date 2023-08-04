using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitInfoDisplayHandler : InfoDisplayHandler
{
    [SerializeField] TextMeshProUGUI unitTitleText;

    [Header("Stats")]
    [SerializeField] TextMeshProUGUI speedText;
    [SerializeField] TextMeshProUGUI initiativeText;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI dexterityText;
    [SerializeField] TextMeshProUGUI constitutionText;
    [SerializeField] TextMeshProUGUI wisdomText;

    [Header("Damage")]
    [SerializeField] TextMeshProUGUI baseAttackTypeText;
    [SerializeField] TextMeshProUGUI baseDamageText;
    [SerializeField] TextMeshProUGUI baseAttackFormulaText;

    [Header("Resistances/Weaknesses")]
    [SerializeField] DamageTypeInfoUI[] resistanceInfoUI;
    [SerializeField] DamageTypeInfoUI[] weaknessInfoUI;

    [Header("Status Effects")]
    [SerializeField] Transform statusEffectGrid;
    [SerializeField] StatusEffectInfoPanel statusEffectInfoPanelPrefab;
    List<StatusEffectInfoPanel> statusEffectInfoPanels = new List<StatusEffectInfoPanel>();

    [SerializeField] Button pinButton;
    bool isPinned;

    protected override void Awake()
    {
        base.Awake();
        Unpin(true);
    }

    public override void Display(UIInfoElement uiInfoElement)
    {
        base.Display(uiInfoElement);

        if (isPinned)
            return;

        var unitUI = uiInfoElement as UnitUIInfoElement;
        if (unitUI == null)
            return;

        unitTitleText.text = unitUI.Unit.DataSO.Title;

        speedText.text = unitUI.Unit.DataSO.Speed.ToString();
        initiativeText.text = unitUI.Unit.StatHandler.Initiative.ToString();
        hpText.text = unitUI.Unit.DataSO.HitPoints.ToString();
        dexterityText.text = unitUI.Unit.DataSO.Dexterity.ToString();
        wisdomText.text = unitUI.Unit.DataSO.Wisdom.ToString();
        constitutionText.text = unitUI.Unit.DataSO.Constitution.ToString();

        baseAttackTypeText.text = unitUI.Unit.DataSO.DefaultAttackAbility.IsTouchRange ? "Melee:" : "Ranged:";
        var baseAttack = (HealthModifyingBehaviourSO)unitUI.Unit.DataSO.DefaultAttackAbility.Behaviours[0];
        baseDamageText.text = baseAttack.BaseAmount.ToString();
        baseAttackFormulaText.text = $"{baseAttack.RollTimes}{baseAttack.DiceType} + {baseAttack.BaseAmount}";

        foreach (var effectPanel in statusEffectInfoPanels)
        {
            Destroy(effectPanel.gameObject);
        }

        statusEffectInfoPanels = new List<StatusEffectInfoPanel>(unitUI.Unit.StatusEffectHandler.StatusEffects.Count);
        foreach (var statusEffect in unitUI.Unit.StatusEffectHandler.StatusEffects)
        {
            var panel = Instantiate(statusEffectInfoPanelPrefab, statusEffectGrid);
            panel.Setup(statusEffect);
            statusEffectInfoPanels.Add(panel);
        }

        for (int i = 0; i < resistanceInfoUI.Length; i++)
        {
            if (i < unitUI.Unit.DataSO.Resistances.Length)
            {
                resistanceInfoUI[i].Setup(unitUI.Unit.DataSO.Resistances[i]);
                resistanceInfoUI[i].gameObject.SetActive(true);
            }
            else
            {
                resistanceInfoUI[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < weaknessInfoUI.Length; i++)
        {
            if (i < unitUI.Unit.DataSO.Weaknesses.Length)
            {
                weaknessInfoUI[i].Setup(unitUI.Unit.DataSO.Weaknesses[i]);
                weaknessInfoUI[i].gameObject.SetActive(true);
            }
            else
            {
                weaknessInfoUI[i].gameObject.SetActive(false);
            }
        }

        infoPanel.gameObject.SetActive(true);
    }

    public override void Hide()
    {
        if (isPinned)
            return;

        foreach (var panel in statusEffectInfoPanels)
        {
            Destroy(panel.gameObject);
        }
        statusEffectInfoPanels.Clear();

        base.Hide();
    }

    void Pin()
    {
        isPinned = true;
        pinButton.gameObject.SetActive(true);
    }

    public void Unpin(bool shouldHide)
    {
        isPinned = false;
        pinButton.gameObject.SetActive(false);

        if (shouldHide)
        {
            Hide();
        }
    }

    public void TogglePin()
    {
        if (!infoPanel.gameObject.activeInHierarchy)
            return;

        if (isPinned)
        {
            Unpin(false);
        }
        else
        {
            Pin();
        }
    }
}
