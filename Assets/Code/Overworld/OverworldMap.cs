using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMap
{
    private class Node
    {
        public int x, y;

        public bool traversable;

        public OverworldEntity entity;

        public int dungeon_layout;

        public GameObject model;

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;

            this.model = null;
        }

        public void SetTile(bool traversable, GameObject model)
        {
            if (this.model != null)
                GameObject.Destroy(this.model);

            this.traversable = traversable;
            dungeon_layout = -1;

            if (model != null)
                this.model = GameObject.Instantiate(model, new Vector3(x, y, 0), new Quaternion());
        }

        public void SetDungeon(int dungeon_layout)
        {
            this.dungeon_layout = dungeon_layout;
        }

        public void Clear()
        {
            if (model != null)
                GameObject.Destroy(model);

            traversable = false;
        }
    }

    private int x_size, y_size;

    private Node[,] map;

    public OverworldMap(int x_size, int y_size)
    {
        this.x_size = x_size;
        this.y_size = y_size;

        map = new Node[x_size, y_size];

        for (int i = 0; i < this.x_size; ++i)
            for (int e = 0; e < this.y_size; ++e)
                map[i, e] = new Node(i, e);
    }

    public bool MoveValid(Vector3Int position)
    {
        if (position.x < 0 || position.y < 0 || position.x >= x_size || position.y >= y_size)
            return false;

        if (map[position.x, position.y].entity != null)
            return false;

        return map[position.x, position.y].traversable;
    }

    public void Move(OverworldEntity entity, Vector3Int position)
    {
        if (!MoveValid(position))
            return;

        for (int i = 0; i < x_size; ++i)
            for (int e = 0; e < y_size; ++e)
                if (map[i, e].entity == entity)
                    map[i, e].entity = null;

        map[position.x, position.y].entity = entity;
        entity.UpdatePosition(position);
    }

    public void Interact(int x, int y, out int dungeonLayout)
    {
        dungeonLayout = map[x, y].dungeon_layout;
    }

    public void GetSize(out int x, out int y)
    {
        x = x_size;
        y = y_size;
    }

    public void SetTile(int x, int y, bool traversable, GameObject model, int dungeon_layout)
    {
        SetTile(x, y, traversable, model);
        SetDungeon(x, y, dungeon_layout);
    }

    public void SetTile(int x, int y, bool traversable, GameObject model)
    {
        map[x, y].SetTile(traversable, model);
    }


    public void SetDungeon(int x, int y, int dungeon_layout)
    {
        map[x, y].SetDungeon(dungeon_layout);
    }

    public void Clear()
    {
        for (int i = 0; i < x_size; ++i)
            for (int e = 0; e < y_size; ++e)
                map[i, e].Clear();
    }
}