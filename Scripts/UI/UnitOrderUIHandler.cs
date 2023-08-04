using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UnitOrderUIHandler : MonoBehaviour
{
    Canvas canvas;
    [SerializeReference] CanvasGroup portraitPanel;
    [SerializeField] UnitPortrait unitPortraitPrefab;
    RoundDivider[] roundDividers;
    [SerializeField] float fadeDuration = 1.0f;
    [SerializeField] float animationDuration = 1f;

    List<UnitPortrait> unitPortraits;

    void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        roundDividers = GetComponentsInChildren<RoundDivider>();

        DisableUI();
    }

    void Start()
    {
        BattleManager.Instance.OnBattleStarted += EnableUI;
        BattleManager.Instance.OnBattleOver += DisableUI;
    }

    void OnDestroy()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnBattleStarted -= EnableUI;
            BattleManager.Instance.OnBattleOver -= DisableUI;
        }
    }

    void EnableUI()
    {
        canvas.gameObject.SetActive(true);
    }

    void DisableUI()
    {
        foreach (var divider in roundDividers)
        {
            divider.gameObject.SetActive(false);
        }
        canvas.gameObject.SetActive(false);
    }

    public void InitUnitPortraits()
    {
        unitPortraits = new List<UnitPortrait>(BattleManager.Instance.RoundUnitOrder.Count * 2);

        for (int i = 0; i < BattleManager.Instance.RoundUnitOrder.Count; i++)
        {
            var portrait = Instantiate(unitPortraitPrefab, portraitPanel.transform);
            portrait.AssignUnit(BattleManager.Instance.RoundUnitOrder[i]);
            unitPortraits.Add(portrait);
        }

        for (int i = 0; i < BattleManager.Instance.RoundUnitOrder.Count; i++)
        {
            var portrait = Instantiate(unitPortraitPrefab, portraitPanel.transform);
            portrait.AssignUnit(BattleManager.Instance.RoundUnitOrder[i]);
            unitPortraits.Add(portrait);
        }

        unitPortraits[0].ToggleTurn(true);
        SetupRoundDividers(0);
    }

    public IEnumerator UpdateTurnOrder(int currentUnitId)
    {
        currentUnitId = Mathf.Max(0, currentUnitId); //In case currentUnitId is -1

        if (BattleManager.Instance.CurrentTurn > 1)
        {
            unitPortraits[0].AnimateExit(animationDuration);
            yield return new WaitForSeconds(animationDuration);
            unitPortraits[0].gameObject.SetActive(false);
            unitPortraits[0].transform.SetAsLastSibling();
        }

        AssignUnitsToPortraits(currentUnitId);
        SetupRoundDividers(currentUnitId);
    }

    public void RemoveUnitPortrait(int currentUnitId)
    {
        for (int i = 1; i <= 2; i++)
        {
            var portrait = unitPortraits[unitPortraits.Count - i];
            unitPortraits.Remove(portrait);
            Destroy(portrait.gameObject);
        }

        AssignUnitsToPortraits(currentUnitId);
        SetupRoundDividers(currentUnitId);
    }

    void AssignUnitsToPortraits(int currentUnitId)
    {
        for (int i = 0; i < BattleManager.Instance.RoundUnitOrder.Count - currentUnitId; i++)
        {
            unitPortraits[i].AssignUnit(BattleManager.Instance.RoundUnitOrder[currentUnitId + i]);
            unitPortraits[i].gameObject.SetActive(true);
            unitPortraits[i].transform.SetSiblingIndex(i);
        }

        for (int i = 0; i < BattleManager.Instance.OverallUnitOrder.Count; i++)
        {
            unitPortraits[BattleManager.Instance.RoundUnitOrder.Count - currentUnitId + i].AssignUnit(BattleManager.Instance.OverallUnitOrder[i]);
            unitPortraits[BattleManager.Instance.RoundUnitOrder.Count - currentUnitId + i].gameObject.SetActive(true);
            unitPortraits[BattleManager.Instance.RoundUnitOrder.Count - currentUnitId + i].transform.SetSiblingIndex(BattleManager.Instance.RoundUnitOrder.Count - currentUnitId + i);
        }

        for (int i = BattleManager.Instance.OverallUnitOrder.Count + BattleManager.Instance.RoundUnitOrder.Count - currentUnitId; i < unitPortraits.Count; i++)
        {
            unitPortraits[i].gameObject.SetActive(false);
        }
    }

    void SetupRoundDividers(int currentUnitId)
    {
        roundDividers[1].SetRound(BattleManager.Instance.CurrentRound + 1);
        roundDividers[1].transform.SetAsLastSibling();
        roundDividers[1].gameObject.SetActive(true);

        roundDividers[0].SetRound(BattleManager.Instance.CurrentRound);
        roundDividers[0].transform.SetSiblingIndex(BattleManager.Instance.RoundUnitOrder.Count - currentUnitId);
        roundDividers[0].gameObject.SetActive(true);
    }
}