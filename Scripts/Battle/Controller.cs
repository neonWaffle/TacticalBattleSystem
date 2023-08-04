using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    protected Unit currentUnit;

    public virtual void StartTurn(Unit currentUnit)
    {
        this.currentUnit = currentUnit;
    }

    public virtual void DelayTurn()
    {
        if (currentUnit.ActionHandler.WasTurnDelayed)
            return;

        currentUnit.ActionHandler.WasTurnDelayed = true;
        BattleManager.Instance.DelayTurn();
    }

    protected virtual void ExecuteActions(params BaseAction[] actions)
    {
        currentUnit.ActionHandler.ExecuteActions(actions);
        currentUnit.AbilityHandler.SelectDefaultAttack();
    }

    //Used in case the player surrenders
    public abstract void AbortTurn();
}
