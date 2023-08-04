using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusEffectInfoPanel : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI durationText;

    public void Setup(StatusEffect statusEffect)
    {
        image.sprite = statusEffect.StatusEffectSO.Icon;
        titleText.text = statusEffect.StatusEffectSO.Title;
        durationText.text = $"Duration: {statusEffect.RemainingTurns}";
    }
}
