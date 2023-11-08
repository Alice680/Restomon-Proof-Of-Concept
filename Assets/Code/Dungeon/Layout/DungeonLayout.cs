using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Layout", menuName = "ScriptableObjects/Dungeons/Layout")]
public class DungeonLayout : ScriptableObject
{
    [SerializeField] private DungeonFloor[] floor_options;

    public DungeonFloor GetFloor(int index)
    {
        return floor_options[index];
    }
}