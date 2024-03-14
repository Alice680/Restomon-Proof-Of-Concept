using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gear", menuName = "ScriptableObjects/Gear")]
public class GearData : ScriptableObject
{
    [SerializeField] private int class_refrence, slot_refrence;
    [SerializeField] private Vector2Int[] craft_cost;

    public int GetData(out int other_data)
    {
        other_data = slot_refrence;
        return class_refrence;
    }

    public Vector2Int[] GetCost()
    {
        return craft_cost;
    }
}