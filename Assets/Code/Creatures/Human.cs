using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The human class is similar to a base crature, the only unqie variable it has is ather regen. Which is the amount of hp they
 * regain at the start of each of their turns.
 * 
 * Notes:
 * Most of this calsses complexity comes from it's gear, which is not saved within this class but just used at character creation.
 * It has a weapon, armor, two trinkets. As well as a sub class and a tammer class. All which gives attacks and or traits.
 * 
 * Tammer sub classes have not yet been added to this class and are not planned for this version of the game.
 * Some code is in place in case this changes though.
 * 
 * Refer to Creature class for a breakdown of how creatures function.
 */
public class Human : Creature
{
    private string class_name, sub_name, tammer_name;
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

    /*
     * The following methods all getters
     */

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

    public override Trait GetTrait(int index)
    {
        if (index < 0 || index > 8)
            return null;

        return traits[index];
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