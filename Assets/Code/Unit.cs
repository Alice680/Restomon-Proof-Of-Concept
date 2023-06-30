using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    private static int current_id;
    private int id;

    private CreatureType type;
    private Creature creature;

    private int hp, mp, sp;

    private Vector3Int model_position;
    private Actor owner;
    private GameObject model;

    //Setup
    public Unit(Creature creature, Actor owner)
    {
        id = ++current_id;

        this.owner = owner;

        switch(creature.GetCreatureType())
        {
            case CreatureType.Monster:
                SetupMonster((Monster)creature);
                break;
            case CreatureType.Restomon:
                SetupRestomon((Restomon)creature);
                break;
        }

        SetPosition(new Vector3Int(-1, -1, 0));
    }

    public void SetupMonster(Monster monster)
    {
        type = monster.GetCreatureType();

        hp = monster.GetHp();

        model = monster.GetModel();
    }

    public void SetupRestomon(Restomon restomon)
    {
        type = restomon.GetCreatureType();

        hp = restomon.GetHp();
        sp = restomon.GetSp();
        mp = restomon.GetMp();

        model = restomon.GetModel();
    }

    //Update Data
    public void ChangeHp(int num)
    {
        hp += num;
    }

    public void SetPosition(Vector3Int new_position)
    {
        model_position = new_position;
        model.transform.position = model_position;
    }

    //Get Data
    public Actor GetOwner()
    {
        return owner;
    }

    public int GetID()
    {
        return id;
    }

    public int GetHp()
    {
        return hp;
    }

    public int GetStat(int index)
    {
        return creature.GetStat(index);
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