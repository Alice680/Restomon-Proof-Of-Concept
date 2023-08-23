using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Gets called during various situations to trigger affects.
 * Works by calling a conditinal method and checking which traits apply.
 * It then calls then call affect methods for traits which conditions arae met
 * 
 * Notes:
 * Only stats are in atm and they are a special trait
 */
// TODO set up generic interface
public static class ApplyTrait
{
    /* 
     * Special calls
     */
    public static int[] GetHumanBaseStats(Trait[] traits)
    {
        int[] stats = new int[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] temp_stats;

        foreach (Trait trait in traits)
        {
            for (int i = 0; i < trait.GetNumberAffects(); ++i)
            {
                if (trait.GetAbility(i) != TraitAbility.BaseStats || trait.GetCondition(i) != TraitCondition.Passive)
                    continue;

                temp_stats = trait.GetAbilityVariable(i);
                for (int e = 0; e < 11; ++e)
                    stats[e] += temp_stats[e];
            }
        }

        return stats;
    }

    // TODO add in
    public static int[] GetRestomonBaseStats(Trait[] traits)
    {
        int[] stats = new int[0];

        /*foreach (Effect effect in effects)
            if (effect.ability == TraitAbility.BaseStats && effect.condition == TraitCondition.Passive && index < effect.int_a_variables.Length)
                value += effect.int_a_variables[index];*/

        return stats;
    }

    // TODO add in
    public static int[] GetMonsterBaseStats(Trait[] traits)
    {
        int[] stats = new int[0];

        /*foreach (Effect effect in effects)
            if (effect.ability == TraitAbility.BaseStats && effect.condition == TraitCondition.Passive && index < effect.int_a_variables.Length)
                value += effect.int_a_variables[index];*/

        return stats;
    }

    //TODO set up trait interface
    /*
     * Conditions
     */

    /*
     * Affects
     */
}