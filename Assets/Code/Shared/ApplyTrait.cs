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
        int[] temp_varaibles;
        int value = 0;
        crit_rate = 15;

        foreach (Trait trait in traits)
            for (int i = 0; i < trait.GetNumberEffects(); ++i)
            {
                if (trait.GetCondition(i, out temp_varaibles) == TraitCondition.Passive && temp_varaibles[0] == 0)
                    if (trait.GetAbility(i, out temp_varaibles) == TraitAbility.BoostStats)
                        value += temp_varaibles[0];

                if (trait.GetCondition(i, out temp_varaibles) == TraitCondition.Passive && temp_varaibles[0] == 2)
                    if (trait.GetAbility(i, out temp_varaibles) == TraitAbility.BoostStats)
                        crit_rate -= temp_varaibles[0];
            }

        return value;
    }

    public static int HitBoost(Trait[] traits)
    {
        int[] temp_varaibles;
        int value = 0;

        foreach (Trait trait in traits)
            for (int i = 0; i < trait.GetNumberEffects(); ++i)
            {
                if (trait.GetCondition(i, out temp_varaibles) == TraitCondition.Passive && temp_varaibles[0] == 3)
                    if (trait.GetAbility(i, out temp_varaibles) == TraitAbility.BoostStats)
                        value += temp_varaibles[0];
            }
        return value;
    }

    public static int DefenseBoost(Trait[] traits)
    {
        int[] temp_varaibles;
        int value = 0;

        foreach (Trait trait in traits)
            for (int i = 0; i < trait.GetNumberEffects(); ++i)
            {
                if (trait.GetCondition(i, out temp_varaibles) == TraitCondition.Passive && temp_varaibles[0] == 1)
                    if (trait.GetAbility(i, out temp_varaibles) == TraitAbility.BoostStats)
                        value += temp_varaibles[0];
            }
        return value;
    }

    public static int EvasionBoost(Trait[] traits)
    {
        int[] temp_varaibles;
        int value = 0;

        foreach (Trait trait in traits)
            for (int i = 0; i < trait.GetNumberEffects(); ++i)
            {
                if (trait.GetCondition(i, out temp_varaibles) == TraitCondition.Passive && temp_varaibles[0] == 4)
                    if (trait.GetAbility(i, out temp_varaibles) == TraitAbility.BoostStats)
                        value += temp_varaibles[0];
            }
        return value;
    }

    /*
     * Conditions
     */
    public static void StartTurn(Unit user, Trait[] traits, DungeonManager manager)
    {
        int[] temp_varaibles;

        foreach (Trait trait in traits)
        {
            for (int i = 0; i < trait.GetNumberEffects(); ++i)
            {
                if (trait.GetCondition(i, out temp_varaibles) == TraitCondition.StartTurn)
                    ConditionToAbility(trait, i, user, manager);
            }
        }
    }

    public static void EndTurn(Unit user, Trait[] traits, DungeonManager manager)
    {
        int[] temp_varaibles;

        foreach (Trait trait in traits)
        {
            for (int i = 0; i < trait.GetNumberEffects(); ++i)
            {
                if (trait.GetCondition(i, out temp_varaibles) == TraitCondition.EndTurn)
                    ConditionToAbility(trait, i, user, manager);
            }
        }
    }

    public static void OnMove(Unit user, Trait[] traits, DungeonManager manager)
    {
        int[] temp_varaibles;

        foreach (Trait trait in traits)
        {
            for (int i = 0; i < trait.GetNumberEffects(); ++i)
            {
                if (trait.GetCondition(i, out temp_varaibles) == TraitCondition.OnMove)
                    ConditionToAbility(trait, i, user, manager);
            }
        }
    }

    public static void OnStrike(Unit user, Unit target, Trait[] user_traits, Trait[] target_traits, DungeonManager manager, Element element, int damage)
    {
        int[] temp_varaibles;

        foreach (Trait trait in user_traits)
        {
            for (int i = 0; i < trait.GetNumberEffects(); ++i)
            {
                if (trait.GetCondition(i, out temp_varaibles) == TraitCondition.OnStrike)
                {
                    if (temp_varaibles[0] == 0)
                        ConditionToAbility(trait, i, user, manager);
                    else
                        ConditionToAbility(trait, i, target, manager);
                }
            }
        }

        foreach (Trait trait in target_traits)
        {
            for (int i = 0; i < trait.GetNumberEffects(); ++i)
            {
                if (trait.GetCondition(i, out temp_varaibles) == TraitCondition.OnStruck)
                {
                    if (temp_varaibles[0] == 0)
                        ConditionToAbility(trait, i, user, manager);
                    else
                        ConditionToAbility(trait, i, target, manager);
                }
            }
        }
    }

    public static void OnKill(Unit user, Unit target, Trait[] user_traits, Trait[] target_traits, DungeonManager manager)
    {
        int[] temp_varaibles;

        foreach (Trait trait in user_traits)
        {
            for (int i = 0; i < trait.GetNumberEffects(); ++i)
            {
                if (trait.GetCondition(i, out temp_varaibles) == TraitCondition.OnKill && user.GetID() != target.GetID())
                {
                    if (temp_varaibles[0] == 0)
                        ConditionToAbility(trait, i, user, manager);
                    else
                        ConditionToAbility(trait, i, target, manager);
                }
            }
        }

        foreach (Trait trait in target_traits)
        {
            for (int i = 0; i < trait.GetNumberEffects(); ++i)
            {
                if (trait.GetCondition(i, out temp_varaibles) == TraitCondition.OnKilled && user.GetID() != target.GetID())
                {
                    if (temp_varaibles[0] == 0)
                        ConditionToAbility(trait, i, user, manager);
                    else
                        ConditionToAbility(trait, i, target, manager);
                }
            }
        }
    }

    public static void OnSpawn(Unit user, Trait[] target_traits, DungeonManager manager)
    {
        int[] temp_varaibles;

        foreach (Trait trait in target_traits)
        {
            for (int i = 0; i < trait.GetNumberEffects(); ++i)
            {
                if (trait.GetCondition(i, out temp_varaibles) == TraitCondition.OnSpawn)
                    ConditionToAbility(trait, i, user, manager);
            }
        }
    }

    /*
     * Bridge the two
     */
    private static void ConditionToAbility(Trait trait, int ability_num, Unit target, DungeonManager manager)
    {
        int[] temp_variables;
        for (int i = 0; i < trait.GetNumberRequirements(ability_num); ++i)
        {
            switch (trait.GetRequirement(ability_num, i, out temp_variables))
            {
                case TraitRequirement.Chance:
                    if (temp_variables[0] < Random.Range(0, 100))
                        return;
                    break;
                case TraitRequirement.Condition:
                    if (temp_variables[0] == 0)
                    {
                        if (target.GetCondition(temp_variables[1]) != temp_variables[2])
                            return;
                    }
                    break;
            }
        }

        switch (trait.GetAbility(ability_num, out temp_variables))
        {
            case TraitAbility.Damage:
                Damage(target, trait.GetName(), temp_variables[0], temp_variables[1]);
                break;

            case TraitAbility.Healing:
                Healing(target, trait.GetName(), temp_variables[0], temp_variables[1]);
                break;

            case TraitAbility.Buff:
                Buff(target, trait.GetName(), temp_variables[0], temp_variables[1]);
                return;

            case TraitAbility.AddToTurn:
                AddToTurn(target, manager, trait.GetName(), temp_variables[0], temp_variables[1]);
                break;

            case TraitAbility.ChangeCondtions:
                ChangeConditions(target, trait.GetName(), temp_variables[0], temp_variables[1], temp_variables[2]);
                break;

            case TraitAbility.InstantKill:
                InstantDeath(target, trait.GetName(), temp_variables[0]);
                break;

            case TraitAbility.Weather:
                Weather();
                break;
            case TraitAbility.Special:
                Special(target, manager, trait.GetName(), temp_variables[0]);
                break;
        }
    }

    /*
     * Test Requirements
     */

    /*
     * Ability
     */

    private static void Damage(Unit target, string trait_name, int type, int scale)
    {
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

        DungeonTextHandler.AddText(target + " took " + ((int)damage) + " damage from " + trait_name);
        target.ChangeHp(-(int)damage);
    }

    private static void Healing(Unit target, string trait_name, int type, int scale)
    {
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

    private static void Buff(Unit user, string trait_name, int index, int amount)
    {
        // TODO add text

        user.ChangeStatRank(index, amount);
    }

    private static void AddToTurn(Unit target, DungeonManager manager, string trait_name, int index, int change)
    {
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

    private static void ChangeConditions(Unit target, string trait_name, int index, int new_rank, int power)
    {
        if (power != -1 && Mathf.Max(0, power - target.GetStat(5)) < Random.Range(0, 200))
            return;

        target.SetCondition(index, new_rank);

        // TODO add text
    }

    private static void InstantDeath(Unit target, string trait_name, int power)
    {
        if (power != -1 && Mathf.Max(0, power - target.GetStat(5)) < Random.Range(0, 200))
            return;

        DungeonTextHandler.AddText(target + " was killed by " + trait_name);

        target.ChangeHp(-9999);
    }

    private static void Weather()
    {
        // TODO add weather
    }

    private static void Special(Unit target, DungeonManager manager, string trait_name, int type)
    {
        if (type == 0)
        {
            if (target.GetCreatureType() == CreatureType.Human)
                manager.WinCurrentFloor();
        }
    }
}