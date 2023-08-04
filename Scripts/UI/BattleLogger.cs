using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum LogColour { Ally, Enemy, Damage, Heal, Ability }

public static class BattleLogString
{
    public static string Bold(this string str)
    {
        return "<b>" + str + "</b>";
    }

    public static string Colour(this string str, string colour)
    {
        return $"<color=#{colour}>{str}</color>";
    }
}

public class BattleLogger : MonoBehaviour
{
    public static BattleLogger Instance { get; private set; }

    Canvas canvas;

    [SerializeField] BattleLogTextUI textPrefab;
    [SerializeField] CanvasGroup textPanel;
    ScrollRect scrollRect;

    [SerializeField] CanvasGroup logDetailPanel;
    [SerializeField] TextMeshProUGUI logDetailText;

    [SerializeField] Color allyColour = Color.blue;
    [SerializeField] Color enemyColour = Color.red;
    [SerializeField] Color abilityColour = Color.blue;
    [SerializeField] Color damageColour = Color.red;
    [SerializeField] Color healColour = Color.green;

    public Dictionary<LogColour, string> Colours { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        canvas = GetComponentInChildren<Canvas>();

        Colours = new Dictionary<LogColour, string>();
        Colours.Add(LogColour.Ally, ColorUtility.ToHtmlStringRGBA(allyColour));
        Colours.Add(LogColour.Enemy, ColorUtility.ToHtmlStringRGBA(enemyColour));
        Colours.Add(LogColour.Ability, ColorUtility.ToHtmlStringRGBA(abilityColour));
        Colours.Add(LogColour.Damage, ColorUtility.ToHtmlStringRGBA(damageColour));
        Colours.Add(LogColour.Heal, ColorUtility.ToHtmlStringRGBA(healColour));

        scrollRect = GetComponentInChildren<ScrollRect>();
        logDetailPanel.gameObject.SetActive(false);

        DisableUI();
    }

    void Start()
    {
        BattleManager.Instance.OnBattleStarted += EnableUI;
        BattleManager.Instance.OnBattleOver += DisableUI;
    }

    void OnDestroy()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnBattleStarted -= EnableUI;
            BattleManager.Instance.OnBattleOver -= DisableUI;
        }
    }

    public void DisplayMessage(BattleLogMessage message)
    {
        var text = Instantiate(textPrefab, textPanel.transform);
        text.Setup(message);

        StartCoroutine(ScrollToBottom());
    }

    IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0.0f;
    }

    public void ShowDetails(BattleLogMessage message, RectTransform rectTransform)
    {
        logDetailPanel.transform.position = rectTransform.position + new Vector3(0.0f, rectTransform.sizeDelta.y, 0.0f);
        logDetailText.text = message.Details;
        logDetailPanel.gameObject.SetActive(true);
    }

    public void HideDetails()
    {
        logDetailPanel.gameObject.SetActive(false);
    }

    void DisableUI()
    {
        canvas.gameObject.SetActive(false);
    }


    void EnableUI()
    {
        canvas.gameObject.SetActive(true);
    }
}
