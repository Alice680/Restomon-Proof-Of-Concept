using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICore : Actor
{
    private AIBase ai;

    private DungeonManager manager_ref;

    private float last_input;

    public AICore(AIBase ai, DungeonManager manager_ref)
    {
        this.ai = ai;
        this.manager_ref = manager_ref;
    }

    public override void Run()
    {
        if (Time.time - last_input < 0.1F)
            return;

        last_input = Time.time;

        if (ai.PreRun(manager_ref))
            ai.Run(manager_ref);
    }
}