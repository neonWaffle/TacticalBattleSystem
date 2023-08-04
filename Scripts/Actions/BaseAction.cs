using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseAction
{
    public abstract void Execute();
    public bool IsSwiftAction { get; private set; }

    public event Action OnCompleted;

    public BaseAction(bool isSwiftAction)
    {
        IsSwiftAction = isSwiftAction;
    }

    protected void CompleteAction()
    {
        OnCompleted?.Invoke();
    }
}
