using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShapeData
{
    [SerializeField] private Shape shape;
    [SerializeField] private int length;
    [SerializeField] private int width;

    public Vector3Int[] GetArea(Direction dir)
    {
        List<Vector3Int> area = new List<Vector3Int>();

        switch (shape)
        {
            case Shape.Line:
                break;

            case Shape.Cone:
                break;

            case Shape.Cube:
                for (int i = -length; i <= length; ++i)
                    for (int e = -width; e <= width; ++e)
                        area.Add(new Vector3Int(i, e, 0));
                break;

            case Shape.Blast:
                for (int i = -length; i <= length; ++i)
                    for (int e = -width; e <= width; ++e)
                        if (i + e <= length)
                            area.Add(new Vector3Int(i, e, 0));
                break;

            case Shape.Cross:
                for (int i = -length; i <= length; ++i)
                    area.Add(new Vector3Int(i, 0, 0));

                for (int i = -width; i <= width; ++i)
                    area.Add(new Vector3Int(0, i, 0));

                break;
        }

        return area.ToArray();
    }
}