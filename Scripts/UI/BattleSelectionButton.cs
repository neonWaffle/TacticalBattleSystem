using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BattleSelectionButton : MonoBehaviour
{
    [field: SerializeField] public BattleDataSO BattleDataSO { get; private set; }
    [SerializeField] TextMeshProUGUI levelNumberText;
    [SerializeField] Image lockedIcon;
    Button button;

    [SerializeField] float selectAnimDuration = 0.5f;
    [SerializeField] float selectScale = 1.2f;
    Vector3 defaultScale;

    bool isSelected;

    BattleSelectionHandler battleSelectionHandler;

    void Awake()
    {
        button = GetComponent<Button>();
        battleSelectionHandler = transform.root.GetComponent<BattleSelectionHandler>();

        Lock();

        defaultScale = transform.localScale;
    }

    public void Unlock()
    {
        button.interactable = true;
        lockedIcon.gameObject.SetActive(false);
        levelNumberText.gameObject.SetActive(true);
    }

    public void Lock()
    {
        button.interactable = false;
        lockedIcon.gameObject.SetActive(true);
        levelNumberText.gameObject.SetActive(false);
    }

    public void Select()
    {
        if (!isSelected)
        {
            isSelected = true;
            battleSelectionHandler.SelectBattle(this);
            transform.DOScale(selectScale, selectAnimDuration);
        }
    }

    public void Deselect()
    {
        isSelected = false;
        transform.DOScale(defaultScale, selectAnimDuration);
    }
}
