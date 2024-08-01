using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Stores all data for a specfic Restomon as a scriptable object. Has a single method that generates a restomon
 * creature. 
 * 
 * Notes:
 * No data in this class can be edited at run time
 * Is not finished, needs evolutions, 3 traits, 2 held items
 * Merge base attack and attack to comply with standard
 */
[CreateAssetMenu(fileName = "RestomonFormOne", menuName = "ScriptableObjects/Creatures/Restomon/FormOne")]
public class RestomonBase : ScriptableObject
{
    [Serializable]
    private class StatHolder
    {
        public int hp, mp, atk, mag, frc, def, shd, wil, spd, mov, act;

        public int GetStats(int index)
        {
            if (index < 0 || index >= 12)
                return -1;

            switch (index)
            {
                case 0:
                    return hp;
                case 1:
                    return mp;
                case 2:
                    return atk;
                case 3:
                    return mag;
                case 4:
                    return frc;
                case 5:
                    return def;
                case 6:
                    return shd;
                case 7:
                    return wil;
                case 8:
                    return spd;
                case 9:
                    return mov;
                case 10:
                    return act;
            }

            return -1;
        }
    }

    [Serializable]
    private class EvolutionStats
    {
        public StatHolder base_stats;

        public int[] cost;

        public Attack[] attack;
        public Trait[] traits;

        public Element element;

        public GameObject model;
    }

    [SerializeField] private int id;
    [SerializeField] private string restomon_name;

    [SerializeField] private StatHolder growth_stats;

    [SerializeField] private Attack[] base_attack;

    [SerializeField] private EvolutionStats first_evo;
    [SerializeField] private EvolutionStats[] second_evos;
    [SerializeField] private EvolutionStats[] mixed_evos;
    [SerializeField] private EvolutionStats[] third_evos;

    /*
     * All data for monsters is set in in editor. As such, all that needs to be done it turning stats into an int array.
     * 
     * lv int for what level to set the creature.
     * base_attack_id int that refrences which attack is to be the main attack
     * attack_id ints that refrences which attacks are to be set
     * return Restomon after setting its data
     */
    public Restomon GetRestomon(int reforges, int[] refinements, int mutation, Vector2Int[] attack_id, int[] trait_id)
    {
        int[,] temp_stats = new int[11, 11];
        int[,] temp_cost = new int[11, 3];
        Attack[] temp_attack = new Attack[22];
        Trait[] temp_traits = new Trait[11];
        GameObject[] temp_models = new GameObject[11];

        for (int i = 0; i < 11; ++i)
            temp_stats[0, i] = first_evo.base_stats.GetStats(i) + (growth_stats.GetStats(i));

        for (int i = 0; i < 3; ++i)
            temp_cost[0, i] = first_evo.cost[i];

        temp_traits[0] = first_evo.traits[0];
        temp_traits[1] = first_evo.traits[1];

        temp_attack[0] = base_attack[attack_id[0].x];
        temp_attack[1] = base_attack[attack_id[0].y];
        temp_attack[2] = first_evo.attack[attack_id[1].x];
        temp_attack[3] = first_evo.attack[attack_id[1].y];

        temp_models[0] = first_evo.model;
        for (int I = 0; I < 3; ++I)
        {
            EvolutionStats[] temp_stats_holder;

            if (I == 0)
                temp_stats_holder = second_evos;
            else if (I == 1)
                temp_stats_holder = mixed_evos;
            else
                temp_stats_holder = third_evos;

            for (int i = 0; i < 3; ++i)
            {
                for (int e = 0; e < 11; ++e)
                    temp_stats[1 + i + (I * 3), e] = temp_stats_holder[i].base_stats.GetStats(e);

                for (int e = 0; e < 3; ++e)
                    temp_cost[1 + i + (I * 3), e] = temp_stats_holder[i].cost[e];

                for (int e = 0; e < 3; ++e)
                    temp_cost[1 + i + (I * 3), e] = temp_stats_holder[i].cost[e];

                temp_attack[4 + (i * 2) + (I * 6)] = temp_stats_holder[i].attack[attack_id[1 + i + (I * 3)].x];
                temp_attack[5 + (i * 2) + (I * 6)] = temp_stats_holder[i].attack[attack_id[1 + i + (I * 3)].y];

                temp_traits[2 + i + (I * 3)] = temp_stats_holder[i].traits[trait_id[2 + i + (I * 3)]];

                temp_models[1 + i + (I * 3)] = temp_stats_holder[i].model;
            }
        }

        return new Restomon(restomon_name, id, temp_cost, temp_stats, first_evo.traits[0], temp_traits, first_evo.attack[0], temp_attack, temp_models);
    }

    public String GetName()
    {
        return restomon_name;
    }

    public Attack[] GetBasicAttacks()
    {
        return base_attack;
    }

    public Attack[] GetAttacks(int index)
    {
        if (index < 0 || index >= 10)
            return null;

        if (index == 0)
        {
            return first_evo.attack;
        }
        else if (index < 4)
        {
            return second_evos[index - 1].attack;
        }
        else if (index < 7)
        {
            return mixed_evos[index - 4].attack;
        }
        else
        {
            return third_evos[index - 7].attack;
        }
    }

    public Trait[] GetTraits(int index)
    {
        if (index < 0 || index >= 10)
            return null;

        if (index == 0)
        {
            return first_evo.traits;
        }
        else if (index < 4)
        {
            return second_evos[index - 1].traits;
        }
        else if (index < 7)
        {
            return mixed_evos[index - 4].traits;
        }
        else
        {
            return third_evos[index - 7].traits;
        }
    }
}