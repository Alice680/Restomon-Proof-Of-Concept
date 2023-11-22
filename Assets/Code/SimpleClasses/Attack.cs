using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Attacks are active abilies and the main way creatures interact with the game.
 * The class itself holds all the data for one. While ApplyAttack actully apllies the affect onto the game.
 * 
 * Notes:
 */
// TODO Weather
public enum AttackEffect { None, Damage, Healing, Buff, AddToTurn, ChangeCondition, Weather, TileCondtion }
public enum AttackRequirement { None, Chance }
public enum AttackTarget { Self, Enemy, Ally, All, Dungeon, Tile }

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableAttack", order = 2)]
public class Attack : ScriptableObject
{
    [Serializable]
    private class Effect
    {
        public AttackEffect type;
        public AttackTarget target;
        public int[] variables;
        public Requirement[] requirements;
    }

    [Serializable]
    private class Requirement
    {
        public AttackRequirement requirement;
        public int[] requirement_variables;
    }

    [SerializeField] private string attack_name;
    [SerializeField] private string description;

    [SerializeField] private Element element;

    [SerializeField] private int[] cost = new int[2];

    [SerializeField] private int hit_chance;

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

    public Element GetElement()
    {
        return element;
    }

    public int GetCost(int index)
    {
        if (index < 0 || index > 1)
            return -1;

        return cost[index];
    }

    public int GetHitChance()
    {
        return hit_chance;
    }

    public GameObject GetModel()
    {
        return model;
    }

    public Vector3Int[] GetArea(Vector3Int target)
    {
        return area.GetArea(target, Direction.None);
    }

    public Vector3Int[] GetTarget(Vector3Int target, Direction dir)
    {
        return this.target.GetArea(target, dir);
    }

    public int GetNumberEffects()
    {
        return effects.Length;
    }

    public AttackTarget GetTarget(int index)
    {
        return effects[index].target;
    }

    public AttackEffect GetEffect(int index, out int[] abilities)
    {
        if (index < 0 || index >= effects.Length)
        {
            abilities = null;
            return AttackEffect.None;
        }

        abilities = effects[index].variables;
        return effects[index].type;
    }

    public int GetNumberRequirement(int index)
    {
        if (index < 0 || index >= effects.Length)
        {
            return -1;
        }

        return effects[index].requirements.Length;
    }

    public AttackRequirement GetRequirement(int index_a, int index_b, out int[] requirement)
    {
        if (index_a < 0 || index_a >= effects.Length)
        {
            requirement = new int[0];
            return AttackRequirement.None;
        }
        if (index_b < 0 || index_b >= effects[index_a].requirements.Length)
        {
            requirement = new int[0];
            return AttackRequirement.None;
        }

        requirement = effects[index_a].requirements[index_b].requirement_variables;
        return effects[index_a].requirements[index_b].requirement;
    }
}