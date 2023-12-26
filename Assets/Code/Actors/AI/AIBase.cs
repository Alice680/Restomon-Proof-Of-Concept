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
    public virtual void Run(DungeonManager manager, float last_input, out float updated_input)
    {
        updated_input = last_input;
    }

    protected int GetNearestEnemy(DungeonManager manager)
    {
        int enemy_id = -1;
        Vector3Int[] shortest_path = null;
        int current_id = manager.GetIDFromActive();

        foreach (int i in manager.GetPlayerIDS())
        {
            Vector3Int[] temp_path = manager.GetPath(manager.GetPositionFromID(current_id), manager.GetPositionFromID(i));
            if (shortest_path == null || temp_path != null || shortest_path.Length > temp_path.Length)
            {
                enemy_id = i;
                shortest_path = temp_path;
            }
        }

        return enemy_id;
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

    protected Direction GetNextTarget(DungeonManager manager, out int enemy_id, out int length)
    {
        length = -1;
        enemy_id = GetNearestEnemy(manager);
        Vector3Int[] shortest_path = manager.GetPath(manager.GetPositionFromID(manager.GetIDFromActive()), manager.GetPositionFromID(enemy_id));

        if (shortest_path == null || shortest_path.Length <= 2)
            return Direction.None;

        length = shortest_path.Length;
        return DirectionMath.GetDirectionChange(shortest_path[0], shortest_path[1]);
    }

    protected Vector3Int GetAttackTarget(DungeonManager manager, out int index, out int enemy_id)
    {
        index = -1;

        enemy_id = GetNearestEnemy(manager);

        if (enemy_id == -1)
            return new Vector3Int();

        List<int> valid_moves = new List<int>();

        for (int i = 0; i < 8; ++i)
            if (manager.AttackTargetValid(manager.GetPositionFromID(enemy_id), i))
                valid_moves.Add(i);

        if (valid_moves.Count != 0)
            index = valid_moves[Random.Range(0, valid_moves.Count)];

        return manager.GetPositionFromID(enemy_id);
    }

    protected void SpawnRandomUnit(DungeonManager manager)
    {
        Vector3Int area = new Vector3Int(-1, 1, 1);

        while (!manager.PositionValid(area) || !manager.TileEmpty(area))
            area = new Vector3Int(Random.Range(0, manager.GetMapSize().x), Random.Range(0, manager.GetMapSize().y), 0);

        manager.SpawnRandomUnit(area);
    }

    protected bool OnScreen(DungeonManager manager, int id)
    {
        Vector3Int current_postion = manager.GetPositionFromID(manager.GetIDFromActive());
        Vector3Int enemy_postion = manager.GetPositionFromID(id);

        if (Mathf.Abs(current_postion.x - enemy_postion.x) > 4)
            return false;

        if (Mathf.Abs(current_postion.y - enemy_postion.y) > 4)
            return false;

        return true;
    }
}