using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Preset", menuName = "ScriptableObjects/Dungeons/Preset")]
public class DungeonLayoutPreset : DungeonLayout
{
    [Serializable]
    protected class Node
    {
        public GameObject model;
        public DungeonTileType tile_type;
    }

    [SerializeField] protected Node[] tile_types;
    [SerializeField] protected int x_size, y_size;
    [SerializeField] protected int[] tiles;

    //Run time
    public override DungeonMap GenerateDungeon()
    {
        DungeonMap map = new DungeonMap(x_size, y_size);

        DungeonTileType type = DungeonTileType.Empty;
        GameObject model = null;
        int index = 0;

        for (int i = 0; i < x_size; ++i)
            for (int e = 0; e < y_size; ++e)
            {
                index = tiles[i + (e * x_size)];
                type = tile_types[index].tile_type;
                model = tile_types[index].model;

                map.SetNode(i, e, type, model);
            }

        return map;
    }

    public override Vector3Int GetStartPosition()
    {
        return start_position;
    }

    public override AIBase GetAI()
    {
        return ai;
    }

    //Backend
    public void Setup(int x, int y)
    {
        x_size = x;
        y_size = y;

        tiles = new int[x * y];
    }

    public void SetTile(int x, int y, int index)
    {
        tiles[x + (x_size * y)] = index;
    }
}