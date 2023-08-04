using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIInfoElement : MonoBehaviour
{
    Coroutine displayCoroutine;
    protected InfoDisplayHandler infoDisplayHandler;
    public RectTransform RectTransform { get; private set; }

    [SerializeField] float timeTilDisplay = 0.5f;

    public event Action OnDisabled;

    protected virtual void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }

    void OnDisable()
    {
        OnDisabled?.Invoke();
    }

    public void DisplayInfo()
    {
        displayCoroutine = StartCoroutine(WaitUntilDisplay());
    }

    public void HideInfo()
    {
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }
        infoDisplayHandler.Hide();
    }

    IEnumerator WaitUntilDisplay()
    {
        yield return new WaitForSecondsRealtime(timeTilDisplay);
        infoDisplayHandler.Display(this);
    }
}
