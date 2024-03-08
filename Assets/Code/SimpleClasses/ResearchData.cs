using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Research", menuName = "ScriptableObjects/Research")]
public class ResearchData : ScriptableObject
{
    [SerializeField] private int output, quantity;
    [SerializeField] private Vector2Int[] craft_cost, research_cost;

    public int GetItems(out int quantity)
    {
        quantity = this.quantity;
        return output;
    }

    public Vector2Int[] GetCraftCost()
    {
        return craft_cost;
    }

    public Vector2Int[] GetResearchCost()
    {
        return research_cost;
    }
}