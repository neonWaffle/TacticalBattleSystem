using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BattleOverUIHandler : MonoBehaviour
{
    Canvas canvas;
    [Header("Losses")]
    [SerializeField] CanvasGroup uiPanel;
    [SerializeField] TextMeshProUGUI outcomeText;

    [SerializeField] CanvasGroup playerUnitPanel;
    [SerializeField] CanvasGroup enemyUnitPanel;
    [SerializeField] GameObject continueButton;

    DeadUnitUI[] deadUnitsUIPlayer;
    DeadUnitUI[] deadUnitsUIEnemy;
    Player player;

    [Header("Battle Rewards")]
    [SerializeField] CanvasGroup battleRewardPanel;
    [SerializeField] Image xpProgressBar;
    [SerializeField] CanvasGroup xpBarPanel;
    [SerializeField] GameObject battleRewardMoneyPanel;
    [SerializeField] TextMeshProUGUI battleRewardMoneyText;
    [SerializeField] TextMeshProUGUI gainedXPText;
    [SerializeField] TextMeshProUGUI xpProgressText;
    [SerializeField] TextMeshProUGUI currentLevelText;
    [SerializeField] TextMeshProUGUI nextLevelText;
    [SerializeField] GameObject nextLevelIcon;
    [SerializeField] GameObject currentLevelIcon;
    [SerializeField] GameObject battleRewardConfirmButton;

    [Header("Level Up")]
    [SerializeField] CanvasGroup levelUpPanel;
    [SerializeField] TextMeshProUGUI increasedRosterSizeText;
    [SerializeField] GameObject levelUpConfirmButton;
    [SerializeField] AudioClip levelUpSFX;

    [Header("Animation")]
    [SerializeField] float fadeDuration = 1.5f;
    [SerializeField] float popupFadeDuration = 0.5f;
    [SerializeField] float levelIconAnimationDuration = 0.5f;
    [SerializeField] float levelIconPauseDuration = 0.2f;
    [SerializeField] float levelIconAnimationScale = 1.2f;
    [SerializeField] float popupAnimationDuration = 0.5f;
    [SerializeField] float xpProgressDuration = 1.5f;
    [SerializeField] AudioClip popupSFX;

    void Awake()
    {
        deadUnitsUIPlayer = playerUnitPanel.GetComponentsInChildren<DeadUnitUI>();
        deadUnitsUIEnemy = enemyUnitPanel.GetComponentsInChildren<DeadUnitUI>();

        canvas = GetComponentInChildren<Canvas>();
        DisableUI();
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public IEnumerator ShowBattleResults(BattleOutcome battleOutcome, List<UnitStack> deadPlayerUnits, List<UnitStack> deadEnemyUnits)
    {
        outcomeText.text = battleOutcome.ToString();
        uiPanel.alpha = 0.0f;
        uiPanel.DOFade(1.0f, fadeDuration).SetEase(Ease.OutSine);
        canvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(fadeDuration);

        yield return SetupUnitStackUI(deadUnitsUIEnemy, deadEnemyUnits, false);
        yield return SetupUnitStackUI(deadUnitsUIPlayer, deadPlayerUnits, true);

        if (battleOutcome == BattleOutcome.Victory)
        {
            yield return StartCoroutine(ShowBattleRewards());
        }

        yield return new WaitForSeconds(popupAnimationDuration);

        continueButton.SetActive(true);
    }

    public void ReturnToBattleSelection()
    {
        SceneLoader.Instance.LoadScene("BattleSelection");
    }

    IEnumerator SetupUnitStackUI(DeadUnitUI[] deadUnitsUI, List<UnitStack> deadUnitStacks, bool isPlayer)
    {
        var popupDelay = new WaitForSeconds(popupAnimationDuration);
        for (int i = 0; i < deadUnitStacks.Count; i++)
        {
            deadUnitsUI[i].ShowUnitInfo(deadUnitStacks[i], isPlayer);
            AudioManager.Instance.PlaySFX(popupSFX);
            yield return popupDelay;
        }
    }

    IEnumerator ShowBattleRewards()
    {
        battleRewardPanel.alpha = 0.0f;
        battleRewardPanel.gameObject.SetActive(true);
        battleRewardPanel.DOFade(1.0f, popupFadeDuration);
        yield return new WaitForSeconds(popupFadeDuration);

        var popupDelay = new WaitForSeconds(popupAnimationDuration);
        yield return popupDelay;

        battleRewardMoneyText.text = BattleManager.Instance.CurrentBattleDataSO.Money.ToString();
        battleRewardMoneyPanel.SetActive(true);
        AudioManager.Instance.PlaySFX(popupSFX);
        yield return popupDelay;

        if (player.Level < player.LevelUpSO.MaxLevel || (player.LevelledUp && player.Level == player.LevelUpSO.MaxLevel))
        {
            gainedXPText.text = $"+{BattleManager.Instance.CurrentBattleDataSO.XP} XP";
            gainedXPText.gameObject.SetActive(true);
            AudioManager.Instance.PlaySFX(popupSFX);
            yield return popupDelay;
        }

        int levelBeforeXPAdd = player.LevelledUp ? player.Level - 1 : player.Level;
        int xpToLevelUp = player.Level < player.LevelUpSO.MaxLevel
            ? player.LevelUpSO.GetRequiredXP(levelBeforeXPAdd + 1)
            : player.LevelUpSO.MaxXP;
        int xpToCurrentLevel = player.LevelUpSO.GetRequiredXP(levelBeforeXPAdd);
        int xpDifference = xpToLevelUp - xpToCurrentLevel;

        if (player.Level < player.LevelUpSO.MaxLevel || player.LevelledUp)
        {
            nextLevelText.text = player.LevelledUp ? player.Level.ToString() : (player.Level + 1).ToString();
            nextLevelIcon.SetActive(true);

            xpProgressBar.fillAmount = (player.XP - BattleManager.Instance.CurrentBattleDataSO.XP - xpToCurrentLevel) / (float)xpDifference;
            xpProgressBar.DOFillAmount((player.XP - xpToCurrentLevel) / (float)xpDifference, xpProgressDuration);

            currentLevelText.text = player.LevelledUp ? (player.Level - 1).ToString() : player.Level.ToString();
        }
        else
        {
            xpProgressBar.fillAmount = 1.0f;
            nextLevelText.text = player.Level.ToString();
            nextLevelIcon.SetActive(player.LevelledUp);

            currentLevelText.text = player.Level.ToString();
        }

        currentLevelIcon.SetActive(true);
        xpBarPanel.gameObject.SetActive(true);

        AudioManager.Instance.PlaySFX(popupSFX);
        yield return new WaitForSeconds(xpProgressDuration);

        if (player.LevelledUp)
        {
            var levelIconAnim = DOTween.Sequence();
            levelIconAnim.Append(currentLevelIcon.transform.DOScale(levelIconAnimationScale, levelIconAnimationDuration))
                .Join(nextLevelIcon.transform.DOScale(levelIconAnimationScale, levelIconAnimationDuration))
                .OnComplete(() =>
                {
                    currentLevelText.text = player.Level.ToString();
                    nextLevelText.text = (player.Level + 1).ToString();
                    nextLevelIcon.SetActive(player.Level < player.LevelUpSO.MaxLevel);
                    if (levelUpSFX != null)
                    {
                        AudioManager.Instance.PlaySFX(levelUpSFX);
                    }
                });

            yield return new WaitForSeconds(levelIconAnim.Duration());

            levelIconAnim
                .Append(currentLevelIcon.transform.DOScale(1.0f, levelIconAnimationDuration))
                .Join(nextLevelIcon.transform.DOScale(1.0f, levelIconAnimationDuration));

            yield return new WaitForSeconds(levelIconAnim.Duration());
            
            if (player.Level < player.LevelUpSO.MaxLevel)
            {
                xpToCurrentLevel = xpToLevelUp;
                xpToLevelUp = player.LevelUpSO.GetRequiredXP(player.Level + 1);
                xpDifference = xpToLevelUp - xpToCurrentLevel;
                xpProgressBar.fillAmount = 0.0f;
                xpProgressBar.DOFillAmount((player.XP - xpToCurrentLevel) / (float)xpDifference, xpProgressDuration);
                yield return new WaitForSeconds(xpProgressDuration);
            }
            yield return ShowLevelUpRewards();
        }

        xpProgressText.text = $"{player.XP}/{xpToLevelUp}";
        xpProgressText.gameObject.SetActive(true);

        yield return popupDelay;

        battleRewardConfirmButton.SetActive(true);
    }

    IEnumerator ShowLevelUpRewards()
    {
        levelUpPanel.alpha = 0.0f;
        levelUpPanel.gameObject.SetActive(true);
        levelUpPanel.DOFade(1.0f, popupFadeDuration);
        yield return new WaitForSeconds(popupFadeDuration);

        var popupDelay = new WaitForSeconds(popupAnimationDuration);
        yield return popupDelay;

        var levelUpReward = player.LevelUpSO.LevelUpRewards[player.Level - 1];

        if (levelUpReward.IncreaseRosterSize)
        {
            increasedRosterSizeText.gameObject.SetActive(true);
            AudioManager.Instance.PlaySFX(popupSFX);
            yield return popupDelay;
        }

        levelUpConfirmButton.SetActive(true);
    }

    void DisableUI()
    {
        canvas.gameObject.SetActive(false);

        foreach (var stackUI in deadUnitsUIEnemy)
        {
            stackUI.HideUnitInfo();
        }

        foreach (var stackUI in deadUnitsUIPlayer)
        {
            stackUI.HideUnitInfo();
        }

        continueButton.SetActive(false);

        levelUpPanel.gameObject.SetActive(false);
        battleRewardPanel.gameObject.SetActive(false);

        increasedRosterSizeText.gameObject.SetActive(false);
        levelUpConfirmButton.SetActive(false);

        gainedXPText.gameObject.SetActive(false);
        xpProgressText.gameObject.SetActive(false);
        battleRewardMoneyPanel.SetActive(false);
        xpBarPanel.gameObject.SetActive(false);
        battleRewardConfirmButton.SetActive(false);
        currentLevelIcon.SetActive(false);
        nextLevelIcon.SetActive(false);
    }
}
