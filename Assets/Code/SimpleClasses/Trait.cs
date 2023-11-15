using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A class that stores all data for traits.
 * ApllyTrait is called and uses the data within.
 * 
 * Notes:
 * Almost every passive ability is done through traits
 * They are used for gear, status condtions, passive slots
 * Affects: base stats
 * Planned effects: buff, use attack, apply condition
 * Condtions: passive
 * Planned conditons: On hit, on attack, on kill, on killed, start turn, during weather
 */
// TODO Make interface so I don't need to relly on the list for variables
public enum TraitCondition { None, Passive, StartTurn, EndTurn, OnStrike, OnStruck, OnKill, OnKilled, OnMove, OnSpawn }
public enum TraitAbility { None, Damage, Healing, Buff, AddToTurn, ChangeCondtions,  Weather, InstantKill, BoostStats, Special }

[CreateAssetMenu(fileName = "Class", menuName = "ScriptableObjects/Trait")]
public class Trait : ScriptableObject
{
    [Serializable]
    private class Affect
    {
        public TraitAbility ability;
        public int[] ability_variables;

        public TraitCondition condition;
        public int[] condition_variables;
    }

    [SerializeField] private string trait_name, trait_description;
    [SerializeField] private Affect[] affects;

    public int GetNumberAffects()
    {
        return affects.Length;
    }

    public TraitAbility GetAbility(int index)
    {
        if (index < 0 || index >= affects.Length)
            return TraitAbility.None;

        return affects[index].ability;
    }

    public int[] GetAbilityVariable(int index)
    {
        if (index < 0 || index >= affects.Length)
            return null;

        return affects[index].ability_variables;
    }

    public TraitCondition GetCondition(int index)
    {
        if (index < 0 || index >= affects.Length)
            return TraitCondition.None;

        return affects[index].condition;
    }

    public int[] GetConditionVariable(int index)
    {
        if (index < 0 || index >= affects.Length)
            return null;

        return affects[index].condition_variables;
    }

    public String GetName()
    {
        return trait_name;
    }

    public String GetDescription()
    {
        return trait_description;
    }

    public override string ToString()
    {
        return trait_name;
    }
}