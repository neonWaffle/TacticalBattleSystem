using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(UtilityAI))]
public class EnemyController : Controller
{
    UtilityAI utilityAI;
    [SerializeField] float executionDelay = 1.0f;
    Coroutine handleTurnCoroutine;

    void Awake()
    {
        utilityAI = GetComponent<UtilityAI>();
    }

    public override void StartTurn(Unit currentUnit)
    {
        base.StartTurn(currentUnit);
        handleTurnCoroutine = StartCoroutine(HandleActionExecution());
    }

    public override void AbortTurn()
    {
        if (handleTurnCoroutine != null)
        {
            StopCoroutine(handleTurnCoroutine);
        }
    }
    
    IEnumerator HandleActionExecution()
    {
        yield return new WaitForSeconds(executionDelay);
        SelectAutoBattleAction();
    }

    void SelectAutoBattleAction()
    {
        var actions = utilityAI.SelectAction(currentUnit);
        ExecuteActions(actions);
    }

    protected override void ExecuteActions(params BaseAction[] actions)
    {
        base.ExecuteActions(actions);

        if (actions.Where(action => !action.IsSwiftAction).FirstOrDefault() == null)
        {
            SelectAutoBattleAction();
        }
    }
}
