using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAction : BaseAction
{
    Ability ability;
    Unit executingUnit;
    GridNode targetNode;

    public bool IsDefaultAbilityAction => ability == executingUnit.AbilityHandler.DefaultAttackAbility || ability == executingUnit.AbilityHandler.DefenceAbility;

    AbilityParticle abilityParticle;

    public AbilityAction(Ability ability, Unit executingUnit, GridNode targetNode) : base(ability.AbilitySO.CastingTime == CastingTime.Swift)
    {
        this.ability = ability;
        this.executingUnit = executingUnit;
        this.targetNode = targetNode;
    }

    public override void Execute()
    {
        var dir = targetNode.transform.position - executingUnit.transform.position;
        if (dir != Vector3.zero)
        {
            executingUnit.transform.rotation = Quaternion.LookRotation(dir);
        }

        executingUnit.Animator.SetTrigger(ability.AbilitySO.Animation);

        if (ability.AbilitySO.SFX != null)
        {
            AudioManager.Instance.PlaySFX(ability.AbilitySO.SFX);
        }

        if (ability.AbilitySO.ExecuteOnAnimation)
        {
            executingUnit.ActionHandler.OnActionCompleted += UseAbility;
        }

        if (ability.AbilitySO.Particle != null)
        {
            if (!ability.AbilitySO.SpawnParticleOnAnimation)
            {
                SpawnAbilityParticle();
            }
            else
            {
                executingUnit.AbilityHandler.OnAbilitySpawn += SpawnAbilityParticle;
            }
        }
    }

    void UseAbility()
    {
        if (ability.AbilitySO.ExecuteOnAnimation)
        {
            executingUnit.ActionHandler.OnActionCompleted -= UseAbility;
        }
        else
        {
            abilityParticle.OnFinished -= UseAbility;
        }

        if (ability.AbilitySO != executingUnit.AbilityHandler.DefaultAttackAbility.AbilitySO)
        {
            BattleLogger.Instance.DisplayMessage(new BattleLogMessage(string.Format("{0} {1}",
                executingUnit.DataSO.Title.Bold(),
                $"uses ability: {ability.AbilitySO.Title.Bold()}".Colour(BattleLogger.Instance.Colours[LogColour.Ability]))));
        }

        ability.Use(targetNode);

        CompleteAction();
    }

    void SpawnAbilityParticle()
    {
        executingUnit.AbilityHandler.OnAbilitySpawn -= SpawnAbilityParticle;

        var spawnPoint = targetNode.Unit == null 
            ? targetNode.transform : ability.AbilitySO.SpawnParticleOnTarget 
            ? targetNode.Unit.GetSpawnPoint(ability.AbilitySO.ParticleSpawnPointType) : executingUnit.GetSpawnPoint(ability.AbilitySO.ParticleSpawnPointType);
        
        abilityParticle = Object.Instantiate(ability.AbilitySO.Particle, spawnPoint.position, Quaternion.identity);

        if (!ability.AbilitySO.ExecuteOnAnimation)
        {
            abilityParticle.OnFinished += UseAbility;
            abilityParticle.Setup(targetNode.Unit);
        }
    }
}
