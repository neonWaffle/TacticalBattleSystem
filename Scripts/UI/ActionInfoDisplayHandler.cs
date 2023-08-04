using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionInfoDisplayHandler : InfoDisplayHandler
{
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] RectTransform infoPanelRect;

    public override void Display(UIInfoElement uiInfoElement)
    {
        base.Display(uiInfoElement);

        var actionUI = uiInfoElement as ActionUIInfoElement;
        if (actionUI == null)
            return;

        titleText.text = actionUI.ActionTitle;
        descriptionText.text = actionUI.ActionDescription;

        infoPanel.transform.position = uiInfoElement.RectTransform.position
            + new Vector3(infoPanelRect.sizeDelta.x * 0.5f - uiInfoElement.RectTransform.sizeDelta.x * 0.5f,
            infoPanelRect.sizeDelta.y * 0.5f + uiInfoElement.RectTransform.sizeDelta.y * 0.5f,
            0.0f);

        infoPanel.gameObject.SetActive(true);
    }
}
