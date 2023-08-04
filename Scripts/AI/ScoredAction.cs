using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoredAction
{
    public BaseAction[] Actions { get; private set; }
    public float Score { get; private set; }

    public ScoredAction(float score, params BaseAction[] actions)
    {
        Actions = actions;
        Score = score;
    }
}
