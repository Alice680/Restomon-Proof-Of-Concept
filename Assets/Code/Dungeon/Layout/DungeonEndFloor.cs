using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Random", menuName = "ScriptableObjects/Dungeons/End")]
public class DungeonEndFloor : DungeonFloor
{
    [SerializeField] protected TileSetHolder tile_set;

    public override DungeonMap GenerateDungeon(DungeonWeatherManager weather_manager, PermDataHolder data_holder, out Vector3Int start_location, out Creature[] enemies, out Vector3Int[] positions)
    {
        DungeonMap temp_map = new DungeonMap(9, 9, tile_set);

        for (int i = 0; i < 9; ++i)
            for (int e = 0; e < 9; ++e)
                temp_map.SetNode(i, e, Random.Range(14, 17));

        for (int i = 1; i < 8; ++i)
        {
            temp_map.SetNode(i, 0, 5);
            temp_map.SetNode(i, 8, 1);
        }

        for (int i = 1; i < 8; ++i)
        {
            temp_map.SetNode(0, i, 7);
            temp_map.SetNode(8, i, 3);
        }

        temp_map.SetNode(0, 0, 6);
        temp_map.SetNode(0, 8, 0);
        temp_map.SetNode(8, 0, 4);
        temp_map.SetNode(8, 8, 2);

        temp_map.SetWeatherManager(weather_manager);

        temp_map.ForceWeather(1, 0);

        temp_map.SetTileTrait(new Vector3Int(4, 6, 0), 2);

        start_location = new Vector3Int(4, 3, 0);
        enemies = new Creature[0];
        positions = new Vector3Int[0];
        return temp_map;
    }

    public override Creature GetDungeonManager()
    {
        return new FloorCreatureRandom(0);
    }

    public override AIBase GetAI()
    {
        return ai;
    }
}