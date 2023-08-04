using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    Canvas canvas;
    [SerializeField] CanvasGroup transitionPanel;
    [SerializeField] float animationDuration = 1.0f;
    [SerializeField] float animationPauseDuration = 0.5f;

    Sequence transitionAnimation;
    bool isLoading = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        canvas = GetComponentInChildren<Canvas>();
        canvas.gameObject.SetActive(false);
    }

    public bool IsMainMenu => SceneManager.GetActiveScene().name == "MainMenu";

    public void LoadScene(string sceneName)
    {
        if (isLoading)
            return;

        isLoading = true;
        canvas.gameObject.SetActive(true);

        if (transitionAnimation != null && transitionAnimation.IsPlaying())
        {
            transitionAnimation.OnComplete(() => StartLoading(sceneName));
        }
        else
        {
            StartLoading(sceneName);
        }
    }

    void StartLoading(string sceneName)
    {
        transitionPanel.alpha = 0.0f;
        transitionAnimation = DOTween.Sequence();
        transitionAnimation.Append(transitionPanel.DOFade(1.0f, animationDuration))
            .AppendInterval(animationPauseDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                SceneManager.sceneLoaded += FinishLoading;
                SceneManager.LoadScene(sceneName);
            });
    }

    void FinishLoading(Scene scene, LoadSceneMode mode)
    {
        isLoading = false;
        SceneManager.sceneLoaded -= FinishLoading;

        transitionAnimation = DOTween.Sequence();
        transitionAnimation.Append(transitionPanel.DOFade(0.0f, animationDuration))
            .SetEase(Ease.InOutSine)
            .OnComplete(() => canvas.gameObject.SetActive(false));

        SaveHandler.Instance.Load();
    }
}
