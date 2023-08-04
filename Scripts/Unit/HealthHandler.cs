using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthHandler : MonoBehaviour
{
    Unit unit;

    public int CurrentHP { get; private set; }
    public bool IsDead { get; private set; }

    public int MaxUnitAmount { get; private set; }
    public int UnitAmount { get; set; }

    [SerializeField] HealthPopup healPopupPrefab;
    [SerializeField] HealthPopup damagePopupPrefab;
    [SerializeField] Transform popupSpawnPoint;

    [SerializeField] AudioClip[] hitSFX;
    [SerializeField] AudioClip[] deathSFX;

    public event Action<int> OnUnitAmountChanged;
    public event Action<Unit> OnDied;

    void Awake()
    {
        unit = GetComponent<Unit>();
        IsDead = false;
    }

    public void Init(int maxUnitAmount)
    {
        MaxUnitAmount = maxUnitAmount;
        UnitAmount = maxUnitAmount;
        CurrentHP = unit.DataSO.HitPoints * UnitAmount;
    }

    public int CalculateDeadUnitAmount(int hp)
    {
        return UnitAmount - Mathf.CeilToInt((float)hp / unit.DataSO.HitPoints);
    }

    public int CalculateRevivedUnitAmount(int hp)
    {
        return Mathf.Min(Mathf.CeilToInt((float)hp / unit.DataSO.HitPoints), MaxUnitAmount - UnitAmount);
    }

    public void TakeDamage(Unit instigator, int damage)
    {
        if (IsDead || damage <= 0)
            return;

        CurrentHP -= damage;

        int deadUnitAmount = CalculateDeadUnitAmount(CurrentHP);
        UnitAmount -= deadUnitAmount;
        OnUnitAmountChanged?.Invoke(UnitAmount);

        if (CurrentHP <= 0)
        {
            if (deathSFX.Length > 0)
            {
                AudioManager.Instance.PlaySFX(deathSFX[UnityEngine.Random.Range(0, deathSFX.Length)]);
            }

            Die();
        }
        else
        {
            if (hitSFX.Length > 0)
            {
                AudioManager.Instance.PlaySFX(hitSFX[UnityEngine.Random.Range(0, hitSFX.Length)]);
            }
        }

        var logMessage = string.Format("{0} {1} {2}. ",
             instigator.DataSO.Title.Bold(),
            $"deal {damage.ToString().Bold()} damage to".Colour(BattleLogger.Instance.Colours[LogColour.Damage]),
            unit.DataSO.Title.Bold());

        if (deadUnitAmount > 0)
        {
            logMessage += $"{deadUnitAmount.ToString().Bold()} die.".Colour(BattleLogger.Instance.Colours[LogColour.Damage]);
        }
        BattleLogger.Instance.DisplayMessage(new BattleLogMessage(logMessage));

        var popup = Instantiate(damagePopupPrefab, popupSpawnPoint.position, Quaternion.identity);
        popup.Setup(damage, deadUnitAmount, false);
    }

    public void Heal(Unit instigator, int amount)
    {
        if (amount <= 0)
            return;

        CurrentHP += amount;

        int revivedUnitAmount = CalculateRevivedUnitAmount(CurrentHP);
        if (revivedUnitAmount > 0)
        {
            UnitAmount += revivedUnitAmount;
            OnUnitAmountChanged?.Invoke(UnitAmount);

            var logMessage = string.Format("{0} {1} {2} {3}. ",
                instigator.DataSO.Title.Bold(),
                $"heals".Colour(BattleLogger.Instance.Colours[LogColour.Heal]),
                unit.DataSO.Title.Bold(),
                $" for {amount.ToString().Bold()} hit points.".Colour(BattleLogger.Instance.Colours[LogColour.Heal]));

            BattleLogger.Instance.DisplayMessage(new BattleLogMessage(logMessage));
        }

        var popup = Instantiate(healPopupPrefab, popupSpawnPoint.position, Quaternion.identity);
        popup.Setup(amount, revivedUnitAmount, true);
    }

    void Die()
    {
        IsDead = true;
        unit.Animator.SetTrigger("Die");
        unit.Node.Unit = null;
        unit.Node = null;

        unit.StatusEffectHandler.RemoveAllStatusEffects();

        OnDied?.Invoke(unit);
    }
}
