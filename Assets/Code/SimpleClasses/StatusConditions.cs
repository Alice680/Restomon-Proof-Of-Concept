using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Temporary method to keep track of all status condtions
 * Notes:
 * Currently being grabbed by file location, change in near future.
 */
// TODO replace with cleaner set up
[CreateAssetMenu(fileName = "Conditions", menuName = "ScriptableObjects/Creatures/Conditons")]
public class StatusConditions : ScriptableObject
{
    [Serializable]
    private class ConditionRank
    {
        public string name;
        public Trait effect;
        public GameObject model;
    }

    [Serializable]
    private class Condition
    {
        public ConditionRank[] ranks;
    }

    [SerializeField] private Condition[] condtion_list;

    public string GetName(int index, int rank)
    {
        if (index < 0 || index >= condtion_list.Length || rank < 0 || rank >= condtion_list[index].ranks.Length)
            return "";

        return condtion_list[index].ranks[rank].name;
    }

    public Trait GetTrait(int index, int rank)
    {
        if (index < 0 || index >= condtion_list.Length || rank < 0 || rank >= condtion_list[index].ranks.Length)
            return null;

        return condtion_list[index].ranks[rank].effect;
    }

    public GameObject GetModel(int index, int rank)
    {
        if (index < 0 || index >= condtion_list.Length || rank < 0 || rank >= condtion_list[index].ranks.Length)
            return null;

        return condtion_list[index].ranks[rank].model;
    }
}