using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitUIHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI unitAmountText;
    [SerializeField] Image allianceIcon;
    [SerializeField] Color playerColour = Color.blue;
    [SerializeField] Color enemyColour = Color.red;

    Unit unit;

    [SerializeField] Vector3 offset;
    bool shouldFollowUnit;

    void Awake()
    {
        unit = GetComponentInParent<Unit>();
    }

    void Start()
    {
        transform.parent = null;
        transform.position = unit.transform.position + offset;

        allianceIcon.color = unit.Alliance == Alliance.Ally ? playerColour : enemyColour;

        unit.HealthHandler.OnUnitAmountChanged += UpdateUnitAmount;
        unit.NavigationHandler.OnStartedMoving += StartFollowingUnit;
        unit.NavigationHandler.OnDestinationReached += StopFollowingUnit;

        UpdateUnitAmount(unit.HealthHandler.UnitAmount);
    }

    void OnDestroy()
    {
        if (unit != null)
        {
            unit.HealthHandler.OnUnitAmountChanged -= UpdateUnitAmount;
            unit.NavigationHandler.OnStartedMoving -= StartFollowingUnit;
            unit.NavigationHandler.OnDestinationReached -= StopFollowingUnit;
        }
    }

    void UpdateUnitAmount(int newAmount)
    {
        if (newAmount > 0)
        {
            unitAmountText.text = newAmount.ToString();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void StartFollowingUnit()
    {
        if (!shouldFollowUnit)
        {
            shouldFollowUnit = true;
            StartCoroutine(FollowUnit());
        }
    }

    void StopFollowingUnit()
    {
        shouldFollowUnit = false;
    }

    IEnumerator FollowUnit()
    {
        while (shouldFollowUnit)
        {
            transform.position = unit.transform.position + offset;
            yield return null;
        }
    }
}
