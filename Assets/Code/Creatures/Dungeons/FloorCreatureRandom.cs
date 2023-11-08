using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCreatureRandom : FloorCreature
{
    public FloorCreatureRandom()
    {

    }

    public override CreatureType GetCreatureType()
    {
        return CreatureType.Floor;
    }

    public override string ToString()
    {
        return "Floor ";
    }
}