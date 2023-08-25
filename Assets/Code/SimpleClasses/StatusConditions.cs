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
    private class Ailment
    {
        public Trait effect;
        public GameObject model;
    }

    [Serializable]
    private class Condition
    {
        public Trait[] effect;
        public GameObject[] model;
    }

    [SerializeField] private Ailment[] ailment_list;
    [SerializeField] private Condition[] condtion_list;

    public GameObject GetAilmentModel(int index)
    {
        if (index < 0 || index >= ailment_list.Length)
            return null;

        return ailment_list[index].model;
    }
}