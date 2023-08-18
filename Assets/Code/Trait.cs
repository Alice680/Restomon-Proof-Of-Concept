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
        public int[] int_a_variables;

        public TraitCondition condition;
        public int[] int_c_variables;
    }

    [SerializeField] private Effect[] effects;

    public int GetBaseStats(int index)
    {
        if (index < 0)
            return 0;

        int value = 0;

        foreach (Effect effect in effects)
            if (effect.ability == TraitAbility.BaseStats && effect.condition == TraitCondition.Passive && index < effect.int_a_variables.Length)
                value += effect.int_a_variables[index];

        return value;
    }
}