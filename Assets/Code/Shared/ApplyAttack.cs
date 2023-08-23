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
//TODO healing.
//TODO Condtions.
//TODO Weather.
//TODO Traps.
public static class ApplyAttack
{
    //Use attack
    public static bool TryPayCost(Attack attack, Unit user)
    {
        if (user.GetHp() < attack.GetCost(0))
            return false;

        user.ChangeHp(-attack.GetCost(0));

        return true;
    }

    public static void ApplyEffect(Attack attack, Unit user, Unit[] targets, Vector3Int[] tiles, DungeonMap map, DungeonManager manager)
    {
        if (attack == null || user == null || targets == null || tiles == null || map == null || manager == null)
            return;


        foreach (AttackAffect affect in attack.GetAffects())
        {
            Unit[] units;

            if (affect.target == Target.Self)
                units = new Unit[1] { user };
            else
                units = targets;

            switch (affect.type)
            {
                case AttackEffect.Damage:
                    Damage(user, units, attack.GetElement(), affect.variables[0], affect.variables[1], affect.variables[2]);
                    break;

                case AttackEffect.Healing:

                    break;

                case AttackEffect.Buff:
                    Buff(units, affect.variables[0], affect.variables[1], affect.variables[2]);
                    break;

                case AttackEffect.AddToTurn:
                    AddToTurn(manager, affect.variables[0], affect.variables[1]);
                    break;

                case AttackEffect.Ailments:
                    Ailments(user, units, affect.variables[0], affect.variables[1], affect.variables[2], affect.variables[3], affect.variables[4]);
                    break;
            }
        }
    }

    //type: phys, mag, true, lv
    private static void Damage(Unit user, Unit[] targets, Element element, int type, int base_scale, int accuracy)
    {
        foreach (Unit target in targets)
        {
            if (accuracy < Random.Range(0, 100))
                continue;

            float damage = 0;
            switch (type)
            {
                case 0:
                    damage = (user.GetLV() + 3f) / 2f;

                    damage *= base_scale / 50F;
                    damage *= user.GetStat(0) / target.GetStat(3);
                    damage *= 1 + (0.05F * user.GetElementAffinity(element));
                    damage *= Mathf.Pow(2, target.GetResistance(element));
                    damage *= (UnityEngine.Random.Range(0, 15) == 0 ? 1.35f : 1f);
                    damage += 1;

                    Debug.Log(damage);
                    break;
                case 1:
                    damage = (user.GetLV() + 3f) / 2f;

                    damage *= base_scale / 50F;
                    damage *= user.GetStat(1) / target.GetStat(4);
                    damage *= 1 + (0.05F * user.GetElementAffinity(element));
                    damage *= Mathf.Pow(2, target.GetResistance(element));
                    damage *= (UnityEngine.Random.Range(0, 15) == 0 ? 1.35f : 1f);
                    damage += 1;

                    Debug.Log(damage);
                    break;
                case 2:
                    damage = -base_scale;
                    break;
                case 3:
                    damage = user.GetLV() * base_scale / 100f;
                    break;
            }
            target.ChangeHp(-(int)damage);
        }
    }

    //TODO add healing, low priority
    private static void Heal(Unit user, Unit[] targets, int scale)
    {

    }

    private static void Buff(Unit[] targets, int index, int amount, int accuracy)
    {
        if (index < 0 || index > 9)
            return;

        foreach (Unit target in targets)
        {
            if (accuracy < UnityEngine.Random.Range(0, 100))
                continue;

            target.ChangeStatRank(index, amount);
        }
    }

    private static void AddToTurn(DungeonManager manager, int index, int change)
    {
        if (index == 0)
            manager.ChangeMovement(change);

        if (index == 1)
            manager.ChangeAction(change);
    }

    private static void Ailments(Unit user, Unit[] targets, int index, int power, int min, int max, int accuracy)
    {
        foreach (Unit target in targets)
        {
            if (accuracy == -1 || accuracy < UnityEngine.Random.Range(0, 100))
                continue;

            if (power == -1 || user.GetStat(2) / target.GetStat(5) * power < UnityEngine.Random.Range(0, 200))
                continue;

            target.ChangeAilment(index, UnityEngine.Random.Range(min, max + 1));
        }
    }
}