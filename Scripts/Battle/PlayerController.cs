using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System;

[RequireComponent(typeof(UtilityAI))]
public class PlayerController : Controller
{
    new Camera camera;
    BattlePreviewHandler battlePreviewHandler;

    GridNode targetNode;
    List<GridNode> currentPath = new List<GridNode>();

    Coroutine handleTurnCoroutine;
    Coroutine meleeDestinationCoroutine;

    UtilityAI utilityAI;
    
    bool wasActionSelected;

    public event Action OnTurnFinished;

    void Awake()
    {
        battlePreviewHandler = FindObjectOfType<BattlePreviewHandler>();
        camera = Camera.main;

        utilityAI = GetComponent<UtilityAI>();
    }

    void OnDestroy()
    {
        if (Grid.Instance != null)
        {
            Grid.Instance.OnReachabilityUpdated -= battlePreviewHandler.ShowReachableNodes;
        }
    }

    public override void StartTurn(Unit currentUnit)
    {
        base.StartTurn(currentUnit);
        wasActionSelected = false;

        if (BattleManager.Instance.IsAutoBattle)
        {
            EnableAutoBattle();
        }
        else
        {
            DisableAutoBattle();
        }
    }

    public override void DelayTurn()
    {
        base.DelayTurn();
        FinishTurn();
    }

    public void ToggleAutoBattle()
    {
        if (wasActionSelected)
            return;

        if (handleTurnCoroutine != null)
        {
            StopCoroutine(handleTurnCoroutine);
        }

        if (meleeDestinationCoroutine != null)
        {
            StopCoroutine(meleeDestinationCoroutine);
        }

        if (BattleManager.Instance.IsAutoBattle)
        {
            EnableAutoBattle();
        }
        else
        {
            DisableAutoBattle();
        }
    }

    void EnableAutoBattle()
    {
        foreach (var node in Grid.Instance.Nodes)
        {
            node.OnMouseEntered -= PreviewAction;
        }

        battlePreviewHandler.HideVisuals();
        Grid.Instance.OnReachabilityUpdated -= battlePreviewHandler.ShowReachableNodes;

        SelectAutoBattleAction();
    }

    void DisableAutoBattle()
    {
        foreach (var node in Grid.Instance.Nodes)
        {
            node.OnMouseEntered += PreviewAction;
        }

        battlePreviewHandler.ShowReachableNodes();
        Grid.Instance.OnReachabilityUpdated += battlePreviewHandler.ShowReachableNodes;

        handleTurnCoroutine = StartCoroutine(HandleTurn());
    }

    void SelectAutoBattleAction()
    {
        var actions = utilityAI.SelectAction(currentUnit);
        ExecuteActions(actions);
    }

    void SelectAction(GridNode targetNode, List<GridNode> path = null)
    {
        if (currentUnit.AbilityHandler.SelectedAbility.CanBeCastOnTarget(targetNode))
        {
            if (currentUnit.AbilityHandler.SelectedAbility.AbilitySO.IsTouchRange && path != null && path.Count > 0)
            {
                ExecuteActions(new MoveAction(currentUnit, path), new AbilityAction(currentUnit.AbilityHandler.SelectedAbility, currentUnit, targetNode));
                battlePreviewHandler.HideAbilityVisuals();
            }
            else
            {
                ExecuteActions(new AbilityAction(currentUnit.AbilityHandler.SelectedAbility, currentUnit, targetNode));
                battlePreviewHandler.HideAbilityVisuals();
            }
        }
        else if (currentUnit.AbilityHandler.IsDefaultAttackSelected && targetNode.Unit == null && targetNode.IsWalkable && path != null && path.Count > 0)
        {
            ExecuteActions(new MoveAction(currentUnit, path));
            battlePreviewHandler.HideAbilityVisuals();
        }
    }

    IEnumerator HandleTurn()
    {
        while (!wasActionSelected)
        {
            if (currentUnit.AbilityHandler.SelectedAbility.AbilitySO.AimTargetType == TargetType.None)
            {
                ExecuteActions(new AbilityAction(currentUnit.AbilityHandler.SelectedAbility, currentUnit, currentUnit.Node));
            }

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && targetNode != null)
            {
                SelectAction(targetNode, currentPath);
            }

            yield return null;
        }
    }

    IEnumerator UpdateMeleeTargetDestination()
    {
        while (true)
        {
            var mousePos = camera.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x,
                Input.mousePosition.y,
                Vector3.Dot(camera.transform.forward, targetNode.transform.position - camera.transform.position)));

            currentPath = Pathfinder.Instance.FindPath(currentUnit.Node.transform.position,
                FindClosestTouchRangeNode(targetNode, mousePos).transform.position);

            if (currentPath != null && currentPath.Count > 0)
            {
                battlePreviewHandler.ShowPathVisuals(currentPath, currentUnit.Node);
            }
            else
            {
                battlePreviewHandler.HidePathVisuals();
            }

            yield return null;
        }
    }

    void PreviewAction(GridNode targetNode)
    {
        this.targetNode = targetNode;

        currentPath = null;
        if (meleeDestinationCoroutine != null)
        {
            StopCoroutine(meleeDestinationCoroutine);
        }

        if (targetNode.Unit == null && currentUnit.AbilityHandler.IsDefaultAttackSelected)
        {
            currentPath = Pathfinder.Instance.FindPath(currentUnit.Node.transform.position, targetNode.transform.position);
            battlePreviewHandler.HideAbilityVisuals();
        }
        else if (currentUnit.AbilityHandler.SelectedAbility.CanBeCastOnTarget(targetNode))
        {
            if (currentUnit.AbilityHandler.SelectedAbility.AbilitySO.IsTouchRange)
            {
                if (meleeDestinationCoroutine != null)
                {
                    StopCoroutine(meleeDestinationCoroutine);
                }
                meleeDestinationCoroutine = StartCoroutine(UpdateMeleeTargetDestination());
            }
            battlePreviewHandler.ShowAbilityVisuals(currentUnit, targetNode);
        }
        else
        {
            battlePreviewHandler.HideAbilityVisuals();
        }

        if (currentPath != null && currentPath.Count > 0)
        {
            battlePreviewHandler.ShowPathVisuals(currentPath, currentUnit.Node);
        }
        else
        {
            battlePreviewHandler.HidePathVisuals();
        }
    }

    GridNode FindClosestTouchRangeNode(GridNode targetNode, Vector3 mouseWorldPos)
    {
        var nodes = Grid.Instance.GetNodesInRange(targetNode, 1).Where(node => (node.IsReachable && node.Unit == null) || node.Unit == currentUnit).ToList();

        GridNode closestNode = null;
        float closestDot = float.MinValue;
        var dir = (mouseWorldPos - targetNode.transform.position).normalized;
        foreach (var node in nodes)
        {
            float dot = Vector3.Dot((node.transform.position - targetNode.transform.position).normalized, dir);
            if (dot > closestDot)
            {
                closestNode = node;
                closestDot = dot;
            }
        }

        return closestNode;
    }

    protected override void ExecuteActions(params BaseAction[] actions)
    {
        base.ExecuteActions(actions);

        if (actions.Where(action => !action.IsSwiftAction).FirstOrDefault() != null)
        {
            FinishTurn();
        }
        else if (BattleManager.Instance.IsAutoBattle)
        {
            SelectAutoBattleAction();
        }
    }

    void FinishTurn()
    {
        foreach (var node in Grid.Instance.Nodes)
        {
            node.OnMouseEntered -= PreviewAction;
        }

        if (meleeDestinationCoroutine != null)
        {
            StopCoroutine(meleeDestinationCoroutine);
        }

        wasActionSelected = true;
        battlePreviewHandler.HideVisuals();

        Grid.Instance.OnReachabilityUpdated -= battlePreviewHandler.ShowReachableNodes;

        OnTurnFinished?.Invoke();
    }

    public override void AbortTurn()
    {
        FinishTurn();
    }
}
