using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature
{
    public virtual CreatureType GetCreatureType()
    {
        return CreatureType.None;
    }

    public virtual int GetHp()
    {
        return -1;
    }

    public virtual int GetStat(int index)
    {
        return -1;
    }

    public virtual Attack GetAttack(int index)
    {
        return null;
    }

    public virtual GameObject GetModel()
    {
        return null;
    }
}