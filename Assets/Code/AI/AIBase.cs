using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBase : ScriptableObject
{
    public virtual int Run(DungeonManager manager, int state)
    {
        return -1;
    }
}