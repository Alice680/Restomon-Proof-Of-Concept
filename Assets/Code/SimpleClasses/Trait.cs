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
public enum TraitRequirement { None, Chance, Condition }

[CreateAssetMenu(fileName = "Class", menuName = "ScriptableObjects/Trait")]
public class Trait : ScriptableObject
{
    [Serializable]
    private class Effect
    {
        public TraitAbility ability;
        public int[] ability_variables;

        public TraitCondition condition;
        public int[] condition_variables;

        public Requirement[] requirements;
    }

    [Serializable]
    private class Requirement
    {
        public TraitRequirement requirement;
        public int[] requirement_variables;
    }

    [SerializeField] private string trait_name, trait_description;
    [SerializeField] private Effect[] effects;

    public String GetName()
    {
        return trait_name;
    }

    public String GetDescription()
    {
        return trait_description;
    }

    public int GetNumberEffects()
    {
        return effects.Length;
    }

    public int GetNumberRequirements(int index)
    {
        return effects[index].requirements.Length;
    }

    public TraitAbility GetAbility(int index, out int[] abilities)
    {
        if (index < 0 || index >= effects.Length)
        {
            abilities = new int[0];
            return TraitAbility.None;
        }

        abilities = effects[index].ability_variables;
        return effects[index].ability;
    }

    public TraitCondition GetCondition(int index, out int[] conditons)
    {
        if (index < 0 || index >= effects.Length)
        {
            conditons = new int[0];
            return TraitCondition.None;
        }

        conditons = effects[index].condition_variables;

        return effects[index].condition;
    }

    public TraitRequirement GetRequirement(int index_a, int index_b, out int[] requirement)
    {
        if (index_a < 0 || index_a >= effects.Length)
        {
            requirement = new int[0];
            return TraitRequirement.None;
        }
        if (effects[index_a].requirements.Length < 0 || index_b >= effects[index_a].requirements.Length)
        {
            requirement = new int[0];
            return TraitRequirement.None;
        }

        requirement = effects[index_a].requirements[index_b].requirement_variables;
        return effects[index_a].requirements[index_b].requirement;
    }

    public override string ToString()
    {
        return trait_name;
    }
}