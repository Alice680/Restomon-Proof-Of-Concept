using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Catalyst", menuName = "ScriptableObjects/Catalyst")]
public class Catalyst : ScriptableObject
{
    [SerializeField] private int team_size;
    [SerializeField] private int summon_cost;
    [SerializeField] private int evolution_cost;
    [SerializeField] private int maintenance_cost;

    [SerializeField] private ShapeData area;

    public int GetTeamSize()
    {
        return team_size;
    }

    public Vector3Int[] GetArea(Vector3Int target)
    {
        return area.GetArea(target,Direction.None);
    }
}