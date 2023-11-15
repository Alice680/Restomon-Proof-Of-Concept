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
    public static int DamageBoost(Trait[] traits, out int crit_rate)
    {
        int value = 0;
        crit_rate = 15;

        foreach (Trait trait in traits)
            for (int i = 0; i < trait.GetNumberAffects(); ++i)
            {
                if (trait.GetCondition(i) == TraitCondition.Passive && trait.GetAbility(i) == TraitAbility.BoostStats && trait.GetConditionVariable(i)[0] == 0)
                    value += trait.GetAbilityVariable(i)[0];

                if (trait.GetCondition(i) == TraitCondition.Passive && trait.GetAbility(i) == TraitAbility.BoostStats && trait.GetConditionVariable(i)[0] == 2)
                    crit_rate -= trait.GetAbilityVariable(i)[0];
            }

        return value;
    }

    public static int HitBoost(Trait[] traits)
    {
        int value = 0;

        foreach (Trait trait in traits)
            for (int i = 0; i < trait.GetNumberAffects(); ++i)
            {
                if (trait.GetCondition(i) == TraitCondition.Passive && trait.GetAbility(i) == TraitAbility.BoostStats && trait.GetConditionVariable(i)[0] == 3)
                    value += trait.GetAbilityVariable(i)[0];
            }
        return value;
    }

    public static int DefenseBoost(Trait[] traits)
    {
        int value = 0;

        foreach (Trait trait in traits)
            for (int i = 0; i < trait.GetNumberAffects(); ++i)
            {
                if (trait.GetCondition(i) == TraitCondition.Passive && trait.GetAbility(i) == TraitAbility.BoostStats && trait.GetConditionVariable(i)[0] == 1)
                    value += trait.GetAbilityVariable(i)[0];
            }
        return value;
    }

    public static int EvasionBoost(Trait[] traits)
    {
        int value = 0;

        foreach (Trait trait in traits)
            for (int i = 0; i < trait.GetNumberAffects(); ++i)
            {
                if (trait.GetCondition(i) == TraitCondition.Passive && trait.GetAbility(i) == TraitAbility.BoostStats && trait.GetConditionVariable(i)[0] == 4)
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
                    ConditionToAbility(user, trait.GetAbility(i), manager, trait.ToString(), trait.GetAbilityVariable(i));
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
                    ConditionToAbility(user, trait.GetAbility(i), manager, trait.ToString(), trait.GetAbilityVariable(i));
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
                    ConditionToAbility(user, trait.GetAbility(i), manager, trait.ToString(), trait.GetAbilityVariable(i));
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
                        ConditionToAbility(user, trait.GetAbility(i), manager, trait.ToString(), trait.GetAbilityVariable(i));
                    else
                        ConditionToAbility(target, trait.GetAbility(i), manager, trait.ToString(), trait.GetAbilityVariable(i));
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
                        ConditionToAbility(user, trait.GetAbility(i), manager, trait.ToString(), trait.GetAbilityVariable(i));
                    else
                        ConditionToAbility(target, trait.GetAbility(i), manager, trait.ToString(), trait.GetAbilityVariable(i));
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
                        ConditionToAbility(user, trait.GetAbility(i), manager, trait.ToString(), trait.GetAbilityVariable(i));
                    else
                        ConditionToAbility(target, trait.GetAbility(i), manager, trait.ToString(), trait.GetAbilityVariable(i));
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
                        ConditionToAbility(user, trait.GetAbility(i), manager, trait.ToString(), trait.GetAbilityVariable(i));
                    else
                        ConditionToAbility(target, trait.GetAbility(i), manager, trait.ToString(), trait.GetAbilityVariable(i));
                }
            }
        }
    }

    public static void OnSpawn(Unit target, Trait[] target_traits, DungeonManager manager)
    {

        foreach (Trait trait in target_traits)
        {
            for (int i = 0; i < trait.GetNumberAffects(); ++i)
            {
                if (trait.GetCondition(i) == TraitCondition.OnSpawn)
                {
                    ConditionToAbility(target, trait.GetAbility(i), manager, trait.ToString(), trait.GetAbilityVariable(i));
                }
            }
        }
    }

    /*
     * Bridge the two
     */
    private static void ConditionToAbility(Unit target, TraitAbility ability, DungeonManager manager, string trait_name, int[] variables)
    {
        switch (ability)
        {
            case TraitAbility.Damage:
                Damage(target, trait_name, variables[0], variables[1], variables[2]);
                break;

            case TraitAbility.Healing:
                Healing(target, trait_name, variables[0], variables[1], variables[2]);
                break;

            case TraitAbility.Buff:
                Buff(target, trait_name, variables[0], variables[1], variables[2]);
                return;

            case TraitAbility.AddToTurn:
                AddToTurn(target, manager, trait_name, variables[0], variables[1], variables[2]);
                break;

            case TraitAbility.ChangeCondtions:
                ChangeConditions(target, trait_name, variables[0], variables[1], variables[2], variables[3], variables[4]);
                break;

            case TraitAbility.InstantKill:
                InstantDeath(target, trait_name, variables[0], variables[1]);
                break;

            case TraitAbility.Weather:
                Weather();
                break;
            case TraitAbility.Special:
                Special(target,manager, trait_name, variables[0]);
                break;
        }
    }

    /*
     * Ability
     */

    private static void Damage(Unit target, string trait_name, int type, int scale, int chance)
    {
        if (chance < Random.Range(0, 100))
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

        DungeonTextHandler.AddText(target + " took " + scale + " damage from " + trait_name);
        target.ChangeHp(-(int)damage);
    }

    private static void Healing(Unit target, string trait_name, int type, int scale, int chance)
    {
        if (chance < Random.Range(0, 100))
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

        DungeonTextHandler.AddText(target + " healed " + scale + " from " + trait_name);
        target.ChangeHp((int)healing);
    }

    private static void Buff(Unit user, string trait_name, int index, int amount, int chance)
    {
        if (chance < Random.Range(0, 100))
            return;

        // TODO add text

        user.ChangeStatRank(index, amount);
    }

    private static void AddToTurn(Unit target, DungeonManager manager, string trait_name, int index, int change, int chance)
    {
        if (chance < Random.Range(0, 100))
            return;

        if (index == 0)
        {
            DungeonTextHandler.AddText(target + " gained " + change + " movement from " + trait_name);
            manager.ChangeMovement(change);
        }

        if (index == 1)
        {
            DungeonTextHandler.AddText(target + " gained " + change + " actions from " + trait_name);
            manager.ChangeAction(change);
        }
    }

    private static void ChangeConditions(Unit target, string trait_name, int type, int current_rank, int new_rank, int power, int chance)
    {

        if (chance < Random.Range(0, 100))
            return;

        if (power != -1 && Mathf.Max(0, power - target.GetStat(5)) < Random.Range(0, 200))
            return;

        // TODO add text
        if (current_rank == -2 || current_rank == target.GetCondition(type))
            target.SetCondition(type, new_rank);
        else if (current_rank == -3 && new_rank != target.GetCondition(type))
            target.SetCondition(type, new_rank);
        else if (current_rank == -3)
            target.SetCondition(type, -1);
    }

    private static void InstantDeath(Unit target, string trait_name, int power, int chance)
    {
        if (chance < Random.Range(0, 100))
            return;

        if (power != -1 && Mathf.Max(0, power - target.GetStat(5)) < Random.Range(0, 200))
            return;

        DungeonTextHandler.AddText(target + " was killed by " + trait_name);

        target.ChangeHp(-9999);
    }

    private static void Weather()
    {
        // TODO add weather
    }

    private static void Special(Unit target,DungeonManager manager, string trait_name, int type)
    {
        if(type == 0)
        {
            if (target.GetCreatureType() == CreatureType.Human)
                manager.WinCurrentFloor();
        }
    }
}