using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private DungeonMap map;
    private List<Unit> units;
    private List<Actor> actors;

    private Unit current_unit;

    private float attack_time;
    private List<GameObject> attack_moddels;

    //temp
    public Player controller;

    public DungeonLayout layout;

    public RestomonBase player_temp;
    public MonsterStats enemy_temp;

    //Unit calls
    private void Start()
    {
        map = layout.GenerateDungeon();

        units = new List<Unit>();
        actors = new List<Actor>();

        controller = new Player(this);

        current_unit = new Unit(player_temp.GetRestomon(5, 0, new int[4] { 0, 0, 0, 0 }), controller);
        map.MoveUnit(5, 5, current_unit);

        Unit temp_unit = new Unit(enemy_temp.GetMonster(), controller);
        map.MoveUnit(7, 5, temp_unit);

        units.Add(current_unit);
        units.Add(temp_unit);
        actors.Add(controller);

        attack_time = -1;
        attack_moddels = new List<GameObject>();
    }

    private void Update()
    {
        if (attack_time != -1 && Time.time - attack_time > 0.1f)
            RemoveAttackModels();

        current_unit.GetOwner().Run();
    }

    //Actions
    public void Move(Direction dir)
    {
        if (dir == Direction.None || !MoveValid(dir))
            return;

        Vector3Int new_position = current_unit.GetPosition();

        if (dir == Direction.Up)
            new_position += new Vector3Int(0, 1, 0);
        else if (dir == Direction.Down)
            new_position += new Vector3Int(0, -1, 0);
        else if (dir == Direction.Right)
            new_position += new Vector3Int(1, 0, 0);
        else if (dir == Direction.Left)
            new_position += new Vector3Int(-1, 0, 0);

        map.MoveUnit(new_position, current_unit);
    }

    public void Attack(Vector3Int target, int index)
    {
        if (!AttackTargetValid(target, index))
            return;

        Attack attack = current_unit.GetAttack(index);
        GameObject marker = attack.GetModel();

        List<Unit> attack_targets = new List<Unit>();

        RemoveAttackModels();
        attack_time = Time.time;

        Vector3Int[] positions = attack.GetTarget(Direction.None);

        for (int i = 0; i < positions.Length; ++i)
            positions[i] += target;

        for (int i = 0; i < positions.Length; ++i)
        {
            attack_moddels.Add(Instantiate(marker, positions[i], new Quaternion()));

            if (map.GetUnit(target) != null)
                attack_targets.Add(map.GetUnit(target));
        }

        foreach (Unit unit in attack_targets)
        {
            attack.ApplyEffect(current_unit, unit);

            if (unit.GetHp() <= 0)
                RemoveUnit(unit);
        }
    }

    //Internal data edit
    private void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
        map.RemoveUnit(unit);
    }

    private void RemoveAttackModels()
    {
        attack_time = -1;

        foreach (GameObject obj in attack_moddels)
            Destroy(obj);

        attack_moddels.Clear();
    }

    //Edit Markers
    public void ShowAttackArea(Vector3Int target, int index)
    {
        map.RemoveAllMarker();

        Attack attack = current_unit.GetAttack(index);

        Vector3Int[] positions = attack.GetArea(Direction.None);
        Vector3Int unit_position = current_unit.GetPosition();

        map.SetMarker(target.x, target.y, 0);

        for (int i = 0; i < positions.Length; ++i)
            positions[i] += unit_position;

        for (int i = 0; i < positions.Length; ++i)
        {
            if (positions[i] == target)
            {
                map.SetMarker(positions[i].x, positions[i].y, 2);
            }
            else
            {
                map.SetMarker(positions[i].x, positions[i].y, 1);
            }
        }
    }

    public void ShowAttackTarget(Vector3Int target, int index)
    {
        map.RemoveAllMarker();

        Attack attack = current_unit.GetAttack(index);

        Vector3Int[] positions = attack.GetTarget(Direction.None);

        map.SetMarker(target.x, target.y, 0);

        for (int i = 0; i < positions.Length; ++i)
            positions[i] += target;

        for (int i = 0; i < positions.Length; ++i)
            map.SetMarker(positions[i].x, positions[i].y, 3);
    }

    public void RemoveMarker()
    {
        map.RemoveAllMarker();
    }

    //Checkers
    public bool PositionValid(Vector3Int position)
    {
        return map.IsInMap(position);
    }

    public bool MoveValid(Direction dir)
    {
        Vector3Int new_position = current_unit.GetPosition();

        if (dir == Direction.Up)
            new_position += new Vector3Int(0, 1, 0);
        else if (dir == Direction.Down)
            new_position += new Vector3Int(0, -1, 0);
        else if (dir == Direction.Right)
            new_position += new Vector3Int(1, 0, 0);
        else if (dir == Direction.Left)
            new_position += new Vector3Int(-1, 0, 0);

        return map.IsValidMove(current_unit, new_position);
    }

    public bool AttackTargetValid(Vector3Int target, int index)
    {
        Attack attack = current_unit.GetAttack(index);

        Vector3Int[] positions = attack.GetArea(Direction.None);
        Vector3Int unit_position = current_unit.GetPosition();

        for (int i = 0; i < positions.Length; ++i)
            if (positions[i] + unit_position == target)
                return true;

        return false;
    }

    //Grab Data From Unit
    public Vector3Int GetPosition(int id)
    {
        return current_unit.GetPosition();
    }
}