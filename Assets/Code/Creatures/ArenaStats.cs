using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A temporary class that is tasks with spawning units in. This was created as a temporary way of handling this
 * until the next major dungeon update. At which point make sure to either re write this or replace it.
 * 
 * Notes:
 * Phaze this out when you have the chance.
 * It exists outside the map so it can not be hit by attacks. If you make an attack that ignore palcement, remeber this!
 */
// TODO remove
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