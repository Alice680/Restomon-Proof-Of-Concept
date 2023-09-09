using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * The main class for dungeon game phase.
 * It is a modeled as a sort of sudo server. Giving temporary controll to players when it is there turn.
 * While it is a players turn, they can call one of a handfull of "action" methods.
 * Each one of these actions then verfies if the call is valid and applies the changes if so.
 * 
 * The method also manages what data can be seen by players via an ID system.
 * 
 * Notes:
 */
public class DungeonManager : MonoBehaviour
{
    private PermDataHolder data_holder;
    private DungeonUI dungeon_ui;
    private ManagerMenuActions action_menu;
    private DungeonWeatherManager weather_manager;
    private StatusConditions condition_list;

    private DungeonMap map;
    private TurnKeeper turn_keeper;

    private Unit player;
    private List<Unit> player_units;
    private Actor player_controller;

    private Unit enemy;
    private List<Unit> enemy_units;
    private Actor enemy_controller;

    private DungeonLayout current_floor;

    private int moves, actions;
    private Unit current_unit;
    private float attack_time;
    private List<GameObject> attack_moddels;
    private bool performed_action;

    private void Start()
    {
        data_holder = GameObject.Find("DataHolder").GetComponent<PermDataHolder>();
        dungeon_ui = GameObject.Find("UIManager").GetComponent<DungeonUI>();
        action_menu = GameObject.Find("UIManager").GetComponent<ManagerMenuActions>();
        weather_manager = GameObject.Find("WeatherManager").GetComponent<DungeonWeatherManager>();
        condition_list = (StatusConditions)Resources.Load("Conditions");

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

        if (performed_action)
            ActionCleanup();
    }

    /*
     * Actions
     */
    public void Move(Direction dir)
    {
        if (dir == Direction.None || !MoveValid(dir) || moves == 0 || performed_action)
            return;

        Vector3Int new_position = current_unit.GetPosition() + DirectionMath.GetVectorChange(dir);

        map.MoveUnit(new_position, current_unit);

        ApplyTrait.OnMove(current_unit, GetAllTraits(current_unit), this);

        performed_action = true;
        --moves;
    }

    public void Attack(Vector3Int target, int index)
    {
        if (!AttackTargetValid(target, index) || actions == 0 || performed_action)
            return;

        Attack attack = current_unit.GetAttack(index);

        if (!ApplyAttack.TryPayCost(attack, current_unit))
            return;

        List<Unit> attack_targets = new List<Unit>();
        List<Trait[]> trait_targets = new List<Trait[]>();

        RemoveAttackModels();

        Vector3Int[] positions = attack.GetTarget(target, DirectionMath.GetDirectionChange(current_unit.GetPosition(), target));

        for (int i = 0; i < positions.Length; ++i)
        {
            attack_moddels.Add(Instantiate(attack.GetModel(), positions[i], new Quaternion()));

            if (map.GetUnit(positions[i]) != null)
            {
                attack_targets.Add(map.GetUnit(positions[i]));
                trait_targets.Add(GetAllTraits(map.GetUnit(positions[i])));
            }
        }

        ApplyAttack.ApplyEffect(attack, current_unit, GetAllTraits(current_unit), attack_targets.ToArray(), trait_targets, positions, map, this);

        attack_time = Time.time;
        performed_action = true;
        --actions;
    }

    // TODO Clean up dungoen spawnning with dungeon V2
    public void SpawnUnit(Vector3Int position)
    {
        if (current_unit.GetCreatureType() != CreatureType.Arena)
            return;

        Unit unit_temp = new Unit(current_floor.GetRandomCreature(), enemy_controller);

        enemy_units.Add(unit_temp);
        map.MoveUnit(position, unit_temp);
        turn_keeper.AddUnit(unit_temp);
    }

    public void EndTurn()
    {
        if (performed_action)
            return;

        RemoveMarker();

        if (current_unit != null && current_unit.GetCreatureType() != CreatureType.Arena)
            ApplyTrait.EndTurn(current_unit, GetAllTraits(current_unit), this);

        turn_keeper.NextTurn();

        current_unit = turn_keeper.Peak();

        current_unit.StartTurn();

        moves = current_unit.GetStat(7);
        actions = current_unit.GetStat(8);

        if (current_unit.GetCreatureType() != CreatureType.Arena)
            ApplyTrait.StartTurn(current_unit, GetAllTraits(current_unit), this);

        dungeon_ui.UpdateActions(0, 0);

        performed_action = true;
    }

    /*
     * Internal data edits
     */
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

        EndTurn();
    }

    // TODO add in with random dungeons
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

        unit.KillUnit();
    }

    private void RemoveAttackModels()
    {
        attack_time = -1;

        foreach (GameObject obj in attack_moddels)
            Destroy(obj);

        attack_moddels.Clear();
    }

    private void UpdatePlayerUI()
    {
        dungeon_ui.UpdateActions(moves, actions);
        dungeon_ui.UpdateCam(current_unit.GetPosition());
    }

    private void UpdateUI()
    {
        dungeon_ui.UpdatePlayerStats(player, player_units);
    }

    private void ActionCleanup()
    {
        if (current_unit.GetOwner() == player_controller)
            UpdatePlayerUI();

        UpdateUI();

        foreach (Unit unit in GetAllUnits())
            if (unit.GetHp() == 0)
            {
                ApplyTrait.OnKill(current_unit, unit, GetAllTraits(current_unit), GetAllTraits(unit), this);

                RemoveUnit(unit);
            }

        if (current_unit.GetHp() == 0)
        {
            current_unit = null;
            EndTurn();
        }

        performed_action = false;
    }

    // TODO Remove all public setters by finishing assosiated systems or turning them into actions
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

    public void WinDungeon()
    {
        SceneManager.LoadScene(1);
    }

    public void LoseDungeon()
    {
        SceneManager.LoadScene(1);
    }

    // TODO maybe put into dungeon UI
    public void ShowAttackArea(Vector3Int target, int index)
    {
        map.RemoveAllMarker();

        dungeon_ui.UpdateUnitStatus(map.GetUnit(target));

        Attack attack = current_unit.GetAttack(index);

        Vector3Int unit_position = current_unit.GetPosition();

        Vector3Int[] positions = attack.GetArea(unit_position);

        map.SetMarker(target.x, target.y, 0);

        for (int i = 0; i < positions.Length; ++i)
        {
            if (positions[i] == target)
                map.SetMarker(positions[i].x, positions[i].y, 2);
            else
                map.SetMarker(positions[i].x, positions[i].y, 1);
        }
    }

    public void ShowAttackTarget(Vector3Int target, int index)
    {
        map.RemoveAllMarker();

        dungeon_ui.UpdateUnitStatus(map.GetUnit(target));

        Attack attack = current_unit.GetAttack(index);

        Vector3Int[] positions = attack.GetTarget(target, DirectionMath.GetDirectionChange(current_unit.GetPosition(), target));

        map.SetMarker(target.x, target.y, 0);

        for (int i = 0; i < positions.Length; ++i)
            map.SetMarker(positions[i].x, positions[i].y, 3);
    }

    public void RemoveMarker()
    {
        dungeon_ui.UpdateUnitStatus(null);
        map.RemoveAllMarker();
    }

    /* 
     * External getter
     */
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

        Vector3Int[] positions = attack.GetArea(current_unit.GetPosition());

        for (int i = 0; i < positions.Length; ++i)
            if (positions[i] == target)
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

    public Vector3Int GetMapSize()
    {
        return map.GetSize();
    }

    public Vector3Int[] GetPath(Vector3Int start, Vector3Int goal)
    {
        return Pathfinding.GetPath(map, start, goal);
    }

    public ManagerMenuActions GetActionMenu()
    {
        return action_menu;
    }

    /*
     * Internal getter
     */
    private Unit[] GetAllUnits()
    {
        List<Unit> units = new List<Unit>();

        foreach (Unit unit in player_units)
            units.Add(unit);

        foreach (Unit unit in enemy_units)
            units.Add(unit);

        return units.ToArray();
    }

    // TODO grab trait from conditions
    private Trait[] GetAllTraits(Unit unit)
    {
        List<Trait> trait_list = new List<Trait>();

        foreach (Trait trait in unit.GetTraits())
            trait_list.Add(trait);

        trait_list.Add(weather_manager.GetTrait());

        trait_list.Add(map.GetTileTrait(unit.GetPosition()));

        int index = -1, rank = -1;
        for (int i = 0; i < unit.GetNumConditions(); ++i)
        {
            index = unit.GetCondition(i, out rank);
            trait_list.Add(condition_list.GetTrait(index, rank));
        }

        return trait_list.ToArray();
    }

    /*
     * ID system
     */
    //TODO Considering turning the ID system into it's own class.
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