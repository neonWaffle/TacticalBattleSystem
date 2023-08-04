using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/LevelUpData")]
public class LevelUpDataSO : ScriptableObject
{
    [field: SerializeField] public int MaxLevel { get; private set; }
    [field: SerializeField] public LevelUpReward[] LevelUpRewards { get; private set; }
    
    public int MaxXP => GetRequiredXP(MaxLevel);

    public int GetRequiredXP(int nextLevel) => (int)Mathf.Pow(nextLevel / 0.1f, 2);
}
