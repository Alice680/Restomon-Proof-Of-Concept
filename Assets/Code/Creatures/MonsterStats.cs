using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Stores all data for a specfic Monster as a scriptable object. Has a single method that generates a monster
 * creature. 
 * 
 * Notes:
 * No data in this class can be edited at run time
 * No input is taken when generating mosnters.
 */
[CreateAssetMenu(fileName = "MonsterStats", menuName = "ScriptableObjects/Creatures/MonsterStats", order = 1)]
public class MonsterStats : ScriptableObject
{
    [Serializable]
    private class StatHolder
    {
        public int Hp, Atk, Mag, Frc, Def, Shd, Wil, Spd, Mov, Act;
    }

    [SerializeField] private string monster_name;
    [SerializeField] private int lv;
    [SerializeField] private Trait[] traits = new Trait[9];
    [SerializeField] private Attack[] attacks = new Attack[8];
    [SerializeField] private Element[] elements = new Element[3];
    [SerializeField] private StatHolder stats;
    [SerializeField] private GameObject model;

    public Monster GetMonster()
    {
        int[] core_stats = new int[] { stats.Hp, stats.Atk, stats.Mag, stats.Frc, stats.Def, stats.Shd, stats.Wil, stats.Spd, stats.Mov, stats.Act };

        return new Monster(monster_name, lv, core_stats, traits, attacks, elements, model);
    }
}