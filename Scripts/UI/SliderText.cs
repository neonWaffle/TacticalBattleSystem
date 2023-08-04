using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Slider slider;

    void OnEnable()
    {
        OnValueUpdate();
    }

    public void OnValueUpdate()
    {
        text.text = slider.value.ToString() + "/" + slider.maxValue.ToString();
    }
}