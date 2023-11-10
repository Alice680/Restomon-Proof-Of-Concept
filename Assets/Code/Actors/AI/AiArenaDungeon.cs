using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI", menuName = "ScriptableObjects/AI/Basic/ArenaDungeon")]
public class AiArenaDungeon : AIBase
{
    public override void Run(DungeonManager manager)
    {
        if (manager.GetCreatureTypeFromID(manager.GetIDFromActive()) == CreatureType.Floor)
        {
            Debug.Log(manager.GetHP(manager.GetIDFromActive()));
            SpawnRandomUnit(manager);
            return;
        }

        int target_id = GetAttackTarget(manager, 0);

        if (target_id != -1)
        {
            if (manager.GetActions() != 0)
                manager.Attack(manager.GetPositionFromID(target_id), 0);
            else
                manager.EndTurn();
            return;
        }
        else if (manager.GetMoves() > 0)
        {
            manager.Move(GetDirectionToNearEnemy(manager));
            manager.EndTurn();
        }

        manager.EndTurn();
    }
}