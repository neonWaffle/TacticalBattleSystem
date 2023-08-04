using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleUIElement : MonoBehaviour
{
    [SerializeField] Image selectionImage;
    [SerializeField] Sprite deselectedSprite;
    [SerializeField] Sprite selectedSprite;

    public void Toggle(bool isSelected)
    {
        selectionImage.sprite = isSelected ? selectedSprite : deselectedSprite;
    }
}
