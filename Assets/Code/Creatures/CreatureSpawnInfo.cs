using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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