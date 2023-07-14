using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Class", menuName = "ScriptableObjects/Creatures/Class")]
public class HumanClass : ScriptableObject
{
    [Serializable]
    private class StatHolder
    {
        public int hp, apr, atk, mag, frc, def, shd, wil, spd, mov, act;

        public int GetStats(int index)
        {
            if (index < 0 || index >= 12)
                return -1;

            switch (index)
            {
                case 0:
                    return hp;
                case 1:
                    return apr;
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
    private class Subclass
    {
        public string subclass_name, description;
        public Trait subclass_trait;
        public Trait[] free_traits;

        [SerializeField] private StatHolder stat_boost;

        public int GetStats(int index)
        {
            return stat_boost.GetStats(index);
        }
    }

    [Serializable]
    private class Tamerclass
    {
        public string subclass_name, description;
        public Trait subclass_trait;
        public Trait[] free_traits;
    }

    [Serializable]
    private class Weapon
    {
        public string weapon_name, description;
        public Attack basic_attack, special_attack;
        public Trait trait;
    }

    [Serializable]
    private class Armor
    {
        public string armor_name, description;
        public Attack defensive_attack;
        public Trait trait;
    }

    [Serializable]
    private class Trinket
    {
        public string trinket_name, description;
        public Attack special_attack;
        public Trait trait;
    }

    [SerializeField] private string class_name, description;
    [SerializeField] private StatHolder base_stats, stat_growth;
    [SerializeField] private Attack class_attack;
    [SerializeField] private Trait[] class_trait;
    [SerializeField] private Subclass[] subclasses;
    [SerializeField] private Tamerclass[] tamerclasses;
    [SerializeField] private Weapon[] weapons;
    [SerializeField] private Armor[] armors;
    [SerializeField] private Trinket[] trinkets;
    [SerializeField] private GameObject model;
    [SerializeField] private Attack[] attack_list;
    [SerializeField] private Trait[] trait_list;

    public Human GetHuman(int lv, int s_class_i, int weapon_i, int armor_i, int trinket_a_i, int trinket_b_i, int[] attacks_i, int[] traits_i)
    {
        //Grab Subclass and Gear
        Subclass temp_sub_class;
        if (s_class_i < 0 || s_class_i > subclasses.Length)
            temp_sub_class = subclasses[0];
        else
            temp_sub_class = subclasses[s_class_i];

        Weapon temp_weapon;
        if (weapon_i < 0 || weapon_i > weapons.Length)
            temp_weapon = weapons[0];
        else
            temp_weapon = weapons[weapon_i];

        Armor temp_armor;
        if (armor_i < 0 || armor_i > armors.Length)
            temp_armor = armors[0];
        else
            temp_armor = armors[armor_i];

        Trinket temp_trinket_a;
        if (trinket_a_i < 0 || trinket_a_i > trinkets.Length)
            temp_trinket_a = trinkets[0];
        else
            temp_trinket_a = trinkets[trinket_a_i];

        Trinket temp_trinket_b;
        if (trinket_b_i < 0 || trinket_b_i > trinkets.Length || trinket_b_i == trinket_a_i)
            temp_trinket_b = trinkets[0];
        else
            temp_trinket_b = trinkets[trinket_a_i];

        //Set Names
        string t_class_name = class_name;
        string t_sub_name = temp_sub_class.subclass_name;

        //Set Stats
        int[] t_stats = new int[11];
        for (int i = 0; i < 11; ++i)
            t_stats[i] = base_stats.GetStats(i) + (lv * stat_growth.GetStats(i)) + temp_sub_class.GetStats(i);

        //Set Attacks
        Attack[] t_attacks = new Attack[9];
        t_attacks[0] = temp_weapon.basic_attack;
        t_attacks[1] = class_attack;
        t_attacks[2] = temp_weapon.special_attack;
        t_attacks[3] = temp_armor.defensive_attack;
        t_attacks[4] = temp_trinket_a.special_attack;
        t_attacks[5] = temp_trinket_b.special_attack;

        for (int i = 0; i < 3; ++i)
        {
            if (attacks_i.Length != 3 || attacks_i[i] < 0 || attacks_i[i] >= attack_list.Length)
                t_attacks[6 + i] = attack_list[0];
            else
                t_attacks[6 + i] = attack_list[attacks_i[i]];
        }

        //Set Traits
        Trait[] t_traits = new Trait[10];
        t_traits[0] = class_trait[(int)Mathf.Clamp(lv/5, 0, 5)];
        t_traits[1] = temp_sub_class.subclass_trait;
        t_traits[2] = trait_list[0]; //Still in planinng
        t_traits[3] = temp_weapon.trait;
        t_traits[4] = temp_armor.trait;
        t_traits[5] = temp_trinket_a.trait;
        t_traits[6] = temp_trinket_b.trait;

        for (int i = 0; i < 3; ++i)
        {
            if (traits_i.Length != 3 || traits_i[i] < 0 || traits_i[i] >= attack_list.Length)
                t_traits[7 + i] = trait_list[0];
            else
               t_traits[7 + i] = trait_list[traits_i[i]];
        }

        //Set Model
        GameObject t_model = model;

        //Make Human
        return new Human(t_class_name, t_sub_name, t_stats, t_attacks, t_traits, t_model);
    }
}