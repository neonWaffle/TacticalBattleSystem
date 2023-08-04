using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ParticleSystemCallback : MonoBehaviour
{
    public event Action OnStopped;

    void OnParticleSystemStopped()
    {
        OnStopped?.Invoke();
    }
}
