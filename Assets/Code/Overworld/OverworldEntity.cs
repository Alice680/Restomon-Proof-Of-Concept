using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldEntity
{
    private int x_position, y_position;

    private GameObject model;

    public OverworldEntity(GameObject model)
    {
        x_position = -1;
        y_position = -1;
        this.model = model;
        this.model.transform.position = new Vector3(-1, -1, 0);
    }

    public Vector3Int GetPosition()
    {
        return new Vector3Int(x_position, y_position, 0);
    }

    public void UpdatePosition(Vector3Int position)
    {
        x_position = position.x;
        y_position = position.y;
        model.transform.position = position;
    }
}