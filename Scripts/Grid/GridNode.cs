using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridNode : MonoBehaviour
{
    public int G { get; set; }
    public int H { get; set; }
    public int F => G + H;
    public GridNode Parent { get; set; }

    public int X { get; private set; }
    public int Z { get; private set; }
    [field: SerializeField] public bool IsWalkable { get; private set; }
    [field: SerializeField] public bool IsUnitSpawnPoint { get; private set; }
    public bool IsReachable { get; set; }
    public Unit Unit { get; set; }
    public List<GridEffect> GridEffects { get; set; }

    [SerializeField] GameObject nodeVisualiser;
    [SerializeField] GameObject activeUnitVisualiser;
    [SerializeField] GameObject abilityTargetVisualiser;
    [SerializeField] GameObject unitHoverVisualiser;
    [SerializeField] GameObject reachableNodeVisualiser;

    public event Action<GridNode> OnMouseEntered;

    void OnValidate()
    {
        if (IsUnitSpawnPoint && !IsWalkable)
        {
            IsWalkable = true;
        }
    }

    void Awake()
    {
        GridEffects = new List<GridEffect>();
        DisableVisuals();
    }

    void OnMouseEnter()
    {
        if (!BattleManager.Instance.IsPlayerTurn)
            return;

        if (Unit != null && Unit != BattleManager.Instance.CurrentUnit)
        {
            unitHoverVisualiser.SetActive(true);
        }

        OnMouseEntered?.Invoke(this);
    }

    void OnMouseExit()
    {
        if (!BattleManager.Instance.IsPlayerTurn)
            return;

        unitHoverVisualiser.SetActive(false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = IsUnitSpawnPoint ? Color.yellow : IsWalkable ? Color.green : Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(transform.localScale.x, 0.01f, transform.localScale.z));
    }

    public void Init(int x, int z)
    {
        X = x;
        Z = z;

        nodeVisualiser.SetActive(IsWalkable);
    }

    public void DisableVisuals()
    {
        activeUnitVisualiser.SetActive(false);
        unitHoverVisualiser.SetActive(false);
        reachableNodeVisualiser.SetActive(false);
        abilityTargetVisualiser.SetActive(false);
    }

    public void ToggleReachabilityVisuals()
    {
        reachableNodeVisualiser.SetActive(IsReachable);
    }

    public void ToggleUnitVisuals(bool isUnitsTurn)
    {
        activeUnitVisualiser.SetActive(isUnitsTurn);
    }

    public void ToggleAbilityTargetVisuals(bool isTargeted)
    {
        abilityTargetVisualiser.SetActive(isTargeted);
    }
}
