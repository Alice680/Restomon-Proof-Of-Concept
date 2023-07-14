using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private PermDataHolder data_holder;
    private DungeonUI dungeon_ui;

    private DungeonMap map;
    private TurnKeeper turn_keeper;

    private int moves, actions;

    private Unit player;
    private Unit current_unit;
    private List<Unit> units;

    private float attack_time;
    private List<GameObject> attack_moddels;

    private Actor player_controller;
    private Actor enemy_controller;

    //Unit calls
    private void Start()
    {
        data_holder = GameObject.Find("DataHolder").GetComponent<PermDataHolder>();
        dungeon_ui = GameObject.Find("UIManager").GetComponent<DungeonUI>();

        turn_keeper = new TurnKeeper();
        units = new List<Unit>();
        attack_moddels = new List<GameObject>();

        player_controller = new Player(this);
        player = new Unit(data_holder.GetPlayer(), player_controller);

        StartNewArea(data_holder.GetDungeon());
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
        if (dir == Direction.None || !MoveValid(dir) || moves == 0)
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

        dungeon_ui.UpdateCam(new_position);

        --moves;
        dungeon_ui.UpdateUI(moves, actions);
    }

    public void Attack(Vector3Int target, int index)
    {
        if (!AttackTargetValid(target, index) || actions == 0)
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

        attack.ApplyEffect(current_unit, attack_targets.ToArray(), positions, map);

        foreach (Unit unit in attack_targets)
            if (unit.GetHp() <= 0)
                RemoveUnit(unit);

        --actions;
        dungeon_ui.UpdateUI(moves, actions);
    }

    public void EndTurn()
    {
        RemoveAttackModels();
        RemoveMarker();

        turn_keeper.NextTurn();

        current_unit = turn_keeper.Peak();

        moves = current_unit.GetStat(7);
        actions = current_unit.GetStat(8);

        Debug.Log(turn_keeper.Peak().GetID());

        dungeon_ui.UpdateUI(moves, actions);
    }

    //Internal data edit
    private void StartNewArea(DungeonLayout layout)
    {
        map = layout.GenerateDungeon();

        enemy_controller = new AICore(layout.GetAI(), this);

        units.Add(player);
        map.MoveUnit(layout.GetStartPosition(), player);
        turn_keeper.AddUnit(player);

        dungeon_ui.Reset(map);
        dungeon_ui.UpdateCam(player.GetPosition());

        EndTurn();
    }

    private void ClearCurrentArea()
    {

    }

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

    public int GetMoves()
    {
        return moves;
    }

    public int GetActions()
    {
        return actions;
    }

    //Grab Data From Unit
    public Vector3Int GetPosition(int id)
    {
        return current_unit.GetPosition();
    }
}