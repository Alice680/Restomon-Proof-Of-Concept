using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Actor run by ai. They work by holding an AIBase which is a scriptable object with contains a set of instructions.
 * As such, no information can be saved within a AIBase.
 * 
 * Notes:
 * Make sure to always give it an AIBase. This is a scriptible object with the commands for how this ai should run.
 * The pre run method is a temp method that exisits atm as proper states are not yet in AIBase as it is a work in progress still.
 */

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

        ai.Run(manager_ref);
    }
}