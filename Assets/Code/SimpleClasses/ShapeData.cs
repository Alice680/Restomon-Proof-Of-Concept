using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A data type that returns vector3s in a various patterns
 * Primaly used for Attacks to generate the area they effect and can target
 * 
 * Notes:
 * Not all shapes are programed
 */
public enum Shape { Line, Cone, Cube, Blast, Cross };

[Serializable]
public class ShapeData
{
    [SerializeField] private Shape shape;
    [SerializeField] private int length, width;
    [SerializeField] private bool rotatable, Hollow;

    public Vector3Int[] GetArea(Vector3Int target, Direction dir)
    {
        Vector3Int[] area = GetBaseArea();

        if (rotatable)
            area = ApplyRotation(area, dir);

        for (int i = 0; i < area.Length; ++i)
            area[i] += target;

        return area;
    }

    // TODO add in the rest of the shapes and any more I can think of.
    // TODO fix blast, the equation is wrong.
    private Vector3Int[] GetBaseArea()
    {
        List<Vector3Int> area = new List<Vector3Int>();

        switch (shape)
        {
            case Shape.Line:
                for (int i = 1; i <= length; ++i)
                    for (int e = 1; e <= width; ++e)
                        area.Add(new Vector3Int(e, i, 0));
                break;

            case Shape.Cone:
                break;

            case Shape.Cube:
                for (int i = -length; i <= length; ++i)
                    for (int e = -width; e <= width; ++e)
                        area.Add(new Vector3Int(e, i, 0));
                break;

            case Shape.Blast:
                for (int i = -length; i <= length; ++i)
                    for (int e = -width; e <= width; ++e)
                        if (i + e <= length)
                            area.Add(new Vector3Int(e, i, 0));
                break;

            case Shape.Cross:
                for (int i = -length; i <= length; ++i)
                    area.Add(new Vector3Int(i, 0, 0));

                for (int i = -width; i <= width; ++i)
                    area.Add(new Vector3Int(0, i, 0));

                break;
        }

        while (Hollow && area.Contains(new Vector3Int(0, 0, 0)))
            area.Remove(new Vector3Int(0, 0, 0));

        return area.ToArray();
    }

    private Vector3Int[] ApplyRotation(Vector3Int[] area,Direction dir)
    {
        if (dir == Direction.Down)
        {
            for (int i = 0; i < area.Length; ++i)
            {
                area[i].y *= -1;
            }
        }

        if (dir == Direction.Right || dir == Direction.Left)
        {
            for (int i = 0; i < area.Length; ++i)
            {
                area[i].z = area[i].x;
                area[i].x = area[i].y;
                area[i].y = area[i].z;
                area[i].z = 0;
            }

            if (dir == Direction.Left)
            {
                for (int i = 0; i < area.Length; ++i)
                {
                    area[i].x *= -1;
                }
            }
        }

        return area;
    }
}