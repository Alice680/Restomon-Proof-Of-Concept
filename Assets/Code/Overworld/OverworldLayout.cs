using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Layout", menuName = "Overworld/Layout")]
public class OverworldLayout : ScriptableObject
{
    [Serializable]
    private class Node
    {
        public GameObject model;
        public bool traversable;

        public Node(GameObject model, bool traversable)
        {
            this.model = model;
            this.traversable = traversable;
        }
    }

    [Serializable]
    private class VariantNode
    {
        public Vector2Int location;

        public GameObject model;
        public bool traversable;
        public int dungeon_layout;

        public EventTrigger event_trigger;
    }

    [Serializable]
    private class TownNode
    {
        public Vector2Int location;

        public OverworldTown town;
    }

    [Serializable]
    private class StartDialogue
    {
        public DialogueTree dialogue;
        public EventTrigger trigger;
    }

    [SerializeField] private int x_size, y_size;
    [SerializeField] private Node[] nodes;
    [SerializeField] private VariantNode[] variant_nodes;
    [SerializeField] private TownNode[] town_nodes;
    [SerializeField] private StartDialogue[] start_dialogues;

    public void Setup(int x_size, int y_size, GameObject model)
    {
        this.x_size = x_size;
        this.y_size = y_size;

        nodes = new Node[this.x_size * this.y_size];

        for (int i = 0; i < this.x_size; ++i)
            for (int e = 0; e < this.y_size; ++e)
                nodes[i + (e * this.x_size)] = new Node(model, true);
    }

    public void SetTile(int x, int y, GameObject model, bool traversable)
    {
        nodes[x + (y * x_size)].model = model;
        nodes[x + (y * x_size)].traversable = traversable;
    }

    public OverworldMap GetMap()
    {
        OverworldMap temp_map = new OverworldMap(x_size, y_size);

        for (int i = 0; i < x_size; ++i)
            for (int e = 0; e < y_size; ++e)
                temp_map.SetTile(i, e, nodes[i + (e * x_size)].traversable, nodes[i + (e * this.x_size)].model);

        return temp_map;
    }

    public OverworldMap GetMap(PermDataHolder dataHolder, out DialogueTree dialogue)
    {
        OverworldMap temp_map = GetMap();

        dialogue = null;

        foreach (VariantNode node in variant_nodes)
            if (node.event_trigger.Check(dataHolder))
                temp_map.SetTile(node.location.x, node.location.y, node.traversable, node.model, node.dungeon_layout);

        foreach (TownNode node in town_nodes)
            temp_map.SetTown(node.location.x, node.location.y, node.town);

        foreach (StartDialogue start_dialogue in start_dialogues)
            if (start_dialogue.trigger.Check(dataHolder))
                dialogue = start_dialogue.dialogue;

        return temp_map;
    }
}