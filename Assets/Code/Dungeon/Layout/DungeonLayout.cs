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

        public EventEffect event_effect;
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
        VictoryCondition temp_victory;

        if (victory)
            temp_victory = win_condition;
        else
            temp_victory = lose_condition;

        data.SetOverworld(temp_victory.overworld_id, temp_victory.position);

        temp_victory.event_effect.Apply(data);
    }

    public override string ToString()
    {
        return dungeon_name;
    }
}