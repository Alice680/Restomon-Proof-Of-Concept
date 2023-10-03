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

        user.ChangeHp(-attack.GetCost(0));

        return true;
    }

    public static void ApplyEffect(Attack attack, Unit user_unit, Trait[] user_traits, Unit[] targets, List<Trait[]> target_traits, Vector3Int[] tiles, DungeonMap map, DungeonManager manager)
    {
        if (attack == null || user_unit == null || targets == null || tiles == null || map == null || manager == null)
            return;

        if (attack.GetHitChance() == -1 || attack.GetHitChance() + ApplyTrait.HitBoost(user_traits) + ApplyTrait.EvasionBoost(user_traits) >= Random.Range(0, 100))
            foreach (AttackAffect affect in attack.GetAffects())
                if (affect.target == Target.Self)
                    CallEffectType(attack, affect, manager, user_unit, user_traits, user_unit, user_traits);

        for (int i = 0; i < targets.Length; ++i)
        {
            if (attack.GetHitChance() != -1 && attack.GetHitChance() + ApplyTrait.HitBoost(user_traits) + ApplyTrait.EvasionBoost(target_traits[i]) < Random.Range(0, 100))
                continue;

            foreach (AttackAffect affect in attack.GetAffects())
            {
                if (affect.target == Target.Self || affect.target == Target.Dungeon || affect.target == Target.Tile)
                    continue;

                if (affect.target == Target.Enemy && user_unit.GetOwner() == targets[i].GetOwner())
                    continue;

                if (affect.target == Target.Ally && user_unit.GetOwner() != targets[i].GetOwner())
                    continue;

                CallEffectType(attack, affect, manager, user_unit, user_traits, targets[i], target_traits[i]);
            }
        }

        foreach (AttackAffect affect in attack.GetAffects())
        {
            if (affect.target == Target.Dungeon)
            {
                if (affect.type == AttackEffect.Weather)
                    Weather(user_unit, manager);
            }
            else if (affect.target == Target.Tile)
            {
                foreach (Vector3Int tile in tiles)
                    TileCondition(map, tile, affect.variables[0]);
            }
        }
    }

    private static void CallEffectType(Attack attack, AttackAffect affect, DungeonManager manager, Unit user_unit, Trait[] user_traits, Unit target_unit, Trait[] target_traits)
    {
        switch (affect.type)
        {
            case AttackEffect.Damage:
                Damage(user_unit, user_traits, target_unit, target_traits, manager, attack.GetElement(), affect.variables[0], affect.variables[1], affect.variables[2]);
                break;

            case AttackEffect.Healing:
                Heal(user_unit, user_traits, target_unit, target_traits, affect.variables[0], affect.variables[1], affect.variables[2]);
                break;

            case AttackEffect.Buff:
                Buff(user_unit, target_unit, target_traits, affect.variables[0], affect.variables[1], affect.variables[2]);
                break;

            case AttackEffect.AddToTurn:
                AddToTurn(user_unit, manager, affect.variables[0], affect.variables[1], affect.variables[2]);
                break;

            case AttackEffect.ChangeCondition:
                ChangeCondition(user_unit, user_traits, target_unit, target_traits, affect.variables[0], affect.variables[1], affect.variables[2], affect.variables[3], affect.variables[4]);
                break;
        }
    }

    /*
     * Affect Types
     */
    private static void Damage(Unit user, Trait[] user_trait, Unit target, Trait[] target_trait, DungeonManager manager, Element element, int type, int base_scale, int chance)
    {
        if (chance < Random.Range(0, 100))
            return;

        float damage = 0;
        int crit_rate;
        switch (type)
        {
            case 0:
                damage = (user.GetLV() + 3f) / 2f;

                damage *= base_scale / 50F;
                damage *= user.GetStat(0) / target.GetStat(3);
                damage *= 1 + (0.05F * user.GetElementAffinity(element));
                damage *= Mathf.Pow(2, target.GetResistance(element));
                damage *= (100f + ApplyTrait.DamageBoost(user_trait, out crit_rate) + ApplyTrait.DefenseBoost(target_trait)) / 100f;
                damage *= (Random.Range(0, crit_rate) == 0) ? 1.35f : 1f;
                damage += 1;
                break;
            case 1:
                damage = (user.GetLV() + 3f) / 2f;

                damage *= base_scale / 50F;
                damage *= user.GetStat(1) / target.GetStat(4);
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

    private static void Heal(Unit user, Trait[] user_trait, Unit target, Trait[] target_trait, int type, int scale, int chance)
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
                healing = scale / 100f * target.GetMaxHP();
                break;
        }

        DungeonTextHandler.AddText(user + " healed " + target + " for " + (int)scale);
        target.ChangeHp((int)healing);

    }

    private static void Buff(Unit user, Unit target, Trait[] target_trait, int index, int amount, int chance)
    {
        if (index < 0 || index > 9)
            return;

        if (chance < Random.Range(0, 100))
            return;

        //TODO add in text
        //DungeonTextHandler.AddText(user + " dealt " + (int)damage + " " + element + " damage to " + target);

        target.ChangeStatRank(index, amount);
    }

    private static void AddToTurn(Unit target, DungeonManager manager, int index, int change, int chance)
    {
        if (chance < Random.Range(0, 100))
            return;

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

    private static void ChangeCondition(Unit user, Trait[] user_trait, Unit target, Trait[] target_trait, int index, int current_rank, int new_rank, int power, int chance)
    {
        if (chance < Random.Range(0, 100))
            return;

        if (power != -1 && 1.0f * user.GetStat(2) / target.GetStat(5) * power < Random.Range(0, 200))
            return;

        if (current_rank == -2 || current_rank == target.GetCondition(index))
            target.SetCondition(index, new_rank);
        else if (current_rank == -3 && new_rank != target.GetCondition(index))
            target.SetCondition(index, new_rank);
        else if (current_rank == -3)
            target.SetCondition(index, -1);

        // TODO add text
    }

    private static void Weather(Unit user, DungeonManager manager)
    {
        // TODO Set Weather
    }

    private static void TileCondition(DungeonMap map, Vector3Int tile, int index)
    {
        map.SetTileTrait(tile, index);
    }
}