using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AbilityInfoDisplayHandler : InfoDisplayHandler
{
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI castingTimeText;
    [SerializeField] RectTransform infoPanelRect;

    public override void Display(UIInfoElement uiInfoElement)
    {
        base.Display(uiInfoElement);

        var abilityUI = uiInfoElement as AbilityUIInfoElement;
        if (abilityUI == null)
            return;

        titleText.text = abilityUI.Ability.AbilitySO.Title;
        descriptionText.text = abilityUI.Ability.AbilitySO.Description;
        castingTimeText.text = abilityUI.Ability.AbilitySO.CastingTime.ToString();

        infoPanel.transform.position = uiInfoElement.RectTransform.position
            + new Vector3(infoPanelRect.sizeDelta.x * 0.5f - uiInfoElement.RectTransform.sizeDelta.x * 0.5f,
            infoPanelRect.sizeDelta.y * 0.5f + uiInfoElement.RectTransform.sizeDelta.y * 0.5f,
            0.0f);

        infoPanel.gameObject.SetActive(true);
    }
}
