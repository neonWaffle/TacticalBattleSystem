using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionHandler : MonoBehaviour
{
    Unit unit;

    Queue<BaseAction> actions = new Queue<BaseAction>();
    BaseAction currentAction;

    bool isExecutingAction;

    public bool WasTurnDelayed { get; set; }
    public bool WasStandardActionUsed { get; private set; }
    public bool WasSwiftActionUsed { get; private set; }

    public event Action OnExecutionStarted;
    public event Action OnActionCompleted;

    void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public void ToggleTurn(bool isCurrentTurn)
    {
        if (isCurrentTurn)
        {
            unit.AbilityHandler.SelectDefaultAttack();
        }

        if ((unit.Alliance == Alliance.Ally && !unit.StatusEffectHandler.IsCharmed)
            || (unit.Alliance == Alliance.Enemy && unit.StatusEffectHandler.IsCharmed))
        {
            unit.Node.ToggleUnitVisuals(isCurrentTurn);
        }
    }

    public void ExecuteActions(params BaseAction[] actions)
    {
        OnExecutionStarted?.Invoke();

        foreach (var action in actions)
        {
            WasStandardActionUsed = !action.IsSwiftAction;
            WasSwiftActionUsed = action.IsSwiftAction;
            this.actions.Enqueue(action);
        }

        if (!isExecutingAction)
        {
            HandleActions();
        }
    }

    void HandleActions()
    {
        isExecutingAction = true;

        if (currentAction != null)
        {
            currentAction.OnCompleted -= HandleActions;
        }

        if (actions.Count > 0)
        {
            currentAction = actions.Dequeue();
            currentAction.OnCompleted += HandleActions;
            currentAction.Execute();
        }
        else
        {
            isExecutingAction = false;
            if (WasStandardActionUsed)
            {
                BattleManager.Instance.CompleteTurn();
            }
        }
    }

    public void AdvanceRound()
    {
        WasStandardActionUsed = false;
        WasTurnDelayed = false;
        isExecutingAction = false;
    }

    //Animation event
    public void OnCompleteAction()
    {
        OnActionCompleted?.Invoke();
    }
}
