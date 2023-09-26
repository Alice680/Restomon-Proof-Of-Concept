using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Stores all data for a specfic class as a scriptable object. Has a single method that generates a human
 * creature. 
 * 
 * Notes:
 * No data in this class can be edited at run time
 * Tammer sub classes are not in this build, all code for them is filler atm
 * Add in option for gear to change model
 */
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
        public Attack attack;
    }

    [Serializable]
    private class Tamerclass
    {
        public string subclass_name, description;
        public Trait subclass_trait;
        public Trait[] free_traits;
        public Attack attack;
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

    /*
     * Takes various ints as inputs so as to set which gear and sub classes the human will be using.
     * It uses these ints to grab data from the various options and saving them in temp variables.
     * It then creates the human from the calculated data.
     * 
     * lv int to set the level
     * s_class_i int showing which sub class to add
     * weapon_i int showing which weapon to add
     * armor_i int showing which armor to add
     * trinket_a_i int showing which trinket to add for the second slot
     * trinket_b_i int showing which trinket to add for the second slot
     * traits_i ints showing which of the unbound traits where chosen
     * @return human once everything is set
     */
    // TODO re orginize once done humans to be easier to follow and remove any fluf
    public Human GetHuman(int lv, int s_class_i, int weapon_i, int armor_i, int trinket_a_i, int trinket_b_i, int[] traits_i, string unique_name = "")
    {
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
            temp_trinket_b = trinkets[trinket_b_i];

        string temp_class_name = class_name;
        string temp_sub_name = temp_sub_class.subclass_name;
        string temp_unique_name = unique_name != "" ? unique_name : class_name;

        GameObject temp_model = model;

        Trait[] temp_traits = new Trait[10];
        temp_traits[0] = class_trait[(int)Mathf.Clamp(lv / 5, 0, 5)];
        temp_traits[1] = temp_sub_class.subclass_trait;
        temp_traits[2] = trait_list[0]; // TODO add with tammer class
        temp_traits[3] = temp_weapon.trait;
        temp_traits[4] = temp_armor.trait;
        temp_traits[5] = temp_trinket_a.trait;
        temp_traits[6] = temp_trinket_b.trait;
        
        for (int i = 0; i < 3; ++i)
        {
            if (traits_i.Length != 3 || traits_i[i] < 0)
                temp_traits[7 + i] = trait_list[0];
            else if (traits_i[i] < trait_list.Length)
                temp_traits[7 + i] = trait_list[traits_i[i]];
            else if (traits_i[i] < trait_list.Length + temp_sub_class.free_traits.Length)
                temp_traits[7 + i] = temp_sub_class.free_traits[traits_i[i]- trait_list.Length];
            else
                temp_traits[7 + i] = trait_list[0];
        }

        Attack[] temp_attacks = new Attack[9];
        temp_attacks[0] = temp_weapon.basic_attack;
        temp_attacks[1] = class_attack;
        temp_attacks[2] = temp_sub_class.attack;
        temp_attacks[3] = attack_list[0]; // TODO add with tammer class
        temp_attacks[4] = temp_weapon.special_attack;
        temp_attacks[5] = temp_armor.defensive_attack;
        temp_attacks[6] = temp_trinket_a.special_attack;
        temp_attacks[7] = temp_trinket_b.special_attack;

        int[] temp_stats = ApplyTrait.GetHumanBaseStats(temp_traits);
        for (int i = 0; i < 11; ++i)
            temp_stats[i] += base_stats.GetStats(i) + (lv * stat_growth.GetStats(i));

        return new Human(temp_unique_name, temp_class_name, temp_sub_name, lv, temp_stats, temp_attacks, temp_traits, temp_model);
    }
}