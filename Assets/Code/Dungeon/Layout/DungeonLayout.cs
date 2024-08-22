using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Layout", menuName = "ScriptableObjects/Dungeons/Layout")]
public class DungeonLayout : ScriptableObject
{
    [Serializable]
    private class TextOption
    {
        public int floor;
        public DialogueTree dialogue_tree;
        public EventTrigger trigger;
    }

    [Serializable]
    private class VictoryCondition
    {
        public int overworld_id;
        public Vector2Int position;

        public EventEffect event_effect;
    }

    [SerializeField] private string dungeon_name;
    [SerializeField] private int corruption_chance, corruption_range;
    [SerializeField] private DungeonFloor[] floor_options;
    [SerializeField] private VictoryCondition lose_condition, win_condition;
    [SerializeField] private TextOption[] text_options;

    public DungeonFloor GetFloor(int index)
    {
        return floor_options[index];
    }

    public DialogueTree GetDialogue(int index, PermDataHolder data_holder) 
    {
        foreach (TextOption option in text_options)
            if (option.floor == index && option.trigger.Check(data_holder))
                return option.dialogue_tree;

        return null;
    }

    public int GetCorruptionChance(out int corruption_range)
    {
        corruption_range = this.corruption_range;
        return corruption_chance;
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