using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Class", menuName = "ScriptableObjects/Trait")]
public class Trait : ScriptableObject
{
    [Serializable]
    private class Effect
    {
        public TraitAbility ability;
        public int[] int_variables;
        public Condition[] conditions;

        [Serializable]
        public class Condition 
        {
            public TraitCondition condition;
            public int[] int_variables;
        }

        public TraitCondition GetCondition(int index)
        {
            return conditions[index].condition;
        }

        public int GetVarible(int index_condition, int index)
        {
            return conditions[index_condition].int_variables[index];
        }
    }

    [SerializeField] private Effect[] effects;

    public int GetBaseStats(int index)
    {
        if (index < 0)
            return 0;

        int value = 0;

        foreach (Effect effect in effects)
            if ( effect.ability == TraitAbility.BaseStats && effect.GetCondition(0) == TraitCondition.Passive && index < effect.int_variables.Length)
                value += effect.int_variables[index];

        return value;
    }
}