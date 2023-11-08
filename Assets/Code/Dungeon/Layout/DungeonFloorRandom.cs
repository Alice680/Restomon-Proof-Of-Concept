using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Random", menuName = "ScriptableObjects/Dungeons/Random")]
public class DungeonFloorRandom : DungeonFloor
{
    [SerializeField] protected TileSetHolder tile_set;
    [SerializeField] protected int x_size, y_size, min_rooms, max_room, room_min_size, room_max_size;
    [SerializeField] protected MonsterStats[] monsters;

    protected class Room
    {
        public int x_point, x_size, y_point, y_size;

        public Room(int x_limit, int y_limit, int min_size, int max_size)
        {
            x_size = Random.Range(min_size, max_size);
            y_size = Random.Range(min_size, max_size);

            x_point = Random.Range(0, x_limit - x_size);
            y_point = Random.Range(0, y_limit - y_size);
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
            return false;
        }

        public Vector3Int GetRandomPoint()
        {
            return new Vector3Int(Random.Range(x_point, x_point + x_size), Random.Range(y_point, y_point + y_size), 0);
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
    }

    public override DungeonMap GenerateDungeon(out Vector3Int start_location)
    {
        List<Room> rooms = GenerateRooms(min_rooms, max_room);

        List<Path> paths = GeneratePaths(rooms);

        DungeonMap map = SetMap(rooms, paths);

        start_location = rooms[Random.Range(0, rooms.Count)].GetRandomPoint();

        return map;
    }

    private List<Room> GenerateRooms(int min_rooms, int max_rooms)
    {
        List<Room> valid_rooms = new List<Room>();
        Room temp_room = null;

        for (int i = 0; i < Random.Range(min_rooms, max_rooms + 1); ++i)
        {
            while (true)
            {
                temp_room = new Room(x_size, y_size, room_min_size, room_max_size);
                foreach (Room room in valid_rooms)
                    if (room.RoomsOverlap(temp_room))
                        continue;
                break;
            }
            valid_rooms.Add(temp_room);
        }

        return valid_rooms;
    }

    private List<Path> GeneratePaths(List<Room> rooms)
    {
        List<Path> valid_path = new List<Path>();

        for (int i = 0; i < rooms.Count - 1; ++i)
            valid_path.Add(new Path(rooms[i], rooms[i + 1]));

        return valid_path;
    }

    private DungeonMap SetMap(List<Room> rooms, List<Path> paths)
    {
        DungeonMap map = new DungeonMap(x_size, y_size);

        for (int i = 0; i < x_size; ++i)
            for (int e = 0; e < y_size; ++e)
                map.SetNode(i, e, DungeonTileType.Wall, tile_set.GetTileModel(19));


        for (int i = 0; i < x_size; ++i)
            for (int e = 0; e < y_size; ++e)
            {
                foreach (Room room in rooms)
                    if (room.IsWithin(i, e))
                        map.SetNode(i, e, DungeonTileType.Ground, tile_set.GetTileModel(16));
                foreach (Path path in paths)
                    if (path.IsWithin(i, e))
                        map.SetNode(i, e, DungeonTileType.Ground, tile_set.GetTileModel(16));
            }

        return map;
    }

    public override Creature GetRandomCreature()
    {
        Creature creature = monsters[Random.Range(0,monsters.Length)].GetMonster();

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