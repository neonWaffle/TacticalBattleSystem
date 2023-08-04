using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionUIHandler : MonoBehaviour
{
    [SerializeField] CanvasGroup unitAbilityPanel;
    [SerializeField] CanvasGroup unitActionPanel;
    [SerializeField] CanvasGroup unavailableActionPanel;
    [SerializeField] CanvasGroup surrenderConfirmationPanel;

    [SerializeField] ToggleUIElement autoBattleButton;

    [SerializeField] float popupDuration = 2.5f;

    Canvas canvas;

    AbilityButton[] unitButtons;

    Coroutine unavailableActionCoroutine;

    PlayerController playerController;

    void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();

        unitButtons = unitAbilityPanel.GetComponentsInChildren<AbilityButton>();
        unavailableActionPanel.gameObject.SetActive(false);
        surrenderConfirmationPanel.gameObject.SetActive(false);
    }

    void Start()
    {
        foreach (var button in unitButtons)
        {
            button.OnSelected += ToggleButtons;
            button.ResetButton();
        }

        playerController = FindObjectOfType<PlayerController>();
        playerController.OnTurnFinished += DisablePlayerActionUI;

        BattleManager.Instance.OnTurnStarted += TogglePlayerActionUI;
        BattleManager.Instance.OnBattleOver += DisableUI;

        DisablePlayerActionUI();
    }

    void OnDestroy()
    {
        foreach (var button in unitButtons)
        {
            button.OnSelected -= ToggleButtons;
        }

        if (playerController != null)
        {
            playerController.OnTurnFinished -= DisablePlayerActionUI;
        }
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnTurnStarted -= TogglePlayerActionUI;
            BattleManager.Instance.OnAutoBattleToggled -= TogglePlayerActionUI;
            BattleManager.Instance.OnBattleOver -= DisableUI;
        }
    }

    public void DelayTurn()
    {
        if (BattleManager.Instance.CurrentUnit.ActionHandler.WasTurnDelayed)
        {
            ChooseUnavailableAction();
        }
        else
        {
            playerController.DelayTurn();
        }
    }

    public void ToggleAutoBattle()
    {
        BattleManager.Instance.ToggleAutoBattle();
        autoBattleButton.Toggle(BattleManager.Instance.IsAutoBattle);
    }

    public void Defend()
    {
        BattleManager.Instance.CurrentUnit.AbilityHandler.SelectDefendAbility();
    }

    public void ChooseUnavailableAction()
    {
        if (unavailableActionCoroutine != null)
        {
            StopCoroutine(unavailableActionCoroutine);
        }
        unavailableActionCoroutine = StartCoroutine(ShowUnavailableActionPopup());
    }

    public void Surrender()
    {
        BattleManager.Instance.Surrender();
    }

    IEnumerator ShowUnavailableActionPopup()
    {
        unavailableActionPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(popupDuration);
        unavailableActionPanel.gameObject.SetActive(false);
    }

    void ToggleButtons(AbilityButton selectedButton)
    {
        foreach (var button in unitButtons)
        {
            if (button != selectedButton)
            {
                button.Deselect();
            }
        }
    }

    void TogglePlayerActionUI()
    {
        if (BattleManager.Instance.IsAutoBattle)
        {
            DisablePlayerActionUI();
        }
        else
        {
            EnablePlayerActionUI();
        }
    }

    void DisablePlayerActionUI()
    {
        BattleManager.Instance.OnAutoBattleToggled -= TogglePlayerActionUI;

        unitActionPanel.gameObject.SetActive(false);
    }

    void EnablePlayerActionUI()
    {
        if (!BattleManager.Instance.IsPlayerTurn || BattleManager.Instance.IsAutoBattle)
            return;
        
        BattleManager.Instance.OnAutoBattleToggled += TogglePlayerActionUI;
        for (int i = 0; i < unitButtons.Length; i++)
        {
            unitButtons[i].ResetButton();
            if (i < BattleManager.Instance.CurrentUnit.AbilityHandler.Abilities.Count)
            {
                unitButtons[i].SetAbility(BattleManager.Instance.CurrentUnit.AbilityHandler.Abilities[i], BattleManager.Instance.CurrentUnit);
            }
        }
        unitActionPanel.gameObject.SetActive(true);
    }

    void DisableUI()
    {
        canvas.gameObject.SetActive(false);
    }
}
