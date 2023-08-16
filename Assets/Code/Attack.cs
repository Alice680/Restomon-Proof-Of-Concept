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
    public void ApplyEffect(Unit user, Unit[] targets, Vector3Int[] tiles, DungeonMap map)
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
                case AttackEffect.DamageAtk:
                    Damage(user, units, eff.variables[0]);
                    break;
            }
        }
    }

    private void Damage(Unit user, Unit[] targets, int scale)
    {
        foreach (Unit target in targets)
            target.ChangeHp(-user.GetStat(0));
    }
}