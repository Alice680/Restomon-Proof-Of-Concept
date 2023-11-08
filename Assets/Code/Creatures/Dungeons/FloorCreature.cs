using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCreature : Creature
{
    private Creature[] spawn_list;

    public FloorCreature()
    {

    }

    public override CreatureType GetCreatureType()
    {
        return CreatureType.Floor;
    }
}