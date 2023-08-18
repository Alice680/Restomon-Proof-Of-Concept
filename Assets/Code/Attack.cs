using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableAttack", order = 2)]
public class Attack : ScriptableObject
{
    [Serializable]
    private class Effect
    {
        public AttackEffect type;
        public Target target;
        public int[] variables;
    }

    [SerializeField] private string attack_name;
    [SerializeField] private string description;

    [SerializeField] private Element element;

    [SerializeField] private int[] cost = new int[3];

    [SerializeField] private ShapeData area;
    [SerializeField] private ShapeData target;

    [SerializeField] private GameObject model;

    [SerializeField] private Effect[] effects;

    //Grab Data
    public string GetName()
    {
        return attack_name;
    }

    public string GetDescription()
    {
        return description;
    }

    public int GetCost(int index)
    {
        if (index < 0 || index > 2)
            return -1;

        return cost[index];
    }

    public GameObject GetModel()
    {
        return model;
    }

    public Vector3Int[] GetArea(Direction dir)
    {
        return area.GetArea(dir);
    }

    public Vector3Int[] GetTarget(Direction dir)
    {
        return target.GetArea(dir);
    }

    //Use attack
    public bool PayCost(Unit user)
    {
        if (user.GetHp() < cost[0])
            return false;

        user.ChangeHp(-cost[0]);

        return true;
    }

    public void ApplyEffect(Unit user, Unit[] targets, Vector3Int[] tiles, DungeonMap map, DungeonManager manager)
    {
        if (target == null)
            return;

        foreach (Effect eff in effects)
        {
            Unit[] units;

            if (eff.target == Target.Self)
                units = new Unit[1] { user };
            else
                units = targets;

            switch (eff.type)
            {
                case AttackEffect.Damage:
                    Damage(user, units, eff.variables[0], eff.variables[1], eff.variables[2]);
                    break;

                case AttackEffect.Healing:

                    break;
                case AttackEffect.Buff:

                    break;
                case AttackEffect.ChangeActive:
                    ChangeActive(manager, eff.variables[0], eff.variables[1]);
                    break;
            }
        }
    }

    //type: phys, mag, true, lv
    private void Damage(Unit user, Unit[] targets, int type, int scale, int accuracy)
    {
        foreach (Unit target in targets)
        {
            if (accuracy < UnityEngine.Random.Range(0, 100))
                continue;

            float damage = 0;
            switch (type)
            {
                case 0:
                    damage = user.GetLV() + 1f; //Base
                    damage *= scale / 50F * user.GetStat(0) / target.GetStat(3); //scale and stats
                    //Stab
                    //Effective
                    damage *= (UnityEngine.Random.Range(0, 15) == 0 ? 1.35F : 1f); //Crit
                    damage += 1; //slighly more damage
                    Debug.Log(damage);
                    break;
                case 1:

                    break;
                case 2:
                    damage = -scale;
                    break;
                case 3:
                    damage = user.GetLV() * scale;
                    break;
            }
            target.ChangeHp(-(int)damage);
        }
    }

    private void Heal(Unit user, Unit[] targets, int scale)
    {

    }

    private void Buff(Unit[] targets, int varible, int change)
    {

    }

    private void DeBuff(Unit[] targets, int varible, int change)
    {

    }

    private void ChangeActive(DungeonManager manager, int variable, int change)
    {
        if (variable == 0)
            manager.ChangeMovement(change);
        if (variable == 1)
            manager.ChangeAction(change);
    }
}