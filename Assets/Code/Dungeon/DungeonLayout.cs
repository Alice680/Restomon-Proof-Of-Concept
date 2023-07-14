using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonLayout : ScriptableObject
{
    public virtual DungeonMap GenerateDungeon()
    {
        return new DungeonMap(0, 0);
    }

    public virtual Vector3Int GetStartPosition()
    {
        return new Vector3Int(-1, -1,0);
    }

    public virtual AIBase GetAI()
    {
        return null;
    }
}