using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeCost
{
    [SerializeField] private Vector2Int[] craft_cost;

    public Vector2Int[] GetCost()
    {
        return craft_cost;
    }
}
