using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Layout", menuName = "ScriptableObjects/Dungeons/Layout")]
public class DungeonLayout : ScriptableObject
{
    [Serializable]
    private class VictoryCondition
    {
        public int overworld_id;
        public Vector2Int position;
    }

    [SerializeField] private string dungeon_name;
    [SerializeField] private DungeonFloor[] floor_options;
    [SerializeField] private VictoryCondition lose_condition, win_condition;

    public DungeonFloor GetFloor(int index)
    {
        return floor_options[index];
    }

    public int GetNumberOfFloor()
    {
        return floor_options.Length;
    }

    public void SetVictory(PermDataHolder data, bool victory)
    {
        if (victory)
            data.SetOverworld(win_condition.overworld_id, win_condition.position);
        else
            data.SetOverworld(lose_condition.overworld_id, lose_condition.position);
    }

    public override string ToString()
    {
        return dungeon_name;
    }
}