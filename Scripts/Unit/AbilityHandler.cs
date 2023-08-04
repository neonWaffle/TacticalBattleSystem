using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbilityHandler : MonoBehaviour
{
    Unit unit;

    public Ability SelectedAbility { get; private set; }
    public Ability DefaultAttackAbility { get; private set; }
    public Ability DefenceAbility { get; private set; }
    public List<Ability> Abilities { get; private set; }
    
    public Dictionary<AbilityType, int> AbilityTypeCooldowns { get; private set; }

    public bool IsDefaultAttackSelected => SelectedAbility == DefaultAttackAbility;

    public event Action OnAbilitySelected;
    public event Action OnAbilitySpawn;

    void Awake()
    {
        unit = GetComponent<Unit>();

        DefaultAttackAbility = new Ability(unit.DataSO.DefaultAttackAbility, unit);
        DefenceAbility = new Ability(unit.DataSO.DefenceAbility, unit);

        Abilities = new List<Ability>(unit.DataSO.Abilities.Length);
        foreach (var abilitySO in unit.DataSO.Abilities)
        {
            var ability = new Ability(abilitySO, unit);
            ability.OnAbilityUsed += UpdateAbilityTypeCooldown;
            Abilities.Add(ability);
        }

        AbilityTypeCooldowns = new Dictionary<AbilityType, int>();
    }

    void OnDestroy()
    {
        foreach (var ability in Abilities)
        {
            ability.OnAbilityUsed -= UpdateAbilityTypeCooldown;
        }
    }

    void UpdateAbilityTypeCooldown(Ability ability)
    {
        if (ability != DefaultAttackAbility && ability != DefenceAbility)
        {
            AbilityTypeCooldowns[ability.AbilitySO.AbilityType] = BattleManager.Instance.CurrentRound;
        }
    }

    public void SelectAbility(Ability ability)
    {
        SelectedAbility = ability;
        OnAbilitySelected?.Invoke();
    }

    public void SelectDefaultAttack()
    {
        SelectAbility(DefaultAttackAbility);
    }

    public void SelectDefendAbility()
    {
        SelectAbility(DefenceAbility);
    }

    public void AdvanceRound()
    {
        foreach (var ability in Abilities)
        {
            ability.AdvanceRound();
        }
    }

    //Animation event
    void OnSpawnAbility()
    {
        OnAbilitySpawn?.Invoke();
    }
}
