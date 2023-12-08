using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI", menuName = "ScriptableObjects/AI/Basic/DungeonBasic")]
public class AIDungeonBasic : AIBase
{
    public override void Run(DungeonManager manager)
    {
        if (manager.GetCreatureTypeFromID(manager.GetIDFromActive()) == CreatureType.Floor)
        {
            Debug.Log("Spawn");
            SpawnRandomUnit(manager);

            manager.EndTurn();

            return;
        }

        if (manager.GetMoves() > 0)
        {
            int temp_length;
            Direction temp_dir = GetNextTarget(manager, out int temp_id, out temp_length);

            if (temp_dir != Direction.None && temp_length < 15)
            {
                manager.Move(temp_dir);
                return;
            }
            else if (temp_length >= 15)
            {
                manager.EndTurn();
            }
        }

        if (manager.GetActions() > 0)
        {
            int target_id;
            Vector3Int target = GetAttackTarget(manager, out target_id);

            if (target_id != -1)
            {
                manager.Attack(target, target_id);
                return;
            }
            else
            {
                manager.EndTurn();
            }
        }

        manager.EndTurn();
    }
}