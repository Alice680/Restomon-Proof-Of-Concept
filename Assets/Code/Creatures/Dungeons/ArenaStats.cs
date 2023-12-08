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
// TODO rename
public class ArenaStats : Creature
{
    private int spawn_rate;
    private int total_waves;

    public ArenaStats(int spawn_rate, int total_waves)
    {
        this.spawn_rate = spawn_rate;
        this.total_waves = total_waves;
    }

    public override CreatureType GetCreatureType()
    {
        return CreatureType.Floor;
    }

    public override int GetStat(int index)
    {
        if (index == 0)
            return total_waves;

        if (index == 6)
            return spawn_rate;

        return -1;
    }
}