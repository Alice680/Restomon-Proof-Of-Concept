using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterStats", menuName = "ScriptableObjects/Creatures/MonsterStats", order = 1)]
public class MonsterStats : ScriptableObject
{
    [Serializable]
    private class StatHolder
    {
        public int Hp, Atk, Mag, Frc, Def, Shd, Wil, Spd, Mov, Act;
    }

    [SerializeField] private StatHolder stats;
    [SerializeField] private GameObject model;
    public Monster GetMonster()
    {
        int[] core_stats = new int[] { stats.Hp, stats.Atk, stats.Mag, stats.Frc, stats.Def, stats.Shd, stats.Wil, stats.Spd, stats.Mov, stats.Act };
        return new Monster(core_stats, model);
    }
}