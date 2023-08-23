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
    [Serializable]
    private class WallSet //0-TL, 1-TC, 2-TR, 3-ML, 4-MR, 5-BL, 6-BC, 7-BR
    {
        [SerializeField] private GameObject top_left;
        [SerializeField] private GameObject top_center;
        [SerializeField] private GameObject top_right;

        [SerializeField] private GameObject middle_left;
        [SerializeField] private GameObject middle_right;

        [SerializeField] private GameObject bottom_left;
        [SerializeField] private GameObject bottom_center;
        [SerializeField] private GameObject bottom_right;

        public GameObject GetTile(int index)
        {
            switch (index)
            {
                case 0:
                    return top_left;
                case 1:
                    return top_center;
                case 2:
                    return top_right;
                case 3:
                    return middle_left;
                case 4:
                    return middle_right;
                case 5:
                    return bottom_left;
                case 6:
                    return bottom_center;
                case 7:
                    return bottom_right;
            }

            return null;
        }
    }

    [Serializable]
    private class GroundSet
    {
        public GameObject option_one;
        public GameObject option_two;
        public GameObject option_three;

        public GameObject GetTile(int index)
        {
            switch (index)
            {
                case 0:
                    return option_one;
                case 1:
                    return option_two;
                case 2:
                    return option_three;
            }

            return null;
        }

        public GameObject GetTile()
        {
            return null;
        }
    }

    [SerializeField] private WallSet out_tiles; //0-7
    [SerializeField] private WallSet in_tiles; //8-15
    [SerializeField] private GroundSet ground_tiles; //16-18 
    [SerializeField] private GameObject center_wall; //19
    [SerializeField] private GameObject lone_wall; //20

    public GameObject GetTileModel(int index)
    {
        if (index < 0)
            return null;

        if (index < 8)
            return out_tiles.GetTile(index);

        if (index < 16)
            return in_tiles.GetTile(index-8);

        if (index < 19)
            return ground_tiles.GetTile(index-16);

        if (index == 19)
            return center_wall;

        if (index == 20)
            return lone_wall;

        return null;
    }

    public DungeonTileType GetTileType(int index)
    {
        if (index < 0)
            return DungeonTileType.Empty;

        if (index < 8)
            return DungeonTileType.Wall;

        if (index < 16)
            return DungeonTileType.Wall;

        if (index < 19)
            return DungeonTileType.Ground;

        if (index == 19)
            return DungeonTileType.Wall;

        if (index == 20)
            return DungeonTileType.Wall;

        return DungeonTileType.Empty;
    }
}