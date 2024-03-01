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
    private ManagerMenuHumanActions human_action_menu;
    private ManagerMenuRestomonActions restomon_action_menu;
    private DungeonTextHandler text_handler;
    private DungeonWeatherManager weather_manager;
    private StatusConditions condition_list;

    private DungeonLayout current_dungeon;
    private int current_floor;

    private DungeonMap map;
    private TurnKeeper turn_keeper;

    private Unit player;
    private List<Unit> player_units;
    private Actor player_controller;

    private Unit enemy;
    private List<Unit> enemy_units;
    private Actor enemy_controller;

    private int moves, actions;
    private Unit current_unit;
    private float attack_time;
    private List<GameObject> attack_moddels;
    private bool performed_action;

    private void Start()
    {
        data_holder = GameObject.Find("DataHolder").GetComponent<PermDataHolder>();
        dungeon_ui = GameObject.Find("UIManager").GetComponent<DungeonUI>();
        human_action_menu = GameObject.Find("UIManager").GetComponent<ManagerMenuHumanActions>();
        restomon_action_menu = GameObject.Find("UIManager").GetComponent<ManagerMenuRestomonActions>();
        text_handler = GameObject.Find("UIManager").GetComponent<DungeonTextHandler>();
        weather_manager = GameObject.Find("WeatherManager").GetComponent<DungeonWeatherManager>();
        condition_list = (StatusConditions)Resources.Load("Conditions");

        text_handler.SetUp();
        human_action_menu.SetDataHolder(data_holder);

        turn_keeper = new TurnKeeper();
        player_units = new List<Unit>();
        enemy_units = new List<Unit>();
        attack_moddels = new List<GameObject>();

        weather_manager.Setup(this, map, 0, 0);

        player_controller = new Player(this);
        player = new Unit(data_holder.GetPlayer(), player_controller);

        current_dungeon = data_holder.GetDungeon();
        current_floor = 0;

        StartNewFloor();
    }

    private void Update()
    {
        if (attack_time != -1 && Time.time - attack_time > 0.1f)
            RemoveAttackModels();

        if (current_unit != null)
            current_unit.GetOwner().Run();
        else
            EndTurn();

        if (performed_action)
        {
            ActionCleanup();
            text_handler.Run();
        }
    }

    /*
     * Actions
     */
    public void Move(Direction dir)
    {
        if (dir == Direction.None || !MoveValid(dir) || (moves == 0 && actions == 0) || performed_action)
            return;

        Vector3Int new_position = current_unit.GetPosition() + DirectionMath.GetVectorChange(dir);

        map.MoveUnit(new_position, current_unit);

        ApplyTrait.OnMove(current_unit, GetAllTraits(current_unit), this);

        performed_action = true;

        if (current_unit == player && map.GetTileItem(current_unit.GetPosition()) != -1)
            GrabItem(current_unit.GetPosition());

        if (moves > 0)
            moves = Mathf.Max(0, moves - 1);
        else if (actions > 0)
            actions = Mathf.Max(0, actions - 1);
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

        if (actions < 0)
            actions = 0;

        if (actions > 4)
            actions = 4;
    }

    public void SummonRestomon(Vector3Int position, int index)
    {
        if (!SummonValid(position, index))
            return;

        Catalyst temp_catalyst = data_holder.GetCatalyst();

        Restomon temp_restomon = data_holder.GetTeam(index);

        actions -= 1;

        current_unit.ChangeHp(-temp_catalyst.GetSummonCost(temp_restomon.GetSummonCost(RestomonEvolution.None, -1)));

        Unit temp_unit = new Unit(temp_restomon, current_unit.GetOwner());

        AddUnit(temp_unit, position);

        performed_action = true;
    }

    public void EvolveRestomon(Vector3Int position, int new_form)
    {
        if (!EvolutionValid(position, new_form))
            return;

        Unit temp_unit = map.GetUnit(position);

        current_unit.ChangeHp(-data_holder.GetCatalyst().GetEvolutionCost(temp_unit.GetEvolutionCost(new_form)));

        temp_unit.Evolve(new_form);
    }

    public void UseItem(Vector3Int target, int index)
    {
        Attack attack = ItemHolder.GetItem(data_holder.GetInventorySlot(index)).GetEffect(out bool has_effect);

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

        if (actions < 0)
            actions = 0;

        if (actions > 4)
            actions = 4;
    }

    public void SpawnRandomUnit(Vector3Int position)
    {
        if (current_unit.GetCreatureType() != CreatureType.Floor)
            return;

        current_unit.ChangeHp(-1);

        if (current_unit.GetHp() == 0)
        {
            WinCurrentFloor();
            return;
        }

        Unit unit_temp = new Unit(current_dungeon.GetFloor(current_floor).GetRandomCreature(), enemy_controller);

        AddUnit(unit_temp, position);
    }

    public void EndTurn()
    {
        if (performed_action)
            return;

        RemoveMarker();

        if (current_unit != null && current_unit.GetCreatureType() != CreatureType.Floor)
            ApplyTrait.EndTurn(current_unit, GetAllTraits(current_unit), this);

        turn_keeper.NextTurn();

        current_unit = turn_keeper.Peak();

        SetUpUnit();

        dungeon_ui.UpdateActions(0, 0);

        performed_action = true;
    }

    /*
     * Internal data edits
     */
    private void EndCurrentFloor()
    {
        player_units.Remove(player);
        turn_keeper.RemoveUnit(player);

        while (player_units.Count != 0)
            RemoveUnit(player_units[0]);

        enemy = null;

        while (enemy_units.Count != 0)
            RemoveUnit(enemy_units[0]);

        map.Clear();
    }

    private void StartNewFloor()
    {
        Vector3Int start_position;
        Creature[] temp_enemy;
        Vector3Int[] temp_enemy_positions;

        map = current_dungeon.GetFloor(current_floor).GenerateDungeon(weather_manager, data_holder, out start_position, out temp_enemy, out temp_enemy_positions);

        enemy_controller = new AICore(current_dungeon.GetFloor(current_floor).GetAI(), this);
        AddUnit(player, start_position);

        enemy = new Unit(current_dungeon.GetFloor(current_floor).GetDungeonManager(), enemy_controller);

        AddUnit(enemy, new Vector3Int(-1, -1, 0));

        for (int i = 0; i < temp_enemy.Length; ++i)
        {
            AddUnit(new Unit(temp_enemy[i], enemy_controller), temp_enemy_positions[i]);
        }

        dungeon_ui.Reset(map);

        performed_action = false;

        EndTurn();
    }

    private void AddUnit(Unit unit, Vector3Int position)
    {
        if (unit.GetOwner() == player_controller)
            player_units.Add(unit);
        else
            enemy_units.Add(unit);

        turn_keeper.AddUnit(unit);

        if (position.x > 0 && position.y > 0)
            map.MoveUnit(position, unit);

        ApplyTrait.OnSpawn(unit, GetAllTraits(unit), this);
    }

    private void RemoveUnit(Unit unit)
    {
        map.RemoveUnit(unit);
        turn_keeper.RemoveUnit(unit);

        if (unit == player)
            LoseDungeon();
        if (unit == enemy)
            WinCurrentFloor();

        if (unit.GetOwner() == player_controller)
            player_units.Remove(unit);
        else
            enemy_units.Remove(unit);

        unit.KillUnit();

        performed_action = true;
    }

    private void GrabItem(Vector3Int position)
    {
        int temp_index = map.GetTileItem(position);

        if (temp_index == -1)
            return;

        string temp_name = ItemHolder.GetItem(temp_index).GetInfo(out string temp_description);

        if (data_holder.GetInventoryCount() < data_holder.GetInventorySize())
        {
            data_holder.AddInventory(temp_index);
            map.SetTileItem(position, -1);
            DungeonTextHandler.AddText("Picked up " + temp_name);
        }
        else
        {
            DungeonTextHandler.AddText("Found " + temp_name + " but inventory was already full.");
        }
    }

    public void ChangeMovement(int value)
    {
        moves += value;


        if (moves < 0)
            moves = 0;

        if (moves > 10)
            moves = 10;

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

    public void WinCurrentFloor()
    {
        ++current_floor;

        EndCurrentFloor();

        if (current_dungeon.GetNumberOfFloor() == current_floor)
            WinDungeon();
        else
            StartNewFloor();
    }

    public void WinDungeon()
    {
        current_dungeon.SetVictory(data_holder, true);
        SceneManager.LoadScene(3);
    }

    public void LoseDungeon()
    {
        current_dungeon.SetVictory(data_holder, false);
        SceneManager.LoadScene(3);
    }

    private void SetUpUnit()
    {
        current_unit.StartTurn();

        moves = current_unit.GetStat(7);
        actions = current_unit.GetStat(8);

        if (current_unit.GetCreatureType() == CreatureType.Human)
        {
            List<Unit> temp_units = (current_unit == player) ? player_units : enemy_units;

            foreach (Unit temp_unit in temp_units)
                current_unit.ChangeHp(-data_holder.GetCatalyst().GetMaintenanceCost(temp_unit.GetMaintenanceCost()));
        }
        else if (current_unit.GetCreatureType() == CreatureType.Restomon)
        {
            Unit temp_parent = enemy;

            foreach (Unit temp_unit in player_units)
                if (current_unit == temp_unit)
                    temp_parent = player;

            if (temp_parent.GetCreatureType() == CreatureType.Human)
                temp_parent.ChangeHp(-data_holder.GetCatalyst().GetMaintenanceCost(current_unit.GetUpkeepCost()));
        }

        if (current_unit.GetCreatureType() != CreatureType.Floor)
            ApplyTrait.StartTurn(current_unit, GetAllTraits(current_unit), this);
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

    public void ShowCamera(Vector3Int target)
    {
        dungeon_ui.UpdateCam(target);
    }

    public void ShowView(Vector3Int target)
    {
        map.RemoveAllMarker();

        Unit temp_unit = map.GetUnit(target);

        dungeon_ui.UpdateUnitStatus(temp_unit);

        if (temp_unit == null)
            map.SetMarker(target.x, target.y, 0);
        else if (temp_unit.GetOwner() == current_unit.GetOwner())
            map.SetMarker(target.x, target.y, 2);
        else
            map.SetMarker(target.x, target.y, 3);
    }

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

    public void ShownSummonTarget(Vector3Int target, int index)
    {
        map.RemoveAllMarker();

        dungeon_ui.UpdateUnitStatus(map.GetUnit(target));

        Vector3Int[] positions = data_holder.GetCatalyst().GetArea(current_unit.GetPosition());

        for (int i = 0; i < positions.Length; ++i)
            map.SetMarker(positions[i].x, positions[i].y, 1);

        if (SummonValid(target, index))
            map.SetMarker(target.x, target.y, 2);
        else
            map.SetMarker(target.x, target.y, 3);
    }

    public void ShowEvolutionTarget(Vector3Int previous, Vector3Int target, int new_form)
    {
        map.RemoveAllMarker();

        dungeon_ui.UpdateUnitStatus(map.GetUnit(target));

        Vector3Int[] positions = data_holder.GetCatalyst().GetArea(current_unit.GetPosition());

        for (int i = 0; i < positions.Length; ++i)
            map.SetMarker(positions[i].x, positions[i].y, 1);

        if (EvolutionValid(target, new_form))
            map.SetMarker(target.x, target.y, 2);
        else
            map.SetMarker(target.x, target.y, 3);

        Unit temp_unit = map.GetUnit(previous);

        if (temp_unit != null && temp_unit.GetCreatureType() == CreatureType.Restomon)
            temp_unit.ShowEvolution(-1);

        temp_unit = map.GetUnit(target);

        if (temp_unit != null && temp_unit.GetCreatureType() == CreatureType.Restomon && temp_unit.GetCurrentEvolution() == RestomonEvolution.None)
            temp_unit.ShowEvolution(new_form);
    }

    public void ShowItemArea(Vector3Int target, int index)
    {
        map.RemoveAllMarker();

        dungeon_ui.UpdateUnitStatus(map.GetUnit(target));

        Attack attack = ItemHolder.GetItem(data_holder.GetInventorySlot(index)).GetEffect(out bool has_effect);

        Vector3Int unit_position = current_unit.GetPosition();

        Vector3Int[] area_positions = attack.GetArea(unit_position);
        Vector3Int[] target_positions = attack.GetTarget(target, DirectionMath.GetDirectionChange(current_unit.GetPosition(), target));

        map.SetMarker(target.x, target.y, 0);

        for (int i = 0; i < area_positions.Length; ++i)
            map.SetMarker(area_positions[i].x, area_positions[i].y, 1);

        for (int i = 0; i < target_positions.Length; ++i)
            map.SetMarker(target_positions[i].x, target_positions[i].y, 3);
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
        Vector3Int new_position = current_unit.GetPosition() + DirectionMath.GetVectorChange(dir);

        return map.IsValidMove(current_unit, new_position);
    }

    public bool AttackTargetValid(Vector3Int target, int index)
    {
        Vector3Int[] positions = current_unit.GetAttack(index).GetArea(current_unit.GetPosition());

        for (int i = 0; i < positions.Length; ++i)
            if (positions[i] == target)
                return true;

        return false;
    }

    public bool SummonValid(int index)
    {
        Restomon temp_restomon = data_holder.GetTeam(index);
        Catalyst temp_catalyst = data_holder.GetCatalyst();

        if (actions == 0)
            return false;

        if (current_unit.GetCreatureType() != CreatureType.Human)
            return false;

        if (index >= temp_catalyst.GetRestomonAmount())
            return false;

        if (player_units.Count > temp_catalyst.GetTeamSize())
            return false;

        if (current_unit.GetHp() < temp_catalyst.GetSummonCost(temp_restomon.GetSummonCost(RestomonEvolution.None, -1)))
            return false;

        return true;
    }

    public bool SummonValid(Vector3Int target, int index)
    {
        if (!SummonValid(index))
            return false;

        if (!PositionValid(target) || !TileEmpty(target))
            return false;

        Vector3Int[] positions = data_holder.GetCatalyst().GetArea(current_unit.GetPosition());

        for (int i = 0; i < positions.Length; ++i)
            if (positions[i] == target)
                return true;

        return false;
    }

    public bool EvolutionValid(Vector3Int position, int new_form)
    {
        Catalyst temp_catalyst = data_holder.GetCatalyst();

        if (new_form < 0 || new_form > 2 || !map.IsInMap(position))
            return false;

        Unit temp_unit = map.GetUnit(position);

        if (temp_unit == null || temp_unit.GetCreatureType() != CreatureType.Restomon || temp_unit.GetCurrentEvolution() != RestomonEvolution.None)
            return false;

        if (temp_catalyst.GetEvolutionCost(temp_unit.GetEvolutionCost(new_form)) > current_unit.GetHp())
            return false;

        return true;
    }

    public bool ItemTargetValid(Vector3Int target, int index)
    {
        if (current_unit != player || actions == 0)
            return false;

        Attack attack = ItemHolder.GetItem(data_holder.GetInventorySlot(index)).GetEffect(out bool has_effect);

        if (!has_effect)
            return false;

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

    public ManagerMenuHumanActions GetHumanActionMenu()
    {
        return human_action_menu;
    }

    public ManagerMenuRestomonActions GetRestomonActionMenu()
    {
        return restomon_action_menu;
    }

    public DungeonMap GetMap()
    {
        return map;
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

    private Trait[] GetAllTraits(Unit unit)
    {
        List<Trait> trait_list = new List<Trait>();

        if (unit.GetCreatureType() == CreatureType.Floor)
            return trait_list.ToArray();

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

    public string GetAttackDescription(int id, int index)
    {
        return GetUnitFromID(id).GetAttack(index).GetDescription();
    }

    public int GetAttackCost(int id, int index)
    {
        return GetUnitFromID(id).GetAttack(index).GetCost(0);
    }

    public int GetAttackMP(int id, int index)
    {
        return GetUnitFromID(id).GetAttack(index).GetCost(1);
    }
}