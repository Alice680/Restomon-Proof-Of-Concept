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
    private class DungeonPlan
    {
        public Vector2Int location;
        public int layout;
    }


    [SerializeField] private int x_size, y_size;
    [SerializeField] private Node[] nodes;
    [SerializeField] private DungeonPlan[] dungeon_layouts;

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

        foreach (DungeonPlan d_layout in dungeon_layouts)
            temp_map.SetDungeon(d_layout.location.x, d_layout.location.y, d_layout.layout);

        return temp_map;
    }
}