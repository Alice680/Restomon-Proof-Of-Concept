using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Creature
{
    private int[] stats; //Hp, Atk, Mag, Frc, Def, Shd, Wil, Spd, Mov, Act
    private GameObject model;

    public Monster(int[] stats, GameObject model)
    {
        this.stats = stats;
        this.model = model;
    }

    //Get data copy
    public override CreatureType GetCreatureType()
    {
        return CreatureType.Monster;
    }

    public override int GetHp()
    {
        return stats[0];
    }

    public override int GetStat(int index)
    {
        if (index < 0 || index > 8)
            return -1;

        return stats[index - 1];
    }

    public override GameObject GetModel()
    {
        return GameObject.Instantiate(model);
    }
}