using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonLayout : ScriptableObject
{
    public virtual DungeonMap GenerateDungeon()
    {
        return new DungeonMap(0, 0);
    }
}