using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DiceType { D4 = 4, D6 = 6, D8 = 8, D10 = 10, D12 = 12, D20 = 20 }

public static class Dice
{
    public static int Roll(DiceType diceType, int rolls)
    {
        int sum = 0;
        for (int i = 0; i < rolls; i++)
        {
            sum = Random.Range(0, (int)diceType + 1);
        }
        return sum;
    }
}
