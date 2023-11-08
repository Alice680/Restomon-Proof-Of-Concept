using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The base script for all AIs the inherite from.
 * It also contains many methods that allow ais to do common things.
 * 
 * Notes:
 * This method is highly unfinished at this time and mostly just serves to let the game run.
 * Nearly everything in it will be eithe re worked or repalced.
 */
public class AIBase : ScriptableObject
{
    public virtual void Run(DungeonManager manager)
    {

    }

    /*
     * Bellow is a list of various methods that can be called from any AI and solve common problems
     */
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

    // TODO write GetNearestEnemy
    protected int GetNearestEnemy(DungeonManager manager)
    {
        return -1;
    }

    protected Direction GetDirectionToNearEnemy(DungeonManager manager)
    {
        Vector3Int[] shortest_path = null;

        int current_id = manager.GetIDFromActive();

        foreach (int i in manager.GetPlayerIDS())
        {
            Vector3Int[] temp_path = manager.GetPath(manager.GetPositionFromID(current_id), manager.GetPositionFromID(i));

            if (shortest_path == null || shortest_path.Length > temp_path.Length)
                shortest_path = temp_path;
        }

        if (shortest_path == null || shortest_path.Length < 2)
            return Direction.None;

        return DirectionMath.GetDirectionChange(shortest_path[0], shortest_path[1]);
    }

    protected void SpawnRandomUnit(DungeonManager manager)
    {
        Vector3Int area = new Vector3Int(-1, 1, 1);

        while (!manager.PositionValid(area) || !manager.TileEmpty(area))
            area = new Vector3Int(Random.Range(0, manager.GetMapSize().x), Random.Range(0, manager.GetMapSize().y), 0);

        manager.SpawnRandomUnit(area);

        manager.EndTurn();
    }
}