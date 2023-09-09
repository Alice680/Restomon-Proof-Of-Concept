using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Monsters are the simples of the 3 core creature types. Used only by the AI. All there data is preset they do not
 * pay any cost to use moves.
 * 
 * Notes:
 * Refer to Creature class for a breakdown of how creatures function.
 */
public class Monster : Creature
{
    private string monster_name;
    private int lv;
    private int[] stats; //Hp, Atk, Mag, Frc, Def, Shd, Wil, Spd, Mov, Act
    private Trait[] traits;
    private Attack[] attacks;
    private GameObject model;

    public Monster(string monster_name, int lv, int[] stats, Trait[] traits, Attack[] attacks, GameObject model)
    {
        this.monster_name = monster_name;
        this.lv = lv;
        this.stats = stats;
        this.traits = traits;
        this.attacks = attacks;
        this.model = model;
    }

    /*
     * The following methods are all getter methods.
     */

    public override CreatureType GetCreatureType()
    {
        return CreatureType.Monster;
    }

    public override int GetLV()
    {
        return lv;
    }

    public override int GetHp()
    {
        return stats[0];
    }

    public override int GetStat(int index)
    {
        if (index < 0 || index > 8)
            return -1;

        return stats[index + 1];
    }

    public override Trait[] GetTraits()
    {
        return traits;
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