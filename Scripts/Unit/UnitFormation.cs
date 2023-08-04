using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitFormation
{
    [field: SerializeField] public UnitStack UnitStack { get; private set; }
    [field: SerializeField, Range(0, 1)] public int SpawnX { get; private set; }
    [field: SerializeField, Range(0, 6)] public int SpawnZ { get; private set; }

    public UnitFormation(UnitStack stack, int spawnX, int spawnZ)
    {
        UnitStack = stack;
        SpawnX = spawnX;
        SpawnZ = spawnZ;
    }
}
