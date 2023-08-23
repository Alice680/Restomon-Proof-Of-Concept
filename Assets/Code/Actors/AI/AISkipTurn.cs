using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A simple AI that just ends it's turn as soon as it gains controll.
 * 
 * Notes:
 * Mostly used as a filler AI
 */
[CreateAssetMenu(fileName = "AI", menuName = "ScriptableObjects/AI/Basic/SkipTurn")]
public class AISkipTurn : AIBase
{
    public override void Run(DungeonManager manager)
    {
        manager.EndTurn();
    }
}