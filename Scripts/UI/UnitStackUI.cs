using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitStackUI : MonoBehaviour
{
    [SerializeField] Image unitImage;
    [SerializeField] TextMeshProUGUI amountText;
    [SerializeField] GameObject unitInfoPanel;

    public UnitStack UnitStack { get; private set; }

    void OnDestroy()
    {
        if (UnitStack != null)
        {
            UnitStack.OnAmountUpdated -= UpdateAmountText;
        }
    }

    public virtual void AssignStack(UnitStack stack)
    {
        UnitStack = stack;
        unitImage.sprite = stack.UnitSO.Portrait;
        unitImage.gameObject.SetActive(true);
        amountText.text = stack.Amount.ToString();
        UpdateAmountText(stack.Amount);

        unitInfoPanel.SetActive(true);

        stack.OnAmountUpdated += UpdateAmountText;
    }

    public virtual void RemoveStack()
    {
        unitInfoPanel.SetActive(false);

        if (UnitStack != null)
        {
            UnitStack.OnAmountUpdated -= UpdateAmountText;
            UnitStack = null;
        }
    }

    void UpdateAmountText(int amount)
    {
        amountText.text = amount.ToString();
    }
}
