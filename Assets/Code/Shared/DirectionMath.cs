using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Handles common sitaution involving Directions
 * 
 * Notes:
 * Keep an eye out for common situation that can be added.
 */

public static class DirectionMath
{
    public static Direction GetDirectionChange(Vector3Int start, Vector3Int end)
    {
        if (start.x < end.x && start.y == end.y)
            return Direction.Right;

        if (start.x > end.x && start.y == end.y)
            return Direction.Left;

        if (start.x == end.x && start.y < end.y)
            return Direction.Up;

        if (start.x == end.x && start.y > end.y)
            return Direction.Down;

        return Direction.None;
    }

    public static Vector3Int GetVectorChange(Direction dir)
    {
        if (dir == Direction.Up)
            return new Vector3Int(0, 1, 0);
        else if (dir == Direction.Down)
            return new Vector3Int(0, -1, 0);
        else if (dir == Direction.Right)
            return new Vector3Int(1, 0, 0);
        else if (dir == Direction.Left)
            return new Vector3Int(-1, 0, 0);

        return new Vector3Int(0, 0, 0);
    }
}