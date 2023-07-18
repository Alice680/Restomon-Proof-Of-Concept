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

    private DungeonType type;

    private Unit current_unit;

    private Unit player;
    private List<Unit> player_units;
    private Actor player_controller;

    private Unit enemy;
    private List<Unit> enemy_units;
    private Actor enemy_controller;

    private float attack_time;
    private List<GameObject> attack_moddels;

    private DungeonLayout current_floor;

    //Unit calls
    private void Start()
    {
        data_holder = GameObject.Find("DataHolder").GetComponent<PermDataHolder>();
        dungeon_ui = GameObject.Find("UIManager").GetComponent<DungeonUI>();

        turn_keeper = new TurnKeeper();
        player_units = new List<Unit>();
        enemy_units = new List<Unit>();
        attack_moddels = new List<GameObject>();

        player_controller = new Player(this);
        player = new Unit(data_holder.GetPlayer(), player_controller);

        current_floor = data_holder.GetDungeon();

        StartNewFloor();
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

    public void SpawnUnit(Vector3Int position)
    {
        if (current_unit.GetCreatureType() == CreatureType.Monster)
            return;
        if (current_unit.GetCreatureType() == CreatureType.Human)
            return;
        if (current_unit.GetCreatureType() == CreatureType.Restomon)
            return;

        Unit unit_temp = new Unit(current_floor.GetRandomCreature(), enemy_controller);

        enemy_units.Add(unit_temp);
        map.MoveUnit(position, unit_temp);
        turn_keeper.AddUnit(unit_temp);
    }

    public void EndTurn()
    {
        RemoveAttackModels();
        RemoveMarker();

        turn_keeper.NextTurn();

        current_unit = turn_keeper.Peak();

        moves = current_unit.GetStat(7);
        actions = current_unit.GetStat(8);

        dungeon_ui.UpdateUI(moves, actions);
    }

    //Internal data edit
    private void StartNewFloor()
    {
        map = current_floor.GenerateDungeon();

        enemy_controller = new AICore(current_floor.GetAI(), this);

        player_units.Add(player);
        map.MoveUnit(current_floor.GetStartPosition(), player);
        turn_keeper.AddUnit(player);

        enemy = new Unit(current_floor.GetDungeonManager(), enemy_controller);
        turn_keeper.AddUnit(enemy);

        dungeon_ui.Reset(map);
        dungeon_ui.UpdateCam(player.GetPosition());

        EndTurn();
    }

    private void ClearCurrentFloor()
    {

    }

    private void RemoveUnit(Unit unit)
    {
        map.RemoveUnit(unit);
        turn_keeper.RemoveUnit(unit);

        if (unit.GetOwner() == player_controller)
            player_units.Remove(unit);
        else
            enemy_units.Remove(unit);
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

    //Checker
    public bool PositionValid(Vector3Int position)
    {
        return map.IsInMap(position);
    }

    public bool TileEmpty(Vector3Int position)
    {
        if (map.GetTileType(position) == DungeonTileType.Wall)
            return false;

        if (map.GetUnit(position) != null)
            return false;

        return true;
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

    //Get Data
    public int GetMoves()
    {
        return moves;
    }

    public int GetActions()
    {
        return actions;
    }

    public Vector3Int GetMapSize()
    {
        return map.GetSize();
    }

    //Grab ID Data
    public int GetIDFromActive()
    {
        return current_unit.GetID();
    }

    public int GetIDFromPosition(Vector2Int position)
    {
        return 0;
    }

    //Grab Data From Unit
    private Unit GetUnitFromID(int id)
    {
        if (player.GetID() == id)
            return player;

        if (enemy.GetID() == id)
            return enemy;

        foreach (Unit unit in player_units)
            if (unit.GetID() == id)
                return unit;

        foreach (Unit unit in enemy_units)
            if (unit.GetID() == id)
                return unit;

        return null;
    }

    public CreatureType GetCreatureTypeFromID(int id)
    {
        return GetUnitFromID(id).GetCreatureType();
    }

    public Vector3Int GetPositionFromID(int id)
    {
        return GetUnitFromID(id).GetPosition();
    }
}