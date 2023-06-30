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

    [SerializeField] private int[] cost = new int[3];

    [SerializeField] private ShapeData area;
    [SerializeField] private ShapeData target;

    [SerializeField] private GameObject model;

    [SerializeField] private Effect[] effects;
    public Vector3Int[] GetArea(Direction dir)
    {
        return area.GetArea(dir);
    }

    public Vector3Int[] GetTarget(Direction dir)
    {
        return target.GetArea(dir);
    }

    public GameObject GetModel()
    {
        return model;
    }

    public void ApplyEffect(Unit user, Unit target)
    {
        if (target == null)
            return;

        foreach (Effect eff in effects)
        {
            Unit effect_target = target;

            if (eff.target == Target.Self && user.GetID() != target.GetID())
                continue;
            else if (eff.target == Target.Self)
                effect_target = user;

            switch (eff.type)
            {
                case AttackEffect.DamageAtk:
                    Damage(user, effect_target, eff.variables[0]);
                    break;
            }
        }
    }

    private void Damage(Unit user, Unit target, int scale)
    {
        target.ChangeHp(-user.GetStat(0));
    }
}