using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleLogTextUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] float timeTilDisplay = 0.5f;
    Coroutine detailDisplayCoroutine;

    BattleLogMessage message;

    public void Setup(BattleLogMessage message)
    {
        this.message = message;
        text.text = message.Action;
    }

    public void ShowDetails()
    {
        if (!string.IsNullOrWhiteSpace(message.Details))
        {
            detailDisplayCoroutine = StartCoroutine(WaitTilDisplay());
        }
    }

    IEnumerator WaitTilDisplay()
    {
        yield return new WaitForSecondsRealtime(timeTilDisplay);
        BattleLogger.Instance.ShowDetails(message, rectTransform);
    }

    public void HideDetails()
    {
        if (detailDisplayCoroutine != null)
        {
            StopCoroutine(detailDisplayCoroutine);
        }
        BattleLogger.Instance.HideDetails();
    }
}
