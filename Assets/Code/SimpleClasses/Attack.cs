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
// TODO finish conditions
// TODO Weather
// TODO Set tile traps
public enum AttackEffect { Damage, Healing, Buff, AddToTurn, Ailments, Condtions, Weather }
public enum Target { Self, Enemy, Ally, All, Dungeon, Tile }

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableAttack", order = 2)]
public class Attack : ScriptableObject
{
    [SerializeField] private string attack_name;
    [SerializeField] private string description;

    [SerializeField] private Element element;

    [SerializeField] private int[] cost = new int[3];

    [SerializeField] private ShapeData area;
    [SerializeField] private ShapeData target;

    [SerializeField] private GameObject model;

    [SerializeField] private AttackAffect[] affects;

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
        if (index < 0 || index > 2)
            return -1;

        return cost[index];
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

    public AttackAffect[] GetAffects()
    {
        return affects;
    }
}