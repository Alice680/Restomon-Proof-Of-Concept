using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    private static int current_id;
    private int id;

    private Vector3Int model_position;
    private Actor owner;
    private GameObject model;

    public Unit(GameObject model_path, Actor owner)
    {
        id = ++current_id;

        this.owner = owner;

        model = GameObject.Instantiate(model_path);
        SetPosition(new Vector3Int(-1, -1, 0));
    }

    public Actor GetOwner()
    {
        return owner;
    }

    public int GetID()
    {
        return id;
    }

    public void SetPosition(Vector3Int new_position)
    {
        model_position = new_position;
        model.transform.position = model_position;
    }

    public Vector3Int GetPosition()
    {
        return model_position;
    }

    public bool Compare(Unit unit)
    {
        return id == unit.GetID();
    }
}