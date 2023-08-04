using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUIInfoElement : UIInfoElement
{
    public Ability Ability { get; set; }

    protected override void Awake()
    {
        base.Awake();
        infoDisplayHandler = transform.root.GetComponentInChildren<AbilityInfoDisplayHandler>();
    }
}
