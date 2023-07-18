using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaStats : Creature
{
    int spawn_rate;

    public ArenaStats(int spawn_rate)
    {
        this.spawn_rate = spawn_rate;
    }

    public override CreatureType GetCreatureType()
    {
        return CreatureType.Arena;
    }

    public override int GetStat(int index)
    {
        if (index == 6)
            return spawn_rate;

        return -1;
    }
}