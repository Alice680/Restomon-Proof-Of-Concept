using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    private static int current_id;
    private int id;

    private CreatureType creature_type;
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
            case CreatureType.Human:
                SetupHuman((Human)creature);
                break;
            case CreatureType.Arena:
                SetupArena((ArenaStats)creature);
                break;
        }

        this.creature = creature;

        SetPosition(new Vector3Int(-1, -1, 0));
    }

    private void SetupMonster(Monster monster)
    {
        creature_type = monster.GetCreatureType();

        hp = monster.GetHp();

        model = monster.GetModel();
    }

    private void SetupRestomon(Restomon restomon)
    {
        creature_type = restomon.GetCreatureType();

        hp = restomon.GetHp();
        sp = restomon.GetSp();
        mp = restomon.GetMp();

        model = restomon.GetModel();
    }

    private void SetupHuman(Human human)
    {
        creature_type = human.GetCreatureType();

        hp = human.GetHp();

        model = human.GetModel();
    }

    private void SetupArena(ArenaStats arena)
    {
        creature_type = CreatureType.Arena;

        hp = -1;

        model = null;
    }

    //Update Data
    public void StartTurn()
    {
        if(creature_type == CreatureType.Human)
        {
            ChangeHp(((Human)creature).GetApr());
        }
    }

    public void ChangeHp(int num)
    {
        hp += num;

        if (hp < 0)
            hp = 0;

        if (hp > GetMaxHP())
            hp = GetMaxHP();
    }

    public void SetPosition(Vector3Int new_position)
    {
        if (creature_type == CreatureType.Arena)
            return;

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

    public CreatureType GetCreatureType()
    {
        return creature_type;
    }

    public int GetLV()
    {
        return creature.GetLV();
    }

    public int GetHp()
    {
        return hp;
    }

    public int GetMaxHP()
    {
        return creature.GetHp();
    }

    public int GetStat(int index)
    {
        return creature.GetStat(index);
    }

    public Attack GetAttack(int index)
    {
        return creature.GetAttack(index);
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