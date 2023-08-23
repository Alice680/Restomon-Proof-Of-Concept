using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A temporary class used to spawn in monsters for testing
 * 
 * Notes:
 * This class should be phazed out once I reach stage 2 of dungeons
 */
[Serializable]
public class CreatureSpawnInfo
{
    public virtual Creature GetCreature()
    {
        return null;
    }
}

[Serializable]
public class MonsterSpawnInfo : CreatureSpawnInfo
{
    [SerializeField] private MonsterStats monster_stats;

    public override Creature GetCreature()
    {
        return monster_stats.GetMonster();
    }
}