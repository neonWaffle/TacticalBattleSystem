using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType { Attack, AC, Dexterity, Constitution, Wisdom, Speed }
public enum SavingThrowType { Fortitude, Reflex, Will }

public class StatHandler : MonoBehaviour
{
    Unit unit;

    public Dictionary<StatType, Stat> Stats { get; private set; }
    public int Initiative { get; private set; }

    void Awake()
    {
        unit = GetComponent<Unit>();

        Stats = new Dictionary<StatType, Stat>();
        Stats.Add(StatType.Attack, new Stat(unit.DataSO.Attack));
        Stats.Add(StatType.AC, new Stat(unit.DataSO.AC));
        Stats.Add(StatType.Dexterity, new Stat(unit.DataSO.Dexterity));
        Stats.Add(StatType.Constitution, new Stat(unit.DataSO.Constitution));
        Stats.Add(StatType.Wisdom, new Stat(unit.DataSO.Wisdom));
        Stats.Add(StatType.Speed, new Stat(unit.DataSO.Speed));

        Initiative = Dice.Roll(DiceType.D20, 1) + CalculateBonus(StatType.Dexterity);
    }

    public int CalculateSavingThrowBonus(SavingThrowType throwType)
    {
        switch (throwType)
        {
            case SavingThrowType.Fortitude:
                return CalculateBonus(StatType.Constitution);
            case SavingThrowType.Will:
                return CalculateBonus(StatType.Wisdom);
            case SavingThrowType.Reflex:
                return CalculateBonus(StatType.Dexterity);
            default:
                return 0;
        }
    }

    public int CalculateBonus(StatType statType)
    {
        return Mathf.FloorToInt((Stats[statType].Value - 10) * 0.5f);
    }
}
