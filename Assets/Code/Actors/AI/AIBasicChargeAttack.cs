using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI", menuName = "ScriptableObjects/AI/Dungeons/BlindCharge")]
public class AIBasicChargeAttack : AIBase
{
    public override void Run(DungeonManager manager)
    {
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