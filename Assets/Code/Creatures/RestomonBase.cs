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
        public int Hp, Sp, Mp, Atk, Mag, Frc, Def, Shd, Wil, Spd, Mov, Act;
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

        core_stats[0] = base_stats.Hp + (growth_stats.Hp * lv);
        core_stats[2] = base_stats.Sp + (growth_stats.Sp * lv);
        core_stats[1] = base_stats.Mp + (growth_stats.Mp * lv);
        core_stats[3] = base_stats.Atk + (growth_stats.Atk * lv);
        core_stats[4] = base_stats.Mag + (growth_stats.Mag * lv);
        core_stats[5] = base_stats.Frc + (growth_stats.Frc * lv);
        core_stats[6] = base_stats.Def + (growth_stats.Def * lv);
        core_stats[7] = base_stats.Shd + (growth_stats.Shd * lv);
        core_stats[8] = base_stats.Wil + (growth_stats.Wil * lv);
        core_stats[9] = base_stats.Spd + (growth_stats.Spd * lv);
        core_stats[10] = base_stats.Mov + (growth_stats.Mov * lv);
        core_stats[11] = base_stats.Act + (growth_stats.Act * lv);

        Attack[] core_attack = new Attack[9];
        core_attack[0] = this.base_attack[base_attack_id];

        for (int i = 0; i < 4; ++i)
            core_attack[i + 1] = attack[attack_id[i]];

        return new Restomon(id,lv, core_stats, core_attack, model);
    }
}
