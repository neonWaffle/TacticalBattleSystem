using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLogMessage
{
    public string Action { get; private set; }
    public string Details { get; private set; }

    public BattleLogMessage(string action)
    {
        Action = action;
    }

    public BattleLogMessage(string action, string details)
    {
        Action = action;
        Details = details;
    }
}
