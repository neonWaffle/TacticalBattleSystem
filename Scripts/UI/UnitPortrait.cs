using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(UnitUIInfoElement))]
public class UnitPortrait : MonoBehaviour
{
    [SerializeField] Color playerColour = Color.blue;
    [SerializeField] Color enemyColour = Color.red;

    [SerializeField] CanvasGroup portrait;
    [SerializeField] Image frame;
    [SerializeField] Image unitImage;
    [SerializeField] Image unitAmountBackground;
    [SerializeField] TextMeshProUGUI unitAmountText;

    [SerializeField] float defaultScale = 1.0f;
    [SerializeField] float currentTurnScale = 1.25f;

    [SerializeField] CanvasGroup statusEffectPanel;
    [SerializeField] Image[] statusEffectIcons;
    [SerializeField] Image statusEffectExpansionIcon;

    [SerializeField] RectTransform rectTransform;
    [SerializeField] UnitUIInfoElement unitUIInfoElement;

    public Unit Unit { get; private set; }

    void OnDestroy()
    {
        if (Unit != null)
        {
            Unit.StatusEffectHandler.OnUpdated -= SetupStatusEffectIcons;
        }
    }

    void OnEnable()
    {
        portrait.alpha = 1.0f;
    }

    public void AssignUnit(Unit unit)
    {
        if (Unit != null)
        {
            Unit.StatusEffectHandler.OnUpdated -= SetupStatusEffectIcons;
        }

        Unit = unit;
        unit.StatusEffectHandler.OnUpdated += SetupStatusEffectIcons;

        unitUIInfoElement.Unit = unit;

        unitImage.sprite = unit.DataSO.Portrait;

        if (unit.Alliance == Alliance.Ally)
        {
            unitAmountBackground.color = playerColour;
            frame.color = playerColour;
        }
        else
        {
            unitAmountBackground.color = enemyColour;
            frame.color = enemyColour;
        }

        unitAmountText.text = unit.HealthHandler.UnitAmount.ToString();

        SetupStatusEffectIcons();
    }

    public void ToggleTurn(bool isCurrentTurn)
    {
        transform.localScale = isCurrentTurn ? Vector3.one * currentTurnScale : Vector3.one * defaultScale;
    }

    public void StartNextRound()
    {
        portrait.alpha = 1.0f;
    }

    public void AnimateExit(float animationDuration)
    {
        transform.DOMoveY(transform.position.y - rectTransform.sizeDelta.y, animationDuration);
        portrait.DOFade(0.0f, animationDuration);
    }

    void SetupStatusEffectIcons()
    {
        for (int i = 0; i < statusEffectIcons.Length; i++)
        {
            if (i >= Unit.StatusEffectHandler.StatusEffects.Count 
                || (i == statusEffectIcons.Length - 1 && Unit.StatusEffectHandler.StatusEffects.Count > statusEffectIcons.Length))
            {
                statusEffectIcons[i].gameObject.SetActive(false);
            }
            else
            {
                statusEffectIcons[i].sprite = Unit.StatusEffectHandler.StatusEffects[i].StatusEffectSO.Icon;
                statusEffectIcons[i].gameObject.SetActive(true);
            }
        }

        statusEffectExpansionIcon.gameObject.SetActive(Unit.StatusEffectHandler.StatusEffects.Count > statusEffectIcons.Length);
    }
}
