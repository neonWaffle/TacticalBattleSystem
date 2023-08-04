using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageTypeInfoUI : MonoBehaviour
{
    [SerializeField] Image damageTypeImage;
    [SerializeField] TextMeshProUGUI damageTypeText;
    [SerializeField] Sprite poisonIcon;
    [SerializeField] Sprite fireIcon;
    [SerializeField] Sprite iceIcon;
    [SerializeField] Sprite electricityIcon;

    public void Setup(DamageType damageType)
    {
        damageTypeText.text = damageType.ToString();
        switch (damageType)
        {
            case DamageType.Ice:
                damageTypeImage.sprite = iceIcon;
                break;
            case DamageType.Poison:
                damageTypeImage.sprite = poisonIcon;
                break;
            case DamageType.Fire:
                damageTypeImage.sprite = fireIcon;
                break;
            case DamageType.Electricity:
                damageTypeImage.sprite = electricityIcon;
                break;
        }
    }
}
