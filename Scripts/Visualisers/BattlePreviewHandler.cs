using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class BattlePreviewHandler : MonoBehaviour
{
    PathVisualiser pathVisualiser;
    new Camera camera;

    List<GridNode> nodesInAbilityAimArea = new List<GridNode>();

    [SerializeField] CanvasGroup predictionPanel;
    [SerializeField] TextMeshProUGUI healthAmountText;
    [SerializeField] TextMeshProUGUI unitAmountText;
    [SerializeField] TextMeshProUGUI unitTitleText;
    [SerializeField] Vector3 predictionPanelOffset;

    [SerializeField] Color damageColour = Color.red;
    [SerializeField] Color healColour = Color.green;

    void Awake()
    {
        pathVisualiser = GetComponentInChildren<PathVisualiser>();
        camera = Camera.main;
    }

    void Start()
    {
        HidePathVisuals();
        HideAbilityVisuals();
    }

    public void ShowPathVisuals(List<GridNode> path, GridNode startNode)
    {
        pathVisualiser.ShowPath(path, startNode);
    }

    public void HidePathVisuals()
    {
        pathVisualiser.DisableVisuals();
    }

    public void ShowReachableNodes()
    {
        foreach (var node in Grid.Instance.Nodes)
        {
            node.ToggleReachabilityVisuals();
        }
    }

    public void HideNodeVisuals()
    {
        pathVisualiser.DisableVisuals();

        foreach (var node in Grid.Instance.Nodes)
        {
            node.DisableVisuals();
        }
    }

    public void HideAbilityVisuals()
    {
        CursorManager.Instance.RevertToDefaultIcon();
        HidePredictedHealthEffects();
        HideAbilityRangeVisuals();
    }

    public void HideVisuals()
    {
        HidePredictedHealthEffects();
        HidePathVisuals();
        HideNodeVisuals();
    }

    public void ShowAbilityVisuals(Unit executingUnit, GridNode targetNode)
    {
        CursorManager.Instance.ChangeIcon(BattleManager.Instance.CurrentUnit.AbilityHandler.SelectedAbility.AbilitySO.CursorIcon);
        ShowAbilityRange(executingUnit.AbilityHandler.SelectedAbility, targetNode);
        var healthModifyingBehaviour = executingUnit.AbilityHandler.SelectedAbility.AbilitySO.Behaviours.Where(behaviour => behaviour is HealthModifyingBehaviourSO).FirstOrDefault() as HealthModifyingBehaviourSO;
        if (healthModifyingBehaviour != null)
        {
            ShowPredictedHealthEffects(executingUnit, targetNode, healthModifyingBehaviour);
        }
    }

    void HidePredictedHealthEffects()
    {
        predictionPanel.gameObject.SetActive(false);
    }

    void ShowPredictedHealthEffects(Unit executingUnit, GridNode targetNode, HealthModifyingBehaviourSO healthModifyingBehaviour)
    {
        unitTitleText.text = targetNode.Unit.DataSO.Title;

        int minHealthAmount = healthModifyingBehaviour.CalculateFinalHealthAmount(executingUnit, targetNode, healthModifyingBehaviour.MinBaseAmount);
        int maxHealthAmount = healthModifyingBehaviour.CalculateFinalHealthAmount(executingUnit, targetNode, healthModifyingBehaviour.MaxBaseAmount);

        int minUnitAmount = healthModifyingBehaviour.IsHealing
            ? targetNode.Unit.HealthHandler.CalculateRevivedUnitAmount(targetNode.Unit.HealthHandler.CurrentHP + minHealthAmount)
            : targetNode.Unit.HealthHandler.CalculateDeadUnitAmount(targetNode.Unit.HealthHandler.CurrentHP - minHealthAmount);

        int maxUnitAmount = healthModifyingBehaviour.IsHealing
            ? targetNode.Unit.HealthHandler.CalculateRevivedUnitAmount(targetNode.Unit.HealthHandler.CurrentHP + maxHealthAmount)
            : targetNode.Unit.HealthHandler.CalculateDeadUnitAmount(targetNode.Unit.HealthHandler.CurrentHP - maxHealthAmount);

        if (!healthModifyingBehaviour.IsHealing)
        {
            healthAmountText.color = damageColour;
            unitAmountText.color = damageColour;
        }
        else
        {
            healthAmountText.color = healColour;
            unitAmountText.color = healColour;
        }

        healthAmountText.text = minHealthAmount != maxHealthAmount ? $"{minHealthAmount} - {maxHealthAmount}" : minHealthAmount.ToString();
        unitAmountText.text = minUnitAmount != maxUnitAmount ? $"{minUnitAmount} - {maxUnitAmount}" : minUnitAmount.ToString();

        predictionPanel.transform.position = camera.WorldToScreenPoint(targetNode.Unit.transform.position) + predictionPanelOffset;
        predictionPanel.gameObject.SetActive(true);
    }

    void ShowAbilityRange(Ability ability, GridNode startNode)
    {
        HideAbilityRangeVisuals();
        nodesInAbilityAimArea = ability.GetNodesInAimArea(startNode);
        foreach (var node in nodesInAbilityAimArea)
        {
            node.ToggleAbilityTargetVisuals(true);
        }
    }

    void HideAbilityRangeVisuals()
    {
        if (nodesInAbilityAimArea == null)
            return;

        foreach (var node in nodesInAbilityAimArea)
        {
            node.ToggleAbilityTargetVisuals(false);
        }
        nodesInAbilityAimArea = null;
    }
}
