using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject continueButton;

    void Start()
    {
        continueButton.SetActive(SaveHandler.Instance.HasExistingSave());
    }

    public void Continue()
    {
        SceneLoader.Instance.LoadScene("BattleSelection");
    }

    public void StartNewGame()
    {
        SaveHandler.Instance.StartNewSave();
        SceneLoader.Instance.LoadScene("BattleSelection");
    }
}
