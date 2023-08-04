using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LimitedSlider : MonoBehaviour
{
    int limit;
    [SerializeField] Slider slider;

    public void SetLimit(int limit)
    {
        this.limit = limit;
    }

    public void OnValueUpdate()
    {
        slider.value = Mathf.Clamp(slider.value, slider.minValue, limit);
    }
}
