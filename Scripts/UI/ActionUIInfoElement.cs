using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ActionUIInfoElement : UIInfoElement
{
    [field: SerializeField, ResizableTextArea] public string ActionTitle { get; private set; }
    [field: SerializeField, ResizableTextArea] public string ActionDescription { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        infoDisplayHandler = transform.root.GetComponentInChildren<ActionInfoDisplayHandler>();
    }
}
