using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICore : Actor
{
    private AIBase ai;

    private DungeonManager manager_ref;

    public AICore(AIBase ai, DungeonManager manager_ref)
    {
        this.ai = ai;
        this.manager_ref = manager_ref;
    }

    public override void Run()
    {
        if (ai.PreRun(manager_ref))
            ai.Run(manager_ref, 0);
    }
}