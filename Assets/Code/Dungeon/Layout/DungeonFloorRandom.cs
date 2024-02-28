using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Random", menuName = "ScriptableObjects/Dungeons/Random")]
public class DungeonFloorRandom : DungeonFloor
{
    [SerializeField] protected TileSetHolder tile_set;
    [SerializeField] protected int x_size, y_size, min_rooms, max_room, room_min_size, room_max_size;
    [SerializeField] protected MonsterChance[] monsters;
    [SerializeField] private int spawn_rate;
    [SerializeField] protected int min_monsters, max_monsters;
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

        public int GetInnerEdge(bool x_first, out int y)
        {
            if (x_first)
            {
                y = Random.Range(y_point + 2, y_point + y_size - 1);
                return Random.Range(0, 2) == 1 ? x_point + 1 : x_point + x_size - 1;
            }
            else
            {
                y = Random.Range(0, 2) == 1 ? y_point + 1 : y_point + y_size - 1;
                return Random.Range(x_point + 2, x_point + x_size - 1);
            }
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

        public bool PathValid(Path other)
        {
            if(other.x_first)
            {
                for (int i = 0; i < 3; ++i)
                {
                    if (other.x_goal == 0)
                        return false;
                }
            }
            else
            {

            }

            return true;
        }

        public Vector3Int GetRandomPoint()
        {
            return new Vector3Int(Random.Range(x_point, x_point + x_size), Random.Range(y_point, y_point + y_size), 0);
        }

        public void SetUpMap(DungeonMap map, TileSetHolder tile_set)
        {
            for (int i = x_point; i < x_size + x_point; ++i)
                for (int e = y_point; e < y_size + y_point; ++e)
                    map.SetNode(i, e, Random.Range(14, 17));

            for (int i = x_point; i < x_size + x_point; ++i)
            {
                map.SetNode(i, y_point - 1, 5);
                map.SetNode(i, y_point + y_size, 1);
            }

            for (int i = y_point; i < y_size + y_point; ++i)
            {
                map.SetNode(x_point - 1, i, 7);
                map.SetNode(x_point + x_size, i, 3);
            }

            map.SetNode(x_point - 1, y_point - 1, 6);
            map.SetNode(x_point - 1, y_point + y_size, 0);
            map.SetNode(x_point + x_size, y_point - 1, 4);
            map.SetNode(x_point + x_size, y_point + y_size, 2);
        }
    }

    protected class Path
    {
        public int x_point, y_point, x_goal, y_goal;
        public bool x_first;

        public Path(Room room_start, Room room_end)
        {
            x_first = /*Random.Range(0, 2) == 1*/true;

            x_point = room_start.GetInnerEdge(x_first, out y_point);

            x_goal = room_end.GetInnerEdge(!x_first, out y_goal);
        }

        public bool PathValid(Path other)
        {
            return true;
        }

        public void SetUpMap(DungeonMap map)
        {
            Vector3Int temp_corner = new Vector3Int(0, 0, 0);

            if (x_first)
            {
                if (x_point > x_goal)
                {
                    SetUpX(map, y_point, x_goal, x_point);
                    temp_corner.x = -1;
                }
                else
                {
                    SetUpX(map, y_point, x_point, x_goal);
                    temp_corner.x = 1;
                }

                if (y_point > y_goal)
                {
                    SetUpY(map, x_goal, y_goal, y_point);
                    temp_corner.y = 1;
                }
                else
                {
                    SetUpY(map, x_goal, y_point, y_goal);
                    temp_corner.y = -1;
                }

                SetUpCorner(map, new Vector3Int(x_goal, y_point, 0), temp_corner);
            }
            else
            {
                if (x_point > x_goal)
                    SetUpX(map, y_goal, x_goal, x_point);
                else
                    SetUpX(map, y_goal, x_point, x_goal);

                if (y_point > y_goal)
                    SetUpY(map, x_point, y_goal, y_point);
                else
                    SetUpY(map, x_point, y_point, y_goal);
            }
        }

        private void SetUpX(DungeonMap map, int y, int x_start, int x_end)
        {
            for (int i = x_start; i <= x_end; ++i)
            {
                map.SetNode(i, y, 14);

                if (map.GetTileModelNum(i, y + 1) < 14)
                {
                    if (map.GetTileModelNum(i, y + 1) == 7)
                        map.SetNode(i, y + 1, 11);
                    else if (map.GetTileModelNum(i, y + 1) == 3)
                        map.SetNode(i, y + 1, 10);
                    else
                        map.SetNode(i, y + 1, 1);
                }
                if (map.GetTileModelNum(i, y - 1) < 14)
                {
                    if (map.GetTileModelNum(i, y - 1) == 7)
                        map.SetNode(i, y - 1, 9);
                    else if (map.GetTileModelNum(i, y - 1) == 3)
                        map.SetNode(i, y - 1, 8);
                    else
                        map.SetNode(i, y - 1, 5);
                }
            }
        }

        private void SetUpY(DungeonMap map, int x, int y_start, int y_end)
        {
            for (int i = y_start; i <= y_end; ++i)
            {
                map.SetNode(x, i, 14);

                if (map.GetTileModelNum(x + 1, i) < 14)
                {
                    if (map.GetTileModelNum(x + 1, i) == 5)
                        map.SetNode(x + 1, i, 8);
                    else if (map.GetTileModelNum(x + 1, i) == 1)
                        map.SetNode(x + 1, i, 10);
                    else
                        map.SetNode(x + 1, i, 3);
                }
                if (map.GetTileModelNum(x - 1, i) < 14)
                {
                    if (map.GetTileModelNum(x - 1, i) == 5)
                        map.SetNode(x - 1, i, 9);
                    else if (map.GetTileModelNum(x - 1, i) == 1)
                        map.SetNode(x - 1, i, 11);
                    else
                        map.SetNode(x - 1, i, 7);
                }
            }
        }

        private void SetUpCorner(DungeonMap map, Vector3Int edge, Vector3Int change)
        {
            int value = -1;

            if (change.x == -1 && change.y == 1)
                value = 0;
            else if (change.x == 1 && change.y == 1)
                value = 2;
            else if (change.x == 1 && change.y == -1)
                value = 4;
            else if (change.x == -1 && change.y == -1)
                value = 6;

            if (map.GetTileModelNum(edge + change) < 14)
                map.SetNode(edge.x + change.x, edge.y + change.y, value);
        }
    }

    [System.Serializable]
    protected class MonsterChance
    {
        public int chance;
        public MonsterStats monster;
    }

    protected class ItemChance
    {
        public int index, chance;
        public bool unique;
    }

    public override DungeonMap GenerateDungeon(DungeonWeatherManager weather_manager, out Vector3Int start_location, out Creature[] enemies, out Vector3Int[] positions)
    {
        List<Room> rooms;
        List<Path> paths;

        while (true)
        {
            if (!GenerateRooms(min_rooms, max_room, out rooms))
                continue;

            if (!GeneratePaths(rooms, out paths))
                continue;

            break;
        }

        DungeonMap map = SetMap(rooms, paths);

        start_location = rooms[Random.Range(0, rooms.Count)].GetRandomPoint();

        map.SetTileTrait(rooms[Random.Range(0, rooms.Count)].GetRandomPoint(), 1);

        map.SetWeatherManager(weather_manager);

        map.ForceWeather(weather_type, weather_power);

        GetEnemys(start_location, rooms.ToArray(), out enemies, out positions);

        return map;
    }

    private bool GenerateRooms(int min_rooms, int max_rooms, out List<Room> valid_rooms)
    {
        valid_rooms = new List<Room>();
        Room temp_room = null;

        for (int i = 0; i < Random.Range(min_rooms, max_rooms + 1); ++i)
        {
            int iteration_tracker = 0;
            bool invalid_room = true;
            while (invalid_room)
            {
                if (++iteration_tracker == 250)
                    return false;

                temp_room = new Room(x_size, y_size, room_min_size, room_max_size);

                invalid_room = false;

                foreach (Room room in valid_rooms)
                    if (room.RoomsOverlap(temp_room))
                        invalid_room = true;
            }
            valid_rooms.Add(temp_room);
        }

        return true;
    }

    private bool GeneratePaths(List<Room> rooms, out List<Path> valid_path)
    {
        valid_path = new List<Path>();

        for (int i = 0; i < rooms.Count; ++i)
        {
            int iteration_tracker = 0;
            while (true)
            {
                if (++iteration_tracker == 250)
                    return false;

                int temp_room = Random.Range(0, rooms.Count);

                if (temp_room == i)
                    continue;

                Path temp_path = new Path(rooms[i], rooms[temp_room]);

                bool temp_is_valid = true;

                foreach (Room room in rooms)
                    if (!room.PathValid(temp_path))
                    {
                        temp_is_valid = false;
                        break;
                    }

                if (temp_is_valid == false)
                    continue;

                foreach (Path path in valid_path)
                    if (!path.PathValid(temp_path))
                    {
                        temp_is_valid = false;
                        break;
                    }

                if (temp_is_valid == false)
                    continue;

                valid_path.Add(temp_path);
                break;
            }
        }

        return true;
    }

    private DungeonMap SetMap(List<Room> rooms, List<Path> paths)
    {
        DungeonMap map = new DungeonMap(x_size, y_size, tile_set);

        for (int i = 0; i < x_size; ++i)
            for (int e = 0; e < y_size; ++e)
                map.SetNode(i, e, 12);

        foreach (Room room in rooms)
            room.SetUpMap(map, tile_set);

        foreach (Path path in paths)
            path.SetUpMap(map);

        return map;
    }

    private void GetEnemys(Vector3Int player_position, Room[] rooms, out Creature[] enemies, out Vector3Int[] positions)
    {
        enemies = new Creature[Random.Range(min_monsters, max_monsters + 1)];
        positions = new Vector3Int[enemies.Length];

        for (int i = 0; i < enemies.Length; ++i)
        {
            enemies[i] = GetRandomCreature();

            while (true)
            {
                positions[i] = rooms[Random.Range(0, rooms.Length)].GetRandomPoint();

                if (positions[i] == player_position)
                    continue;

                bool overlap = false;

                for (int e = 0; e < i; ++e)
                    if (positions[i] == positions[e])
                        overlap = true;

                if (overlap)
                    continue;

                break;
            }
        }
    }

    public override Creature GetRandomCreature()
    {
        Creature creature = null;

        int temp_random = Random.Range(0, 100);

        foreach (MonsterChance option in monsters)
            if (option.chance > temp_random)
            {
                creature = option.monster.GetMonster();
                break;
            }

        return creature;
    }

    public override Creature GetDungeonManager()
    {
        return new FloorCreatureRandom(spawn_rate);
    }

    public override AIBase GetAI()
    {
        return ai;
    }
}