using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The wrapper class for all creature types. It serves as instances of creatures with modifiable data.
 * It also serves as a way for the reste of the game to interact with all the difrent creature types at once.
 * 
 * Notes:
 * This class is unfished atm.
 * Need to finish the ailment/condition system.
 */
public class Unit
{
    private class Condition
    {
        public int type, rank;

        public Condition(int type, int rank)
        {
            this.type = type;
            this.rank = rank;
        }
    }

    private static int current_id;
    private int id;

    private CreatureType creature_type;
    private Creature creature;

    private int hp, mp;

    private int[] stat_rank;

    private List<Condition> conditions;

    private RestomonEvolution restomon_evolution;

    private Vector3Int model_position;
    private Actor owner;
    private GameObject model;

    public Unit(Creature creature, Actor owner)
    {
        id = ++current_id;

        this.creature = creature;
        this.owner = owner;

        stat_rank = new int[9];

        conditions = new List<Condition>();

        switch (creature.GetCreatureType())
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
            case CreatureType.Floor:
                SetupFloor();
                break;
        }

        SetPosition(new Vector3Int(-1, -1, 0));
    }

    /*
     * The following methods all set up data relevent to their creature type
     */
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
        mp = restomon.GetMp();

        restomon_evolution = RestomonEvolution.None;

        model = restomon.GetModel(restomon_evolution);
    }

    private void SetupHuman(Human human)
    {
        creature_type = human.GetCreatureType();

        hp = human.GetHp();

        model = human.GetModel();
    }

    private void SetupFloor()
    {
        creature_type = CreatureType.Floor;

        hp = creature.GetStat(0);

        model = null;
    }

    /*
     * The following methods are all just simple setters atm. But will need to have trait intergration set up in the future.
     * At which point they will recive their own discriptions.
     */
    public void StartTurn()
    {
        if (creature_type == CreatureType.Human)
        {
            ChangeHp(((Human)creature).GetApr());
        }
    }

    public void ChangeHp(int value)
    {
        hp += value;

        if (hp < 0)
            hp = 0;

        if (hp > GetMaxHP())
            hp = GetMaxHP();
    }

    public void ChangeMP(int value)
    {
        if (creature_type != CreatureType.Restomon)
            return;

        mp += value;

        if (mp < 0)
            mp = 0;

        if (mp > GetMaxMP())
            mp = GetMaxMP();
    }

    public void ChangeStatRank(int index, int num)
    {
        if (index < 0 || index > 9)
            return;

        stat_rank[index] += num;

        if (stat_rank[index] > 4)
            stat_rank[index] = 4;

        if (stat_rank[index] < -4)
            stat_rank[index] = -4;
    }

    public void SetCondition(int index, int rank)
    {
        Condition current_condition = null;

        foreach (Condition condition in conditions)
            if (condition.type == index)
                current_condition = condition;

        if (rank != -1 && current_condition != null)
            current_condition.rank = rank;
        else if (rank != -1 && current_condition == null)
            conditions.Add(new Condition(index, rank));
        else if (current_condition != null)
            conditions.Remove(current_condition);
    }

    public void SetPosition(Vector3Int new_position)
    {
        if (creature_type == CreatureType.Floor)
            return;

        model_position = new_position;
        model.transform.position = model_position;
    }

    public void Evolve(int new_form)
    {
        if (creature_type != CreatureType.Restomon)
            return;

        restomon_evolution = (RestomonEvolution)(1 + new_form);

        GameObject temp_model = ((Restomon)creature).GetModel(restomon_evolution);

        temp_model.transform.position = model.transform.position;

        GameObject.Destroy(model);

        model = temp_model;
    }

    public void ShowEvolution(int new_form)
    {
        GameObject temp_model;

        if (new_form == -1)
            temp_model = ((Restomon)creature).GetModel(restomon_evolution);
        else
            temp_model = ((Restomon)creature).GetModel((RestomonEvolution)(new_form + 1));

        temp_model.transform.position = model.transform.position;

        GameObject.Destroy(model);

        model = temp_model;
    }

    public void KillUnit()
    {
        if (creature_type == CreatureType.Floor)
            return;

        GameObject.Destroy(model);
    }

    /*
     * The following methods are all getters.
     */
    public Actor GetOwner()
    {
        return owner;
    }

    public int GetBaseID()
    {
        if(creature_type == CreatureType.Monster)
            return creature.GetID();
        else if (creature_type == CreatureType.Restomon)
            return creature.GetID();

        return -1;
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

    public Element GetElement(int i)
    {
        return creature.GetElement(i);
    }

    // TODO add in
    public int GetElementAffinity(Element element)
    {
        int affinity = 0;

        if (GetElement(0) == element)
            affinity += 4;
        if (GetElement(1) == element)
            affinity += 3;
        if (GetElement(2) == element)
            affinity += 2;

        return affinity;
    }

    //TODO add second form handling
    public Unit GetEvolution(int index)
    {
        Unit temp_unit = new Unit(creature, owner);

        temp_unit.Evolve(index);

        return temp_unit;
    }

    // TODO add in
    public int GetResistance(Element element)
    {
        return -1;
    }

    public int GetHp()
    {
        return hp;
    }

    public int GetMaxHP()
    {
        if (creature_type == CreatureType.Floor)
            return creature.GetStat(0);

        return creature.GetHp();
    }

    public int GetMp()
    {
        if (creature_type != CreatureType.Restomon)
            return -1;

        return mp;
    }

    public int GetMaxMP()
    {
        if (creature_type != CreatureType.Restomon)
            return -1;

        return ((Restomon)creature).GetMp();
    }

    public int GetStat(int index)
    {
        if (index < 0 || index > 9)
            return -1;

        if (creature_type == CreatureType.Floor)
            return creature.GetStat(index);

        int base_stat;

        if (creature_type == CreatureType.Restomon)
            base_stat = ((Restomon)creature).GetStat(index, restomon_evolution);
        else
            base_stat = creature.GetStat(index);

        return (int)Mathf.Max(1, (base_stat * Mathf.Pow(2, stat_rank[index] / 2f)));
    }

    public int GetEvolutionCost(int new_form)
    {
        if (creature_type == CreatureType.Restomon)
            return ((Restomon)creature).GetSummonCost(restomon_evolution, new_form);
        else
            return 0;
    }

    public int GetMaintenanceCost()
    {
        if (creature_type == CreatureType.Restomon)
            return ((Restomon)creature).GetMaintenanceCost(restomon_evolution);
        else
            return 0;
    }

    public int GetUpkeepCost()
    {
        if (creature_type == CreatureType.Restomon)
            return ((Restomon)creature).GetUpkeepCost(restomon_evolution);
        else
            return 0;
    }

    public int GetNumConditions()
    {
        return conditions.Count;
    }

    public int GetCondition(int index, out int rank)
    {
        if (index < 0 || index >= conditions.Count)
        {
            rank = -1;
            return -1;
        }

        rank = conditions[index].rank;
        return conditions[index].type;
    }

    public int GetCondition(int type)
    {
        foreach (Condition condition in conditions)
            if (condition.type == type)
                return condition.rank;
        return -1;
    }

    public Trait[] GetTraits()
    {
        if (creature_type == CreatureType.Restomon)
            return ((Restomon)creature).GetTraits(restomon_evolution);
        else
            return creature.GetTraits();
    }

    public Attack GetAttack(int index)
    {
        if (creature_type == CreatureType.Restomon)
            return ((Restomon)creature).GetAttack(index, restomon_evolution);
        else
            return creature.GetAttack(index);
    }

    public Vector3Int GetPosition()
    {
        return model_position;
    }

    public RestomonEvolution GetCurrentEvolution()
    {
        return restomon_evolution;
    }

    public GameObject GetModel()
    {
       return GameObject.Instantiate(model);
    }

    public override string ToString()
    {
        return creature.ToString();
    }

    public bool Compare(Unit unit)
    {
        return id == unit.GetID();
    }
}