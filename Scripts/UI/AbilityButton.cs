using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(ToggleUIElement))]
[RequireComponent(typeof(AbilityUIInfoElement))]
public class AbilityButton : MonoBehaviour
{
    [SerializeField] Image abilityIcon;
    [SerializeField] TextMeshProUGUI remainingUsesText;

    [SerializeField] Color defaultIconColour = Color.white;
    [SerializeField] Color disabledIconColour = Color.grey;

    Ability ability;
    Unit abilityOwner;

    ActionUIHandler actionUIHandler;
    AbilityUIInfoElement abilityUIInfoElement;

    Button button;
    bool isSelected;

    ToggleUIElement toggleUI;

    public event Action<AbilityButton> OnSelected;

    void Awake()
    {
        button = GetComponent<Button>();
        toggleUI = GetComponent<ToggleUIElement>();
        abilityUIInfoElement = GetComponent<AbilityUIInfoElement>();
        actionUIHandler = transform.root.GetComponent<ActionUIHandler>();

        ResetButton();
    }

    public void SetAbility(Ability ability, Unit abilityOwner)
    {
        this.ability = ability;
        ability.OnAbilityUsed += OnAbilityUse;

        this.abilityOwner = abilityOwner;
        abilityOwner.ActionHandler.OnExecutionStarted += DisableInteraction;

        abilityUIInfoElement.Ability = ability;

        abilityIcon.sprite = ability.AbilitySO.Icon;
        abilityIcon.enabled = true;

        if (!ability.AbilitySO.IsInfiniteUse)
        {
            remainingUsesText.text = ability.RemainingUses.ToString();
            remainingUsesText.gameObject.SetActive(true);
        }
        else
        {
            remainingUsesText.gameObject.SetActive(false);
        }
        ChangeStatus(ability.CanBeUsed);
    }

    public void ResetButton()
    {
        if (abilityOwner != null)
        {
            abilityOwner.ActionHandler.OnExecutionStarted -= DisableInteraction;
            abilityOwner = null;
        }

        if (ability != null)
        {
            ability.OnAbilityUsed -= OnAbilityUse;
            ability = null;
        }

        abilityIcon.enabled = false;
        remainingUsesText.gameObject.SetActive(false);
    }

    void DisableInteraction()
    {
        ChangeStatus(false);
    }

    void ChangeStatus(bool canBeUsed)
    {
        button.interactable = canBeUsed;
        abilityIcon.color = canBeUsed ? defaultIconColour : disabledIconColour;

        Deselect();
    }

    void OnAbilityUse(Ability ability)
    {
        if (!ability.AbilitySO.IsInfiniteUse)
        {
            remainingUsesText.text = ability.RemainingUses.ToString();
        }
    }

    public void Click()
    {
        if (!isSelected)
        {
            if (!ability.CanBeUsed)
            {
                actionUIHandler.ChooseUnavailableAction(); 
            }
            else
            {
                isSelected = true;
                BattleManager.Instance.CurrentUnit.AbilityHandler.SelectAbility(ability);
                toggleUI.Toggle(true);
                OnSelected?.Invoke(this);
            }
        }
        else
        {
            BattleManager.Instance.CurrentUnit.AbilityHandler.SelectDefaultAttack();
            Deselect();
        }
    }

    public void Deselect()
    {
        isSelected = false;
        toggleUI.Toggle(false);
    }
}
