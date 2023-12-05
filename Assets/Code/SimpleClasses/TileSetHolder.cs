using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A class made to hold a "Tile Set", a collection of all tiles needed to generate a dungeon
 * 
 * Notes:
 * Each dungeon will be made up of one tile set.
 * It holds inner and outter walls, as well as 3 ground variants
 */
[CreateAssetMenu(fileName = "Tiles", menuName = "ScriptableObjects/Tileset")]
public class TileSetHolder : ScriptableObject
{
    [SerializeField] private GameObject[] inner_wall = new GameObject[8]; //0-7
    [SerializeField] private GameObject[] outter_wall = new GameObject[4]; //8-11
    [SerializeField] private GameObject center_wall; //12
    [SerializeField] private GameObject lone_wall; //13
    [SerializeField] private GameObject[] floor = new GameObject[3]; //14-16

    public GameObject GetTileModel(int index)
    {
        if (index < 0 || index > 16)
            return null;

        if (index < 8)
            return inner_wall[index];

        if (index < 12)
            return outter_wall[index - 8];

        if (index == 12)
            return center_wall;

        if (index == 13)
            return lone_wall;

        if (index < 17)
            return floor[index - 14];

        return null;
    }

    public DungeonTileType GetTileType(int index)
    {
        if (index < 0 || index == 13)
            return DungeonTileType.Empty;

        if (index < 13)
            return DungeonTileType.Wall;

        if (index < 17)
            return DungeonTileType.Ground;

        return DungeonTileType.Empty;
    }
}