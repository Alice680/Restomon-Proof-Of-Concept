using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RestomonFormOne", menuName = "ScriptableObjects/Creatures/Restomon/FormOne")]
public class RestomonBase : ScriptableObject
{
    [Serializable]
    private class StatHolder
    {
        public int hp, sp, mp, atk, mag, frc, def, shd, wil, spd, mov, act;

        public int GetStats(int index)
        {
            if (index < 0 || index >= 12)
                return -1;

            switch (index)
            {
                case 0:
                    return hp;
                case 1:
                    return sp;
                case 2:
                    return mp;
                case 3:
                    return atk;
                case 4:
                    return mag;
                case 5:
                    return frc;
                case 6:
                    return def;
                case 7:
                    return shd;
                case 8:
                    return wil;
                case 9:
                    return spd;
                case 10:
                    return mov;
                case 11:
                    return act;
            }

            return -1;
        }
    }

    [SerializeField] private int id;

    [SerializeField] private StatHolder base_stats;
    [SerializeField] private StatHolder growth_stats;

    [SerializeField] private Attack[] base_attack;
    [SerializeField] private Attack[] attack;

    [SerializeField] private GameObject model;

    public Restomon GetRestomon(int lv, int base_attack_id, int[] attack_id)
    {
        int[] core_stats = new int[12];

        for (int i = 0; i < 12; ++i)
            core_stats[i] = base_stats.GetStats(i) + (growth_stats.GetStats(i) * lv);

        Attack[] core_attack = new Attack[9];
        core_attack[0] = this.base_attack[base_attack_id];

        for (int i = 0; i < 4; ++i)
            core_attack[i + 1] = attack[attack_id[i]];

        return new Restomon(id, lv, core_stats, core_attack, model);
    }
}
