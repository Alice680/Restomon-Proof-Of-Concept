using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Creature
{
    private string class_name, sub_name;
    private int lv;
    private int[] stats;
    private Attack[] attacks;
    private Trait[] traits;
    private GameObject model;

    public Human(string class_name, string sub_name, int lv,int[] stats, Attack[] attacks, Trait[] traits, GameObject model)
    {
        this.class_name = class_name;
        this.sub_name = sub_name;
        this.lv = lv;
        this.stats = stats;
        this.attacks = attacks;
        this.traits = traits;
        this.model = model;
    }

    public override CreatureType GetCreatureType()
    {
        return CreatureType.Human;
    }

    public override int GetLV()
    {
        return lv;
    }

    public override int GetHp()
    {
        return stats[0];
    }

    public int GetApr()
    {
        return stats[1];
    }

    public override int GetStat(int index)
    {
        if (index < 0 || index > 8)
            return -1;

        return stats[index + 2];
    }

    public override Attack GetAttack(int index)
    {
        if (index < 0 || index >= 9)
            return null;

        return attacks[index];
    }

    public override GameObject GetModel()
    {
        return GameObject.Instantiate(model);
    }
}