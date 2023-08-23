using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Like the other creature types, it takes it stats, likely from it's constructor.
 * On top of the basics, it tracks its evolutions, two held items, a food item.
 * As well as stat points being used to vary the stat spread, though that is done RestomonBase.
 * 
 * Notes:
 * Likely the most complex of the three core creature types. It's stats mostly come from points you can freely invest.
 * As well as evolving. Which gives ups its total number of moves, stats, traits, and changes the model.
 * 
 * This class is not yet finshed, and will be in the second version of the Restomon system
 * Curretly held items, food, stat point investment, and evolutions are not added in.
 * 
 * Refer to Creature class for a breakdown of how creatures function.
 */
public class Restomon : Creature
{
    private int id, lv;
    private int[] stats; //Hp, SP, MP Atk, Mag, Frc, Def, Shd, Wil, Spd, Mov, Act
    private Trait[] traits;
    private Attack[] attacks;
    private GameObject model;

    public Restomon(int id, int lv, int[] stats, Attack[] attacks, GameObject model)
    {
        this.id = id;
        this.lv = lv;
        this.stats = stats;
        this.attacks = attacks;
        this.model = model;
    }

    /*
     * All the follow methods are just getters.
     */

    public override CreatureType GetCreatureType()
    {
        return CreatureType.Restomon;
    }

    public override int GetLV()
    {
        return lv;
    }

    public override int GetHp()
    {
        return stats[0];
    }

    public int GetSp()
    {
        return stats[1];
    }

    public int GetMp()
    {
        return stats[2];
    }

    public override int GetStat(int index)
    {
        if (index < 0 || index > 8)
            return -1;

        return stats[index + 3];
    }

    public override Trait GetTrait(int index)
    {
        if (index < 0 || index > 2)
            return null;

        return traits[index];
    }

    public override Attack GetAttack(int index)
    {
        if (index < 0 || index > 8)
            return null;

        return attacks[index];
    }

    public override GameObject GetModel()
    {
        return GameObject.Instantiate(model);
    }
}