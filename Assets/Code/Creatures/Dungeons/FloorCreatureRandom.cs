using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCreatureRandom : FloorCreature
{
    private int spawn_rate;

    public FloorCreatureRandom(int spawn_rate)
    {
        this.spawn_rate = spawn_rate;
    }

    public override CreatureType GetCreatureType()
    {
        return CreatureType.Floor;
    }

    public override int GetStat(int index)
    {
        if (index == 6)
            return spawn_rate;

        return -1;
    }
}