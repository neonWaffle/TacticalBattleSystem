using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoreUnitInfoDisplayHandler : InfoDisplayHandler
{
    [SerializeField] TextMeshProUGUI unitTitleText;

    [Header("Stats")]
    [SerializeField] TextMeshProUGUI speedText;
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

    [SerializeField] RectTransform infoPanelRect;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Display(UIInfoElement uiInfoElement)
    {
        base.Display(uiInfoElement);

        var unitUI = uiInfoElement as StoreUnitUIInfoElement;
        if (unitUI == null)
            return;

        unitTitleText.text = unitUI.UnitDataSO.Title;

        speedText.text = unitUI.UnitDataSO.Speed.ToString();
        hpText.text = unitUI.UnitDataSO.HitPoints.ToString();
        dexterityText.text = unitUI.UnitDataSO.Dexterity.ToString();
        wisdomText.text = unitUI.UnitDataSO.Wisdom.ToString();
        constitutionText.text = unitUI.UnitDataSO.Constitution.ToString();

        baseAttackTypeText.text = unitUI.UnitDataSO.DefaultAttackAbility.IsTouchRange ? "Melee:" : "Ranged:";
        var baseAttack = (HealthModifyingBehaviourSO)unitUI.UnitDataSO.DefaultAttackAbility.Behaviours[0];
        baseDamageText.text = baseAttack.BaseAmount.ToString();
        baseAttackFormulaText.text = $"{baseAttack.RollTimes}{baseAttack.DiceType} + {baseAttack.BaseAmount}";

        infoPanel.gameObject.SetActive(true);

        for (int i = 0; i < resistanceInfoUI.Length; i++)
        {
            if (i < unitUI.UnitDataSO.Resistances.Length)
            {
                resistanceInfoUI[i].Setup(unitUI.UnitDataSO.Resistances[i]);
                resistanceInfoUI[i].gameObject.SetActive(true);
            }
            else
            {
                resistanceInfoUI[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < weaknessInfoUI.Length; i++)
        {
            if (i < unitUI.UnitDataSO.Weaknesses.Length)
            {
                weaknessInfoUI[i].Setup(unitUI.UnitDataSO.Weaknesses[i]);
                weaknessInfoUI[i].gameObject.SetActive(true);
            }
            else
            {
                weaknessInfoUI[i].gameObject.SetActive(false);
            }
        }

        infoPanel.transform.position = uiInfoElement.RectTransform.position
            + new Vector3(infoPanelRect.sizeDelta.x * 0.5f + uiInfoElement.RectTransform.sizeDelta.x * 0.5f, 0.0f, 0.0f);

        infoPanel.gameObject.SetActive(true);
    }
}
