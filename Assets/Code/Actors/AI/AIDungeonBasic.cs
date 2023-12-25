using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI", menuName = "ScriptableObjects/AI/Basic/DungeonBasic")]
public class AIDungeonBasic : AIBase
{
    public override void Run(DungeonManager manager, float last_input, out float updated_input)
    {
        if (manager.GetCreatureTypeFromID(manager.GetIDFromActive()) == CreatureType.Floor)
        {
            SpawnRandomUnit(manager);

            manager.EndTurn();

            updated_input = Time.time;

            return;
        }

        if (manager.GetActions() > 0)
        {
            Vector3Int target = GetAttackTarget(manager, out int target_id);

            if (target_id != -1)
            {
                Debug.Log(target_id + " " + target);
                manager.ShowCamera(manager.GetPositionFromID(target_id));
                manager.Attack(target, target_id);

                updated_input = Time.time;
                return;
            }
        }

        if (manager.GetMoves() > 0 || manager.GetActions() > 0)
        {
            Direction temp_dir = GetNextTarget(manager, out int temp_id, out int temp_length);

            if (temp_dir == Direction.None)
            {
                manager.EndTurn();

                updated_input = Time.time;
                return;
            }
            else if (OnScreen(manager,temp_id))
            {
                if (Time.time - last_input > 0.05f)
                {
                    manager.Move(temp_dir);
                    manager.ShowCamera(manager.GetPositionFromID(temp_id));

                    updated_input = Time.time;
                    return;
                }

                updated_input = last_input;
                return;
            }
            else
            {
                manager.Move(temp_dir);

                updated_input = last_input;
                return;
            }
        }

        manager.EndTurn();

        updated_input = last_input;
    }
}