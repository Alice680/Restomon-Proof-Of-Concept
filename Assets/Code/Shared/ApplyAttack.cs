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
//TODO Tile Effects.
public static class ApplyAttack
{
    public static bool TryPayCost(Attack attack, Unit user)
    {
        if (user.GetHp() < attack.GetCost(0))
            return false;

        user.ChangeHp(-attack.GetCost(0));

        return true;
    }

    public static void ApplyEffect(Attack attack, Unit user, Trait[] user_traits, Unit[] targets, List<Trait[]> target_traits, Vector3Int[] tiles, DungeonMap map, DungeonManager manager)
    {
        if (attack == null || user == null || targets == null || tiles == null || map == null || manager == null)
            return;


        foreach (AttackAffect affect in attack.GetAffects())
        {
            switch (affect.type)
            {
                case AttackEffect.Damage:
                    if (affect.target == Target.Self)
                        Damage(user, user_traits, user, user_traits, manager, attack.GetElement(), affect.variables[0], affect.variables[1], affect.variables[2]);
                    else
                        for (int i = 0; i < targets.Length; ++i)
                        {
                            if (affect.target == Target.Enemy && user.GetOwner() == targets[i].GetOwner())
                                continue;

                            if (affect.target == Target.Ally && user.GetOwner() != targets[i].GetOwner())
                                continue;

                            Damage(user, user_traits, targets[i], target_traits[i], manager, attack.GetElement(), affect.variables[0], affect.variables[1], affect.variables[2]);
                        }
                    break;

                case AttackEffect.Healing:
                    if (affect.target == Target.Self)
                        Heal(user, user_traits, user, user_traits, affect.variables[0], affect.variables[1], affect.variables[2]);
                    else
                        for (int i = 0; i < targets.Length; ++i)
                        {
                            if (affect.target == Target.Enemy && user.GetOwner() == targets[i].GetOwner())
                                continue;

                            if (affect.target == Target.Ally && user.GetOwner() != targets[i].GetOwner())
                                continue;

                            Heal(user, user_traits, targets[i], target_traits[i], affect.variables[0], affect.variables[1], affect.variables[2]);
                        }
                    break;

                case AttackEffect.Buff:
                    if (affect.target == Target.Self)
                        Buff(user, user_traits, affect.variables[0], affect.variables[1], affect.variables[2]);
                    else
                        for (int i = 0; i < targets.Length; ++i)
                        {
                            if (affect.target == Target.Enemy && user.GetOwner() == targets[i].GetOwner())
                                continue;

                            if (affect.target == Target.Ally && user.GetOwner() != targets[i].GetOwner())
                                continue;

                            Buff(targets[i], target_traits[i], affect.variables[0], affect.variables[1], affect.variables[2]);
                        }
                    break;

                case AttackEffect.AddToTurn:
                    AddToTurn(manager, affect.variables[0], affect.variables[1], affect.variables[2]);
                    break;

                case AttackEffect.Condtions:
                    if (affect.target == Target.Self)
                        ApplyCondition(user, user_traits, user, user_traits, affect.variables[0], affect.variables[1], affect.variables[2], affect.variables[3]);
                    else
                        for (int i = 0; i < targets.Length; ++i)
                        {
                            if (affect.target == Target.Enemy && user.GetOwner() == targets[i].GetOwner())
                                continue;

                            if (affect.target == Target.Ally && user.GetOwner() != targets[i].GetOwner())
                                continue;

                            ApplyCondition(user, user_traits, targets[i], target_traits[i], affect.variables[0], affect.variables[1], affect.variables[2], affect.variables[3]);
                        }
                    break;

                case AttackEffect.Weather:
                    Weather(user, manager);
                    break;

                case AttackEffect.TileCondtion:
                    TileCondition(user);
                    break;
            }
        }
    }

    /*
     * Affect Types
     */
    private static void Damage(Unit user, Trait[] user_trait, Unit target, Trait[] target_trait, DungeonManager manager, Element element, int type, int base_scale, int accuracy)
    {
        if (accuracy < Random.Range(0, 100))
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
        Debug.Log(damage);
        target.ChangeHp(-(int)damage);

        ApplyTrait.OnStrike(user, target, user_trait, target_trait, manager, element, (int)damage);
    }

    private static void Heal(Unit user, Trait[] user_trait, Unit target, Trait[] target_trait, int type, int scale, int accuracy)
    {
        if (accuracy < Random.Range(0, 100))
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

        target.ChangeHp((int)healing);

    }

    private static void Buff(Unit target, Trait[] target_trait, int index, int amount, int accuracy)
    {
        if (index < 0 || index > 9)
            return;

        if (accuracy < Random.Range(0, 100))
            return;

        target.ChangeStatRank(index, amount);
    }

    private static void AddToTurn(DungeonManager manager, int index, int change, int accuracy)
    {
        if (accuracy < Random.Range(0, 100))
            return;

        if (index == 0)
            manager.ChangeMovement(change);

        if (index == 1)
            manager.ChangeAction(change);
    }

    private static void ApplyCondition(Unit user, Trait[] user_trait, Unit target, Trait[] target_trait, int index, int rank, int power, int accuracy)
    {
        if (accuracy != -1 && accuracy < Random.Range(0, 100))
            return;

        if (power != -1 && 1.0f * user.GetStat(2) / target.GetStat(5) * power < Random.Range(0, 200))
            return;

        target.SetCondition(index, rank);
    }

    private static void Weather(Unit user, DungeonManager manager)
    {

    }

    private static void TileCondition(Unit user)
    {

    }
}