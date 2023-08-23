using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A pathfinding script that finds that shortest path between two nodes
 * Notes:
 * Only moves in Cardinal directions
 * Has no way of taking attacks that move you into considerations
 */
// TODO add in dynamic tile types
public static class Pathfinding
{
    private class PathNode
    {
        public int f;
        public int g;
        public int h;

        public int x;
        public int y;

        public PathNode previous_node;

        public PathNode(Vector3Int position)
        {
            x = position.x;
            y = position.y;

            g = 0;
        }

        public void CalculateG(PathNode previous)
        {
            if (g == 0 || previous.g + 1 < g)
            {
                g = previous.g + 1;

                previous_node = previous;

                f = g + h;
            }
        }

        public void CalculateH(PathNode goal)
        {
            h = 0;

            h += Mathf.Abs(x - goal.x);
            h += Mathf.Abs(y - goal.y);

            f = g + h;
        }

        public List<Vector3Int> GetPath()
        {
            List<Vector3Int> list;

            if (previous_node != null)
                list = previous_node.GetPath();
            else
                list = new List<Vector3Int>();

            list.Add(new Vector3Int(x, y, 0));

            return list;
        }

        public Vector3Int GetPosition()
        {
            return new Vector3Int(x, y, 0);
        }
    }

    private static DungeonMap dungeon_map;

    private static PathNode[,] grid;

    private static int length;
    private static int height;

    private static List<PathNode> open_list;
    private static List<PathNode> closed_list;

    private static PathNode start_node;
    private static PathNode current_node;
    private static PathNode end_node;

    public static Vector3Int[] GetPath(DungeonMap map, Vector3Int start, Vector3Int goal)
    {

        if (map == null || !map.IsInMap(start) || !map.IsInMap(goal))
            return null;


        dungeon_map = map;
        Setup();

        start_node = new PathNode(start);
        end_node = new PathNode(goal);

        open_list.Add(start_node);

        SetH();

        int i = 0;

        while (open_list.Count != 0)
        {
            NextNode();

            if (current_node.x == end_node.x && current_node.y == end_node.y)
            {
                Clear();
                return current_node.GetPath().ToArray();
            }

            open_list.Remove(current_node);
            closed_list.Add(current_node);

            UpdateOpen();

            ++i;

            if (i > 1000)
            {
                Debug.Log("1K runs");
                Clear();
                return null;
            }
        }

        Clear();
        return null;
    }

    private static void Setup()
    {
        length = dungeon_map.GetSize().x;
        height = dungeon_map.GetSize().y;

        open_list = new List<PathNode>();
        closed_list = new List<PathNode>();

        grid = new PathNode[length, height];

        for (int i = 0; i < length; ++i)
            for (int I = 0; I < height; ++I)
            {
                grid[i, I] = new PathNode(new Vector3Int(i, I, 0));
            }
    }

    private static void SetH()
    {
        for (int i = 0; i < length; ++i)
            for (int I = 0; I < height; ++I)
                grid[i, I].CalculateH(end_node);
    }

    private static void NextNode()
    {
        PathNode temp = open_list[0];

        foreach (PathNode n in open_list)
            if (n.f < temp.f)
                temp = n;

        current_node = temp;
    }

    private static void UpdateOpen()
    {
        if (current_node.x > 0)
            AddNodeToOpen(grid[current_node.x - 1, current_node.y]);

        if (current_node.y > 0)
            AddNodeToOpen(grid[current_node.x, current_node.y - 1]);

        if (current_node.x < length - 1)
            AddNodeToOpen(grid[current_node.x + 1, current_node.y]);

        if (current_node.y < height - 1)
            AddNodeToOpen(grid[current_node.x, current_node.y + 1]);
    }

    private static void AddNodeToOpen(PathNode node)
    {
        if (closed_list.Contains(node))
            return;

        if (dungeon_map.GetTileType(node.GetPosition()) == DungeonTileType.Wall)
        {
            closed_list.Add(node);
            return;
        }

        if (dungeon_map.GetUnit(node.GetPosition()) != null && node.GetPosition() != end_node.GetPosition())
        {
            closed_list.Add(node);
            return;
        }

        node.CalculateG(current_node);

        if (!open_list.Contains(node))
            open_list.Add(node);
    }

    private static void Clear()
    {

    }
}