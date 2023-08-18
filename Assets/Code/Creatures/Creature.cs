using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature
{
    public virtual CreatureType GetCreatureType()
    {
        return CreatureType.None;
    }

    public void GetElements(out Element elm_one, out Element elm_two, out Element elm_three)
    {
        elm_one = Element.None;
        elm_two = Element.None;
        elm_three = Element.None;
    }

    public virtual int GetLV()
    {
        return -1;
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