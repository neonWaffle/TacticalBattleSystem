using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CursorType { REGULAR, ABILITY }

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [SerializeField] Texture2D defaultIcon;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        RevertToDefaultIcon();
    }

    public void ChangeIcon(Texture2D cursorIcon)
    {
        Cursor.SetCursor(cursorIcon, new Vector2(0, 0), CursorMode.ForceSoftware);
    }

    public void RevertToDefaultIcon()
    {
        Cursor.SetCursor(defaultIcon, new Vector2(0, 0), CursorMode.ForceSoftware);
    }
}
