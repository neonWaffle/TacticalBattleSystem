using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InfoDisplayHandler : MonoBehaviour
{
    [SerializeField] protected CanvasGroup infoPanel;

    UIInfoElement infoElement;

    protected virtual void Awake()
    {
        Hide();
    }

    void OnDisable()
    {
        Hide();
    }

    public virtual void Display(UIInfoElement uiInfoElement)
    {
        infoElement = uiInfoElement;
        infoElement.OnDisabled += Hide;
    }
 
    public virtual void Hide()
    {
        if (infoElement != null)
        {
            infoElement.OnDisabled -= Hide;
            infoElement = null;
        }
        infoPanel.gameObject.SetActive(false);
    }
}
