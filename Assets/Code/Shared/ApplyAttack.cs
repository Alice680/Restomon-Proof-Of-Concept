using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class handles the applying of all attacks.
 * It takes an attack as input as well as all possible entites effected by it. It then appllies each affect one by one.
 * 
 * Notes:
 * Dose not handle getting the targets. Only what to do with them once they are found.
 */
public static class ApplyAttack
{
    public static bool TryPayCost(Attack attack, Unit user)
    {
        if (user.GetHp() < attack.GetCost(0))
            return false;

        if (user.GetCreatureType() == CreatureType.Restomon)
        {
            if (user.GetMp() < attack.GetCost(1))
                return false;
            else
                user.ChangeMP(-attack.GetCost(1));
        }

        user.ChangeHp(-attack.GetCost(0));

        return true;
    }

    public static void ApplyEffect(Attack attack, Unit user_unit, Trait[] user_traits, Unit[] targets, List<Trait[]> target_traits, Vector3Int[] tiles, DungeonMap map, DungeonManager manager)
    {
        if (attack == null || user_unit == null || targets == null || tiles == null || map == null || manager == null)
            return;

        if (attack.GetHitChance() == -1 || attack.GetHitChance() + ApplyTrait.HitBoost(user_traits) + ApplyTrait.EvasionBoost(user_traits) >= Random.Range(0, 100))
        {
            for (int i = 0; i < attack.GetNumberEffects(); ++i)
                if (attack.GetTarget(i) == AttackTarget.Self)
                    CallEffectType(attack, i, manager, user_unit, user_traits, user_unit, user_traits);
        }

        for (int i = 0; i < targets.Length; ++i)
        {
            if (attack.GetHitChance() != -1 && attack.GetHitChance() + ApplyTrait.HitBoost(user_traits) + ApplyTrait.EvasionBoost(target_traits[i]) < Random.Range(0, 100))
                continue;

            for (int e = 0; e < attack.GetNumberEffects(); ++e)
            {
                AttackTarget temp_target = attack.GetTarget(e);
                if (temp_target == AttackTarget.Self || temp_target == AttackTarget.Dungeon || temp_target == AttackTarget.Tile)
                    continue;

                if (temp_target == AttackTarget.Enemy && user_unit.GetOwner() == targets[i].GetOwner())
                    continue;

                if (temp_target == AttackTarget.Ally && user_unit.GetOwner() != targets[i].GetOwner())
                    continue;

                CallEffectType(attack, e, manager, user_unit, user_traits, targets[i], target_traits[i]);
            }
        }

        for (int i = 0; i < attack.GetNumberEffects(); ++i)
        {
            int[] temp_variables;
            if (attack.GetTarget(i) == AttackTarget.Dungeon)
            {
                if (attack.GetEffect(i, out temp_variables) == AttackEffect.Weather)
                    Weather(attack, i, user_unit, map);
            }
            else if (attack.GetTarget(i) == AttackTarget.Tile)
            {
                if (attack.GetEffect(i, out temp_variables) == AttackEffect.TileCondtion)
                    foreach (Vector3Int tile in tiles)
                        TileCondition(attack, i, map, tile);
            }
        }
    }

    private static void CallEffectType(Attack attack, int effect_num, DungeonManager manager, Unit user_unit, Trait[] user_traits, Unit target_unit, Trait[] target_traits)
    {
        int[] temp_variables;
        for (int i = 0; i < attack.GetNumberRequirement(effect_num); ++i)
        {
            switch (attack.GetRequirement(effect_num, i, out temp_variables))
            {
                case AttackRequirement.Chance:
                    if (temp_variables[0] < Random.Range(0, 100))
                        return;
                    break;
                case AttackRequirement.Condition:
                    if (temp_variables[0] == 0 && user_unit.GetCondition(temp_variables[1]) != temp_variables[2])
                        return;
                    else if (temp_variables[0] == 1 && target_unit.GetCondition(temp_variables[1]) != temp_variables[2])
                        return;
                    break;
            }
        }

        switch (attack.GetEffect(effect_num, out int[] empty_variable))
        {
            case AttackEffect.Damage:
                Damage(attack, effect_num, user_unit, user_traits, target_unit, target_traits, manager);
                break;

            case AttackEffect.Healing:
                Heal(attack, effect_num, user_unit, user_traits, target_unit, target_traits);
                break;

            case AttackEffect.Buff:
                Buff(attack, effect_num, user_unit, target_unit, target_traits);
                break;

            case AttackEffect.AddToTurn:
                AddToTurn(attack, effect_num, user_unit, manager);
                break;

            case AttackEffect.ChangeCondition:
                ChangeCondition(attack, effect_num, user_unit, user_traits, target_unit, target_traits, manager);
                break;
        }
    }

    /*
     * Affect Types
     */
    private static void Damage(Attack attack, int effect_num, Unit user, Trait[] user_trait, Unit target, Trait[] target_trait, DungeonManager manager)
    {
        int[] temp_variables;

        attack.GetEffect(effect_num, out temp_variables);

        int type = temp_variables[0], base_scale = temp_variables[1];

        Element element = attack.GetElement();
        float damage = 0;
        int crit_rate;
        switch (type)
        {
            case 0:
            case 1:
                damage = (user.GetLV() + 3f) / 2f;

                damage *= base_scale / 50F;
                damage *= user.GetStat(type) / target.GetStat(3 + type);
                damage *= 1 + (0.05F * user.GetElementAffinity(element));
                damage *= Mathf.Pow(2, target.GetResistance(element));
                damage *= (100f + ApplyTrait.DamageBoost(user_trait, out crit_rate) + ApplyTrait.DefenseBoost(target_trait)) / 100f;
                damage *= (Random.Range(0, crit_rate)) == 0 ? 1.35f : 1f;
                damage += 1;
                break;
            case 2:
                damage = -base_scale;
                break;
            case 3:
                damage = user.GetLV() * base_scale / 100f;
                break;
        }

        DungeonTextHandler.AddText(user + " dealt " + (int)damage + " " + element + " damage to " + target);
        target.ChangeHp(-(int)damage);

        ApplyTrait.OnStrike(user, target, user_trait, target_trait, manager, element, (int)damage);
    }

    private static void Heal(Attack attack, int effect_num, Unit user, Trait[] user_trait, Unit target, Trait[] target_trait)
    {
        int[] temp_variables;

        attack.GetEffect(effect_num, out temp_variables);

        int type = temp_variables[0], scale = temp_variables[1];

        float healing = 0;
        switch (type)
        {
            case 0:
                healing = scale;
                break;
            case 1:
                healing = scale / 100f * target.GetMaxHP();
                break;
        }

        target.ChangeHp((int)healing);

        DungeonTextHandler.AddText(user + " healed " + target + " by " + healing);
    }

    private static void Buff(Attack attack, int effect_num, Unit user, Unit target, Trait[] target_trait)
    {
        int[] temp_variables;

        attack.GetEffect(effect_num, out temp_variables);

        int index = temp_variables[0], amount = temp_variables[1];

        if (index < 0 || index > 9)
            return;

        //TODO add in text
        //DungeonTextHandler.AddText(user + " dealt " + (int)damage + " " + element + " damage to " + target);

        target.ChangeStatRank(index, amount);
    }

    private static void AddToTurn(Attack attack, int effect_num, Unit target, DungeonManager manager)
    {
        int[] temp_variables;

        attack.GetEffect(effect_num, out temp_variables);

        int index = temp_variables[0], change = temp_variables[1];

        if (index == 0)
        {
            DungeonTextHandler.AddText(target + " regained " + change + " moves");
            manager.ChangeMovement(change);
        }

        if (index == 1)
        {
            DungeonTextHandler.AddText(target + " regained " + change + " actions");
            manager.ChangeAction(change);
        }
    }

    private static void ChangeCondition(Attack attack, int effect_num, Unit user, Trait[] user_trait, Unit target, Trait[] target_trait, DungeonManager manager)
    {
        int[] temp_variables;

        attack.GetEffect(effect_num, out temp_variables);

        int index = temp_variables[0], new_rank = temp_variables[1], power = temp_variables[2];

        if (power != -1 && 1.0f * user.GetStat(2) / target.GetStat(5) * power < Random.Range(0, 200))
            return;

        target.SetCondition(index, new_rank);

        ApplyTrait.OnConditon(user, target, user_trait, target_trait, manager, index, new_rank);

        // TODO add text
    }

    private static void Weather(Attack attack, int effect_num, Unit user, DungeonMap map)
    {
        int[] temp_variables;

        attack.GetEffect(effect_num, out temp_variables);

        int index = temp_variables[0], power = temp_variables[1];

        int temp_power = (int)(1f * user.GetStat(2) * power * Random.Range(5, 20) / 10);

        map.NewWeather(index, temp_power);
    }

    private static void TileCondition(Attack attack, int effect_num, DungeonMap map, Vector3Int tile)
    {
        int[] temp_variables;

        attack.GetEffect(effect_num, out temp_variables);

        int index = temp_variables[0];

        map.SetTileTrait(tile, index);
    }
}