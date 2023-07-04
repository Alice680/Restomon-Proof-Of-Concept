using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI", menuName = "ScriptableObjects/AI/Basic/SkipTurn")]
public class AISkipTurn : AIBase
{
    public override int Run(DungeonManager manager, int state)
    {
        manager.EndTurn();

        return 0;
    }
}