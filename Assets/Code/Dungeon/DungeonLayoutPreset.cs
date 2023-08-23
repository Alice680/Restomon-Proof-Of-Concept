using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Handles dungeon with pre built layouts, but dose not handle their spawnning or win conditions.
 * 
 * Notes:
 * Is meant to be the base for both boss fights and arena
 * 
 * May merge into arena depedning on how I handle boss fights
 */

[CreateAssetMenu(fileName = "Preset", menuName = "ScriptableObjects/Dungeons/Preset")]
public class DungeonLayoutPreset : DungeonLayout
{
    [SerializeField] protected TileSetHolder tile_set;
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
                type = tile_set.GetTileType(index);
                model = tile_set.GetTileModel(index);

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

        for (int i = 0; i < x_size; ++i)
            for (int e = 0; e < y_size; ++e)
                tiles[i + (x_size * e)] = 16;
    }

    public void SetTile(int x, int y, int index)
    {
        tiles[x + (x_size * y)] = index;
    }
}