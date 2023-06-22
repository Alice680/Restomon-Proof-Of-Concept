using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMap
{
    private class Node
    {
        public DungeonTileType type;
        public GameObject model;
        public Unit current_unit;

        public int x, y;

        public Node()
        {
            type = DungeonTileType.Empty;
            model = null;
            current_unit = null;
        }

        public Node(DungeonTileType type, GameObject model, int x, int y)
        {
            this.x = x;
            this.y = y;
            this.type = type;
            this.model = GameObject.Instantiate(model, new Vector3(x, y, 0), new Quaternion());
            current_unit = null;
        }

        public void Clear()
        {
            if (model != null)
                GameObject.Destroy(model);
        }
    }

    private int x_size, y_size;
    private Node[,] nodes;

    public DungeonMap(int x, int y)
    {
        x_size = x;
        y_size = y;

        nodes = new Node[x_size, y_size];

        for (int i = 0; i < x_size; ++i)
            for (int e = 0; e < y_size; ++e)
                nodes[i, e] = new Node();
    }

    public void SetNode(int x, int y, DungeonTileType type, GameObject model)
    {
        nodes[x, y].Clear();
        nodes[x, y] = new Node(type, model, x, y);
    }

    public void MoveUnit(Vector3Int vec, Unit unit) { MoveUnit(vec.x, vec.y, unit); }
    public void MoveUnit(int x, int y, Unit unit)
    {
        RemoveUnit(unit);

        nodes[x, y].current_unit = unit;
        unit.SetPosition(new Vector3Int(x, y, 0));
    }

    public void RemoveUnit(Unit unit)
    {
        Vector3Int vec = unit.GetPosition();

        if (vec.x < 0 || vec.x >= x_size || vec.y < 0 || vec.y >= y_size)
            return;

        nodes[vec.x, vec.y].current_unit = null;

        unit.SetPosition(new Vector3Int(-1, -1, 0));
    }

    public DungeonTileType GetTileType(int x, int y)
    {
        return nodes[x, y].type;
    }

    public void Clear()
    {
        for (int i = 0; i < x_size; ++i)
            for (int e = 0; e < y_size; ++e)
                nodes[i, e].Clear();
    }
}