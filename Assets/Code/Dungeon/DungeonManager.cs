using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    private PermDataHolder data_holder;
    private DungeonUI dungeon_ui;
    private ActionMenu action_menu;

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
        action_menu = GameObject.Find("UIManager").GetComponent<ActionMenu>();

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

        --moves;

        if (current_unit.GetOwner() == player_controller)
        {
            dungeon_ui.UpdateActions(moves, actions);
            dungeon_ui.UpdateCam(new_position);
        }
    }

    public void Attack(Vector3Int target, int index)
    {
        if (!AttackTargetValid(target, index) || actions == 0)
            return;

        Attack attack = current_unit.GetAttack(index);
        GameObject marker = attack.GetModel();

        if (!attack.PayCost(current_unit))
            return;

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

        attack.ApplyEffect(current_unit, attack_targets.ToArray(), positions, map, this);

        foreach (Unit unit in attack_targets)
            if (unit.GetHp() <= 0)
                RemoveUnit(unit);

        --actions;

        if (current_unit.GetOwner() == player_controller)
            dungeon_ui.UpdateActions(moves, actions);
        dungeon_ui.UpdatePlayerStats(player, player_units);
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

        current_unit.StartTurn();

        moves = current_unit.GetStat(7);
        actions = current_unit.GetStat(8);

        if (current_unit.GetOwner() == player_controller)
            dungeon_ui.UpdateActions(moves, actions);
        else
            dungeon_ui.UpdateActions(0, 0);

        dungeon_ui.UpdatePlayerStats(player, player_units);
    }

    public void WinDungeon()
    {
        SceneManager.LoadScene(1);
    }

    public void LoseDungeon()
    {
        SceneManager.LoadScene(1);
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
        dungeon_ui.UpdatePlayerStats(player, player_units);

        EndTurn();
    }

    private void ClearCurrentFloor()
    {

    }

    private void RemoveUnit(Unit unit)
    {
        map.RemoveUnit(unit);
        turn_keeper.RemoveUnit(unit);

        if (unit.GetID() == player.GetID())
            LoseDungeon();
        if (unit.GetID() == enemy.GetID())
            WinDungeon();

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

    //External data edit
    public void ChangeMovement(int value)
    {
        moves += value;

        if (moves < 0)
            moves = 0;

        dungeon_ui.UpdateActions(moves, actions);
    }

    public void ChangeAction(int value)
    {
        actions += value;

        if (actions < 0)
            actions = 0;

        if (actions > 4)
            actions = 4;

        dungeon_ui.UpdateActions(moves, actions);
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

    public Vector3Int[] GetPath(Vector3Int start, Vector3Int goal)
    {
        return Pathfinding.GetPath(map, start, goal);
    }

    public ActionMenu GetActionMenu()
    {
        return action_menu;
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

    public int[] GetPlayerIDS()
    {
        int[] ids = new int[1 + player_units.Count];

        ids[0] = player.GetID();

        for (int i = 1; i <= player_units.Count; ++i)
            ids[i] = player_units[i - 1].GetID();

        return ids;
    }

    public int[] GetEnemyIDS()
    {
        int[] ids = new int[1 + enemy_units.Count];

        ids[0] = enemy.GetID();

        for (int i = 1; i <= enemy_units.Count; ++i)
            ids[i] = enemy_units[i - 1].GetID();

        return ids;
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

    public int GetHP(int id)
    {
        return GetUnitFromID(id).GetHp();
    }

    public string GetAttackName(int id, int index)
    {
        return GetUnitFromID(id).GetAttack(index).GetName();
    }

    public string GetAttackEffect(int id, int index)
    {
        return GetUnitFromID(id).GetAttack(index).GetDescription();
    }

    public int GetAttackCost(int id, int index)
    {
        return GetUnitFromID(id).GetAttack(index).GetCost(0);
    }

}