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
     * Passives
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

    public static int DamageBoost(Trait[] traits, out int crit_rate)
    {
        int value = 0;
        crit_rate = 15;

        foreach (Trait trait in traits)
            for (int i = 0; i < trait.GetNumberAffects(); ++i)
            {
                if (trait.GetCondition(i) == TraitCondition.Passive && trait.GetAbility(i) == TraitAbility.BoostDamage)
                    value += trait.GetAbilityVariable(i)[0];

                if (trait.GetCondition(i) == TraitCondition.Passive && trait.GetAbility(i) == TraitAbility.BoostCrit)
                    crit_rate -= trait.GetAbilityVariable(i)[0];
            }

        return value;
    }

    public static int DefenseBoost(Trait[] traits)
    {
        int value = 0;

        foreach (Trait trait in traits)
            for (int i = 0; i < trait.GetNumberAffects(); ++i)
            {
                if (trait.GetCondition(i) == TraitCondition.Passive && trait.GetAbility(i) == TraitAbility.BoostDefence)
                    value += trait.GetAbilityVariable(i)[0];
            }
        return value;
    }

    /*
     * Conditions
     */
    public static void StartTurn(Unit user, Trait[] traits, DungeonManager manager)
    {
        foreach (Trait trait in traits)
        {
            for (int i = 0; i < trait.GetNumberAffects(); ++i)
            {
                if (trait.GetCondition(i) == TraitCondition.StartTurn)
                    ConditionToAbility(user, trait.GetAbility(i), manager, trait.GetAbilityVariable(i));
            }
        }
    }

    public static void EndTurn(Unit user, Trait[] traits, DungeonManager manager)
    {
        foreach (Trait trait in traits)
        {
            for (int i = 0; i < trait.GetNumberAffects(); ++i)
            {
                if (trait.GetCondition(i) == TraitCondition.EndTurn)
                    ConditionToAbility(user, trait.GetAbility(i), manager, trait.GetAbilityVariable(i));
            }
        }
    }

    public static void OnMove(Unit user, Trait[] traits, DungeonManager manager)
    {
        foreach (Trait trait in traits)
        {
            for (int i = 0; i < trait.GetNumberAffects(); ++i)
            {
                if (trait.GetCondition(i) == TraitCondition.OnMove)
                    ConditionToAbility(user, trait.GetAbility(i), manager, trait.GetAbilityVariable(i));
            }
        }
    }

    public static void OnStrike(Unit user, Unit target, Trait[] user_traits, Trait[] target_traits, DungeonManager manager, Element element, int damage)
    {
        foreach (Trait trait in user_traits)
        {
            for (int i = 0; i < trait.GetNumberAffects(); ++i)
            {
                if (trait.GetCondition(i) == TraitCondition.OnStrike)
                {
                    if (trait.GetConditionVariable(i)[0] == 0)
                        ConditionToAbility(user, trait.GetAbility(i), manager, trait.GetAbilityVariable(i));
                    else
                        ConditionToAbility(target, trait.GetAbility(i), manager, trait.GetAbilityVariable(i));
                }
            }
        }

        foreach (Trait trait in target_traits)
        {
            for (int i = 0; i < trait.GetNumberAffects(); ++i)
            {
                if (trait.GetCondition(i) == TraitCondition.OnStruck)
                {
                    if (trait.GetConditionVariable(i)[0] == 0)
                        ConditionToAbility(user, trait.GetAbility(i), manager, trait.GetAbilityVariable(i));
                    else
                        ConditionToAbility(target, trait.GetAbility(i), manager, trait.GetAbilityVariable(i));
                }
            }
        }
    }

    public static void OnKill(Unit user, Unit target, Trait[] user_traits, Trait[] target_traits, DungeonManager manager)
    {
        foreach (Trait trait in user_traits)
        {
            for (int i = 0; i < trait.GetNumberAffects(); ++i)
            {
                if (trait.GetCondition(i) == TraitCondition.OnKill && user.GetID() != target.GetID())
                {
                    if (trait.GetConditionVariable(i)[0] == 0)
                        ConditionToAbility(user, trait.GetAbility(i), manager, trait.GetAbilityVariable(i));
                    else
                        ConditionToAbility(target, trait.GetAbility(i), manager, trait.GetAbilityVariable(i));
                }
            }
        }

        foreach (Trait trait in target_traits)
        {
            for (int i = 0; i < trait.GetNumberAffects(); ++i)
            {
                if (trait.GetCondition(i) == TraitCondition.OnKilled && user.GetID() != target.GetID())
                {
                    if (trait.GetConditionVariable(i)[0] == 0)
                        ConditionToAbility(user, trait.GetAbility(i), manager, trait.GetAbilityVariable(i));
                    else
                        ConditionToAbility(target, trait.GetAbility(i), manager, trait.GetAbilityVariable(i));
                }
            }
        }
    }

    /*
     * Bridge the two
     */
    private static void ConditionToAbility(Unit target, TraitAbility ability, DungeonManager manager, int[] variables)
    {
        switch (ability)
        {
            case TraitAbility.Damage:
                Damage(target, variables[0], variables[1], variables[2]);
                break;

            case TraitAbility.Healing:
                Healing(target, variables[0], variables[1], variables[2]);
                break;

            case TraitAbility.Buff:
                Buff(target, variables[0], variables[1], variables[2]);
                return;

            case TraitAbility.AddToTurn:
                AddToTurn(manager, variables[0], variables[1], variables[2]);
                break;

            case TraitAbility.ApplyCondtions:
                Conditions(target, variables[0], variables[1], variables[2], variables[3]);
                break;

            case TraitAbility.InstantKill:
                InstantDeath(target, variables[0], variables[1]);
                break;

            case TraitAbility.Weather:
                Weather();
                break;
        }
    }

    /*
     * Ability
     */

    private static void Damage(Unit target, int type, int scale, int accuracy)
    {
        if (accuracy != -1 && accuracy < Random.Range(0, 100))
            return;

        float damage = 0;
        switch (type)
        {
            case 0:
                damage = scale;
                break;
            case 1:
                damage = Mathf.Max(1, 1.0f * target.GetMaxHP() * scale / 100);
                break;
        }

        target.ChangeHp(-(int)damage);
    }

    private static void Healing(Unit target, int type, int scale, int accuracy)
    {
        if (accuracy != -1 && accuracy < Random.Range(0, 100))
            return;

        float healing = 0;
        switch (type)
        {
            case 0:
                healing = scale;
                break;
            case 1:
                healing = Mathf.Max(1, 1.0f * target.GetMaxHP() * scale / 100);
                break;
        }

        target.ChangeHp((int)healing);
    }

    private static void Buff(Unit user, int index, int amount, int accuracy)
    {
        if (accuracy != -1 && accuracy < Random.Range(0, 100))
            return;

        user.ChangeStatRank(index, amount);
    }

    public static void AddToTurn(DungeonManager manager, int index, int change, int accuracy)
    {
        if (accuracy != -1 && accuracy < Random.Range(0, 100))
            return;

        if (index == 0)
            manager.ChangeMovement(change);

        if (index == 1)
            manager.ChangeAction(change);
    }

    public static void Conditions(Unit target, int index, int rank, int power, int accuracy)
    {

        if (accuracy != -1 && accuracy < Random.Range(0, 100))
            return;

        if (power != -1 && Mathf.Max(0, power - target.GetStat(5)) < Random.Range(0, 200))
            return;

        target.SetCondition(index, rank);
    }

    public static void InstantDeath(Unit target, int power, int accuracy)
    {
        if (accuracy != -1 && accuracy < Random.Range(0, 100))
            return;

        if (power != -1 && Mathf.Max(0, power - target.GetStat(5)) < Random.Range(0, 200))
            return;

        target.ChangeHp(-9999);
    }

    public static void Weather()
    {

    }
}