using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Random", menuName = "ScriptableObjects/Dungeons/Random")]
public class DungeonFloorRandom : DungeonFloor
{
    [SerializeField] protected TileSetHolder tile_set;
    [SerializeField] protected int x_size, y_size, min_rooms, max_room, room_min_size, room_max_size;
    [SerializeField] protected MonsterStats[] monsters;
    [SerializeField] protected int weather_type, weather_power;

    protected class Room
    {
        public int x_point, x_size, y_point, y_size;

        public Room(int x_limit, int y_limit, int min_size, int max_size)
        {
            x_size = Random.Range(min_size, max_size);
            y_size = Random.Range(min_size, max_size);

            x_point = Random.Range(1, x_limit - x_size - 1);
            y_point = Random.Range(1, y_limit - y_size - 1);
        }

        public bool IsWithin(int x, int y)
        {
            if (x < x_point || x > x_point + x_size)
                return false;

            if (y < y_point || y > y_point + y_size)
                return false;

            return true;
        }

        public bool RoomsOverlap(Room other)
        {
            if (x_point - 2 > other.x_point + other.x_size)
                return false;

            if (x_point + x_size + 2 < other.x_point)
                return false;

            if (y_point - 2 > other.y_point + other.y_size)
                return false;

            if (y_point + y_size + 2 < other.y_point)
                return false;

            return true;
        }

        public int RoomsRelation(Room other)
        {
            /*if (x_point - 2 > other.x_point + other.x_size)
                return false;

            if (x_point + x_size + 2 < other.x_point)
                return false;

            if (y_point - 2 > other.y_point + other.y_size)
                return false;

            if (y_point + y_size + 2 < other.y_point)
                return false;*/

            return -1;
        }

        public Vector3Int GetRandomPoint()
        {
            return new Vector3Int(Random.Range(x_point, x_point + x_size), Random.Range(y_point, y_point + y_size), 0);
        }

        public void SetUpMap(DungeonMap map, TileSetHolder tile_set)
        {
            for (int i = x_point; i < x_size + x_point; ++i)
                for (int e = y_point; e < y_size + y_point; ++e)
                {
                    map.SetNode(i, e, DungeonTileType.Ground, tile_set.GetTileModel(16), 16);
                }

            for (int i = x_point; i < x_size + x_point; ++i)
            {
                map.SetNode(i, y_point - 1, DungeonTileType.Wall, tile_set.GetTileModel(14), 14);
                map.SetNode(i, y_point + y_size, DungeonTileType.Wall, tile_set.GetTileModel(9), 9);
            }

            for (int i = y_point; i < y_size + y_point; ++i)
            {
                map.SetNode(x_point - 1, i, DungeonTileType.Wall, tile_set.GetTileModel(4), 4);
                map.SetNode(x_point + x_size, i, DungeonTileType.Wall, tile_set.GetTileModel(3), 3);
            }

            map.SetNode(x_point - 1, y_point - 1, DungeonTileType.Wall, tile_set.GetTileModel(13), 13);
            map.SetNode(x_point + x_size, y_point - 1, DungeonTileType.Wall, tile_set.GetTileModel(15), 15);

            map.SetNode(x_point - 1, y_point + y_size, DungeonTileType.Wall, tile_set.GetTileModel(8), 8);
            map.SetNode(x_point + x_size, y_point + y_size, DungeonTileType.Wall, tile_set.GetTileModel(10), 10);
        }
    }

    private class Path
    {
        public int x_point, y_point, x_change, y_change;
        public bool x_first;

        public Path(Room room_start, Room room_end)
        {

            x_point = room_start.x_point + 1;
            y_point = room_start.y_point + 1;

            x_change = (room_end.x_point + 1) - (room_start.x_point + 1);
            y_change = (room_end.y_point + 1) - (room_start.y_point + 1);
        }

        public bool IsWithin(int x, int y)
        {
            if (y == y_point && x_change > 0 && x >= x_point && x <= x_point + x_change)
                return true;

            if (y == y_point && x_change < 0 && x <= x_point && x >= x_point + x_change)
                return true;

            if (x == x_point + x_change && y_change > 0 && y >= y_point && y <= y_point + y_change)
                return true;

            if (x == x_point + x_change && y_change < 0 && y <= y_point && y >= y_point + y_change)
                return true;

            return false;
        }

        public void SetUpMap(DungeonMap map, TileSetHolder tile_set)
        {
            if (x_change > 0)
            {
                for (int i = x_point; i <= x_point + x_change; ++i)
                {
                    map.SetNode(i, y_point, DungeonTileType.Ground, tile_set.GetTileModel(16), 16);

                    if (map.GetTileModelNum(i, y_point + 1) == 19)
                        map.SetNode(i, y_point + 1, DungeonTileType.Wall, tile_set.GetTileModel(9), 9);
                    if (map.GetTileModelNum(i, y_point - 1) == 19)
                        map.SetNode(i, y_point - 1, DungeonTileType.Wall, tile_set.GetTileModel(14), 14);
                }
            }
            else if (x_change < 0)
            {
                for (int i = x_point + x_change; i <= x_point; ++i)
                {
                    map.SetNode(i, y_point, DungeonTileType.Ground, tile_set.GetTileModel(16), 16);

                    if (map.GetTileModelNum(i, y_point + 1) == 19)
                        map.SetNode(i, y_point + 1, DungeonTileType.Wall, tile_set.GetTileModel(9), 9);
                    if (map.GetTileModelNum(i, y_point - 1) == 19)
                        map.SetNode(i, y_point - 1, DungeonTileType.Wall, tile_set.GetTileModel(14), 14);
                }
            }

            if (y_change > 0)
            {
                for (int i = y_point; i <= y_point + y_change; ++i)
                {
                    map.SetNode(x_point + x_change, i, DungeonTileType.Ground, tile_set.GetTileModel(16), 16);

                    if (map.GetTileModelNum(x_point + x_change + 1, i) == 19)
                        map.SetNode(x_point + x_change + 1, i, DungeonTileType.Wall, tile_set.GetTileModel(3), 3);
                    if (map.GetTileModelNum(x_point + x_change - 1, i) == 19)
                        map.SetNode(x_point + x_change - 1, i, DungeonTileType.Wall, tile_set.GetTileModel(4), 4);
                }
            }
            else if (y_change < 0)
            {
                for (int i = y_point + y_change; i <= y_point; ++i)
                {
                    map.SetNode(x_point + x_change, i, DungeonTileType.Ground, tile_set.GetTileModel(16), 16);

                    if (map.GetTileModelNum(x_point + x_change + 1, i) == 19)
                        map.SetNode(x_point + x_change + 1, i, DungeonTileType.Wall, tile_set.GetTileModel(3), 3);
                    if (map.GetTileModelNum(x_point + x_change - 1, i) == 19)
                        map.SetNode(x_point + x_change - 1, i, DungeonTileType.Wall, tile_set.GetTileModel(4), 4);
                }
            }
        }
    }

    public override DungeonMap GenerateDungeon(DungeonWeatherManager weather_manager, out Vector3Int start_location)
    {
        List<Room> rooms = GenerateRooms(min_rooms, max_room);

        List<Path> paths = GeneratePaths(rooms);

        DungeonMap map = SetMap(rooms, paths);

        start_location = rooms[Random.Range(0, rooms.Count)].GetRandomPoint();

        map.SetTileTrait(rooms[Random.Range(0, rooms.Count)].GetRandomPoint(), 1);

        map.SetWeatherManager(weather_manager);

        map.ForceWeather(weather_type,weather_power);

        return map;
    }

    private List<Room> GenerateRooms(int min_rooms, int max_rooms)
    {
        List<Room> valid_rooms = new List<Room>();
        Room temp_room = null;

        for (int i = 0; i < Random.Range(min_rooms, max_rooms + 1); ++i)
        {
            bool invalid_room = true;
            while (invalid_room)
            {
                temp_room = new Room(x_size, y_size, room_min_size, room_max_size);

                invalid_room = false;

                foreach (Room room in valid_rooms)
                    if (room.RoomsOverlap(temp_room))
                        invalid_room = true;
            }
            valid_rooms.Add(temp_room);
        }

        return valid_rooms;
    }

    private List<Path> GeneratePaths(List<Room> rooms)
    {
        List<Path> valid_path = new List<Path>();

        for (int i = 0; i < rooms.Count - 1; ++i)
        {
            Path temp_path = new Path(rooms[i], rooms[i + 1]);

            valid_path.Add(temp_path);
        }

        return valid_path;
    }

    private DungeonMap SetMap(List<Room> rooms, List<Path> paths)
    {
        DungeonMap map = new DungeonMap(x_size, y_size);

        for (int i = 0; i < x_size; ++i)
            for (int e = 0; e < y_size; ++e)
                map.SetNode(i, e, DungeonTileType.Wall, tile_set.GetTileModel(19), 19);

        foreach (Room room in rooms)
            room.SetUpMap(map, tile_set);

        foreach (Path path in paths)
            path.SetUpMap(map, tile_set);

        return map;
    }

    public override Creature GetRandomCreature()
    {
        Creature creature = monsters[Random.Range(0, monsters.Length)].GetMonster();

        return creature;
    }

    public override Creature GetDungeonManager()
    {
        return new FloorCreatureRandom();
    }

    public override AIBase GetAI()
    {
        return ai;
    }
}