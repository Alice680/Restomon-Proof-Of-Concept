using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Layout", menuName = "ScriptableObject/Dialogue")]
public class DialogueTree : ScriptableObject
{
    [Serializable] private enum NodeType { Basic, Choice, End };

    [Serializable]
    private class Node
    {
        public NodeType node_type;

        public int next_node;
        public int optinal_node;

        public string text;
    }

    [SerializeField] private Node dialouge;
        
    public int NextNode(int current_node, bool selection = true)
    {
        return 0;
    }

    public string GetData()
    {
        return "";
    }
}