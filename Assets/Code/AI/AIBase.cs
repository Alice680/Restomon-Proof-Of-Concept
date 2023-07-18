using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBase : ScriptableObject
{
    public bool PreRun(DungeonManager manager)
    {
        int id = manager.GetIDFromActive();

        switch (manager.GetCreatureTypeFromID(id))
        {
            case CreatureType.Arena:
                RunArena(manager);
                return false;
        }

        return true;
    }

    public virtual int Run(DungeonManager manager, int state)
    {
        return -1;
    }

    protected void RunArena(DungeonManager manager)
    {
        Vector3Int area = new Vector3Int(-1, 1, 1);

        while (!manager.PositionValid(area) || !manager.TileEmpty(area))
            area = new Vector3Int(Random.Range(0, manager.GetMapSize().x), Random.Range(0, manager.GetMapSize().y), 0);

        manager.SpawnUnit(area);

        manager.EndTurn();
    }
}