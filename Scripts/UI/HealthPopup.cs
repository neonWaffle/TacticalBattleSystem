using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class HealthPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthAmountText;
    [SerializeField] TextMeshProUGUI unitAmountText;
    [SerializeField] CanvasGroup unitAmountPanel;

    [SerializeField] float duration = 0.5f;
    [SerializeField] float yTargetOffset = 0.5f;

    public void Setup(int healthAmount, int unitAmount, bool wasHealed)
    {
        if (wasHealed)
        {
            healthAmountText.text = healthAmount.ToString();
            unitAmountText.text = unitAmount.ToString();
        }
        else
        {
            healthAmountText.text = $"-{healthAmount}";
            unitAmountText.text = $"-{unitAmount}";
        }

        unitAmountPanel.gameObject.SetActive(unitAmount > 0);
        transform.transform.DOMoveY(transform.position.y + yTargetOffset, duration).SetEase(Ease.OutSine);
        Destroy(gameObject, duration);
    }
}
