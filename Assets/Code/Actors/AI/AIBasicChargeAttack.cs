using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Charge towrds the player until you are in range then attack them.
 * 
 * Notes:
 * Checks if can hit the player and dose so if they can
 * If not, moves towrds the plyer if they can
 * 
 * This scrip will be used for basic enemies.
 */
[CreateAssetMenu(fileName = "AI", menuName = "ScriptableObjects/AI/Dungeons/BlindCharge")]
public class AIBasicChargeAttack : AIBase
{
    // TODO, rework with states so I am not checking the same info multiple times.
    public override void Run(DungeonManager manager)
    {

    }
}