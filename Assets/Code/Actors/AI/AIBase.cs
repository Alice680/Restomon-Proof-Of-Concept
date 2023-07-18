using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBase : ScriptableObject
{
    //Special checks
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

    protected void RunArena(DungeonManager manager)
    {
        Vector3Int area = new Vector3Int(-1, 1, 1);

        while (!manager.PositionValid(area) || !manager.TileEmpty(area))
            area = new Vector3Int(Random.Range(0, manager.GetMapSize().x), Random.Range(0, manager.GetMapSize().y), 0);

        manager.SpawnUnit(area);

        manager.EndTurn();
    }

    //Run data
    public virtual void Run(DungeonManager manager)
    {

    }

    //Common Data
    protected int GetAttackTarget(DungeonManager manager, int index)
    {
        List<int> valid_targets = new List<int>();

        foreach (int i in manager.GetPlayerIDS())
            if (manager.AttackTargetValid(manager.GetPositionFromID(i), index))
                valid_targets.Add(i);

        if (valid_targets.Count != 0)
            return valid_targets[Random.Range(0, valid_targets.Count)];

        return -1;
    }

    protected int GetNearestEnemy(DungeonManager manager)
    {
        return -1;
    }

    protected Direction GetDirectionFromPostions(Vector3Int start, Vector3Int goal)
    {
        if (start.x > goal.x && start.y == goal.y)
            return Direction.Left;

        if (start.x < goal.x && start.y == goal.y)
            return Direction.Right;

        if (start.x == goal.x && start.y > goal.y)
            return Direction.Down;

        if (start.x == goal.x && start.y < goal.y)
            return Direction.Up;

        return Direction.None;
    }

    protected Direction GetDirectionToEnemy(DungeonManager manager, int id)
    {
        return Direction.None;
    }

    protected Direction GetDirectionToNearEnemy(DungeonManager manager)
    {
        Vector3Int[] shortest_path = null;

        int current_id = manager.GetIDFromActive();

        foreach (int i in manager.GetPlayerIDS())
        {
            Vector3Int[] temp_path = manager.GetPath(manager.GetPositionFromID(current_id), manager.GetPositionFromID(i));

            Debug.Log(manager.GetPositionFromID(current_id) + ":" + manager.GetPositionFromID(i) + ":" + temp_path);

            if (shortest_path == null || shortest_path.Length > temp_path.Length)
                shortest_path = temp_path;
        }

        if (shortest_path == null || shortest_path.Length < 2)
            return Direction.None;

        return GetDirectionFromPostions(shortest_path[0], shortest_path[1]);
    }
}