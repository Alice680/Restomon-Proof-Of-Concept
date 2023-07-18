using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI", menuName = "ScriptableObjects/AI/Basic/SkipTurn")]
public class AISkipTurn : AIBase
{
    public override void Run(DungeonManager manager)
    {
        manager.EndTurn();
    }
}