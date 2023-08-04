using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    public void Load(SaveData saveData);
    public void Save(SaveData saveData);
}
