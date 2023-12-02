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
        public string form_name;

        public StatHolder base_stats;

        public int[] cost;

        public Attack[] attack;
        public Trait[] traits;

        public Element element;

        public GameObject model;
    }

    [SerializeField] private int id;

    [SerializeField] private StatHolder growth_stats;

    [SerializeField] private Attack[] base_attack;

    [SerializeField] private EvolutionStats first_evo;
    [SerializeField] private EvolutionStats[] second_evos;

    /*
     * All data for monsters is set in in editor. As such, all that needs to be done it turning stats into an int array.
     * 
     * lv int for what level to set the creature.
     * base_attack_id int that refrences which attack is to be the main attack
     * attack_id ints that refrences which attacks are to be set
     * return Restomon after setting its data
     */
    // TODO add traits
    public Restomon GetRestomon(int lv, int[] attack_id, int[] trait_id)
    {
        int[,] temp_stats = new int[4, 11];
        int[,] temp_cost = new int[4, 3];
        Attack[] temp_attack = new Attack[10];
        Trait[] temp_traits = new Trait[4];
        GameObject[] temp_models = new GameObject[4];

        for (int i = 0; i < 11; ++i)
            temp_stats[0, i] = first_evo.base_stats.GetStats(i) + (growth_stats.GetStats(i) * lv);

        for (int i = 0; i < 3; ++i)
            temp_cost[0, i] = first_evo.cost[i];

        temp_traits[0] = first_evo.traits[0];

        temp_attack[0] = base_attack[attack_id[0]];

        for (int i = 0; i < 3; ++i)
            temp_attack[i + 1] = first_evo.attack[attack_id[i + 1]];

        temp_models[0] = first_evo.model;

        for (int i = 0; i < 3; ++i)
        {
            for (int e = 0; e < 11; ++e)
                temp_stats[1 + i, e] = second_evos[i].base_stats.GetStats(e);

            for (int e = 0; e < 3; ++e)
                temp_cost[i + 1, e] = second_evos[i].cost[e];

            for (int e = 0; e < 3; ++e)
                temp_cost[1 + i, e] = second_evos[i].cost[e];

            temp_attack[4 + (i * 2)] = second_evos[i].attack[attack_id[4 + (i * 2)]];
            temp_attack[5 + (i * 2)] = second_evos[i].attack[attack_id[5 + (i * 2)]];

            temp_traits[1 + i] = second_evos[i].traits[trait_id[1 + i]];

            temp_models[i + 1] = second_evos[i].model;
        }

        return new Restomon(first_evo.form_name, id, lv, temp_cost, temp_stats, first_evo.traits[0], temp_traits, first_evo.attack[0], temp_attack, temp_models);
    }
}