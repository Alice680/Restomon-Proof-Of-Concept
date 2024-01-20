using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Layout", menuName = "ScriptableObjects/Dialogue")]
public class DialogueTree : ScriptableObject
{
    [Serializable]
    private class Node
    {
        public bool is_choice;

        public int next_node;
        public int optinal_node;

        public string speaker_name;
        public string text;
        public string choice_text;
    }

    [SerializeField] private Node[] dialouge;
    [SerializeField] private EventEffect[] event_effects;

    public int NextNode(int current_node, bool selection, out bool is_choice)
    {
        int next_node = selection ? dialouge[current_node].next_node : dialouge[current_node].optinal_node;

        if (next_node != -1)
            is_choice = dialouge[next_node].is_choice;
        else
            is_choice = false;

        return next_node;
    }

    public string GetData(int current_node, out string speaker_name, out string choice_name)
    {
        speaker_name = dialouge[current_node].speaker_name;
        choice_name = dialouge[current_node].choice_text;
        return dialouge[current_node].text;
    }

    public void RunEvent(PermDataHolder data_holder, int index)
    {
        index = -1 - index;

        event_effects[index].Apply(data_holder);
    }
}