using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbilityParticle : MonoBehaviour
{
    [SerializeField] bool finishOnParticleStop;
    new ParticleSystemCallback particleSystem;

    public event Action OnFinished;

    public virtual void Setup(Unit targetUnit)
    {
        if (finishOnParticleStop)
        {
            particleSystem = GetComponentInChildren<ParticleSystemCallback>();
            if (particleSystem != null)
            {
                particleSystem.OnStopped += OnStopped;
            }
            else
            {
                Debug.Log($"{name} is missing ParticleSystemCallback");
                Destroy(gameObject);
            }
        }
    }


    void OnStopped()
    {
        if (particleSystem != null)
        {
            particleSystem.OnStopped -= OnStopped;
        }
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        OnFinished?.Invoke();
    }
}
