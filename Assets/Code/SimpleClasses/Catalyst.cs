using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Catalyst", menuName = "ScriptableObjects/Catalyst")]
public class Catalyst : ScriptableObject
{
    [SerializeField] private string catalyst_name, catalyst_description;

    [SerializeField] private int team_size, restomon_amount, summon_cost, evolution_cost, maintenance_cost;

    [SerializeField] private ShapeData area;

    public int GetTeamSize()
    {
        return team_size;
    }

    public int GetRestomonAmount()
    {
        return restomon_amount;
    }

    public Vector3Int[] GetArea(Vector3Int target)
    {
        return area.GetArea(target, Direction.None);
    }

    public string GetName()
    {
        return catalyst_name;
    }

    public string GetDescription()
    {
        return catalyst_description;
    }
}