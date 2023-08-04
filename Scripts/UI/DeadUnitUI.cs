using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeadUnitUI : MonoBehaviour
{
    [SerializeField] Image unitImage;
    [SerializeField] Image unitAmountBackground;
    [SerializeField] TextMeshProUGUI unitAmountText;
    [SerializeField] CanvasGroup unitInfoPanel;

    [SerializeField] Color playerColour = Color.blue;
    [SerializeField] Color enemyColour = Color.red;

    public void ShowUnitInfo(UnitStack unitStack, bool isPlayerUnit)
    {
        unitImage.sprite = unitStack.UnitSO.Portrait;
        unitAmountBackground.color = isPlayerUnit ? playerColour : enemyColour;
        unitAmountText.text = unitStack.Amount.ToString();
        unitInfoPanel.gameObject.SetActive(true);
    }

    public void HideUnitInfo()
    {
        unitInfoPanel.gameObject.SetActive(false);
    }
}
