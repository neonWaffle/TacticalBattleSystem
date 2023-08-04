using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundDivider : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roundText;

    public void SetRound(int round)
    {
        roundText.text = round.ToString();
    }
}
