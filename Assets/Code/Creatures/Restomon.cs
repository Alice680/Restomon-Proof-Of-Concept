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
public enum RestomonEvolution { None, FormA, FormB, FormC, FormAB, FormBA, FormBC, FormCB, FormAC, FormCA, FormA2, FormB2, FormC2 };

public class Restomon : Creature
{
    private string restomon_name, restomon_description;
    private int id;
    private int[,] cost;
    private int[,] stats; //Hp, SP, MP Atk, Mag, Frc, Def, Shd, Wil, Spd, Mov, Act
    private Attack empty_attack;
    private Attack[] attacks;
    private Trait empty_trait;
    private Trait[] traits;
    private GameObject[] model;

    public Restomon(string restomon_name, int id,  int[,] cost, int[,] stats, Trait empty_trait,Trait[] traits, Attack empty_attack, Attack[] attacks, GameObject[] model)
    {
        this.restomon_name = restomon_name;
        this.id = id;
        this.cost = cost;
        this.stats = stats;
        this.empty_attack = empty_attack;
        this.attacks = attacks;
        this.empty_trait = empty_trait;
        this.traits = traits;
        this.model = model;
    }

    /*
     * All the follow methods are just getters.
     */

    public string GetName()
    {
        return restomon_name;
    }

    public override int GetID()
    {
        return id;
    }

    public string GetDescription()
    {
        return restomon_description;
    }

    public override CreatureType GetCreatureType()
    {
        return CreatureType.Restomon;
    }

    public int GetSummonCost(RestomonEvolution current_evolution, int new_form)
    {
        //Temp
        if (current_evolution != RestomonEvolution.None)
            return -1;

        if (new_form == -1)
            return cost[0, 0];

        return cost[new_form + 1, 0];
    }

    public int GetMaintenanceCost(RestomonEvolution current_evolution)
    {
        if (current_evolution != RestomonEvolution.None)
            return cost[0, 1] + cost[(int)current_evolution, 1];
        else
            return cost[0, 1];
    }

    public int GetUpkeepCost(RestomonEvolution current_evolution)
    {
        if (current_evolution != RestomonEvolution.None)
            return cost[0, 2] + cost[(int)current_evolution, 2];
        else
            return cost[0, 2];
    }

    public override int GetHp()
    {
        return stats[0, 0];
    }

    public int GetMp()
    {
        return stats[0, 1];
    }

    public int GetStat(int index, RestomonEvolution current_evolution)
    {
        if (index < 0 || index > 8)
            return -1;

        if (current_evolution != RestomonEvolution.None)
            return stats[0, index + 2] + stats[(int)current_evolution, index + 2];
        else
            return stats[0, index + 2];
    }

    public Attack GetAttack(int index, RestomonEvolution current_evolution)
    {
        if (index < 0 || index > 8)
            return null;

        if (index < 4)
            return attacks[index];
        else if (current_evolution != RestomonEvolution.None && (index == 4 || index == 5))
            return attacks[index + (((int)current_evolution - 1) * 2)];
        else if ((index == 4 || index == 5))
            return empty_attack;
        else
            return empty_attack;
    }

    public Trait[] GetTraits(RestomonEvolution current_evolution)
    {
        Trait[] temp_traits = new Trait[4];

        temp_traits[0] = traits[0];
        /*temp_traits[1] = traits[1];

        if (current_evolution == RestomonEvolution.None)
            temp_traits[2] = empty_trait;
        else
            temp_traits[2] = traits[(int)current_evolution + 1];
        */

        // TODO add in
        temp_traits[0] = traits[0];
        temp_traits[1] = traits[0];
        temp_traits[2] = traits[0];
        temp_traits[3] = traits[0];

        return traits;
    }

    public GameObject GetModel(RestomonEvolution current_evolution)
    {
        if (current_evolution == RestomonEvolution.None)
            return GameObject.Instantiate(model[0]);

        return GameObject.Instantiate(model[(int)current_evolution]);
    }

    public override string ToString()
    {
        return restomon_name;
    }
}