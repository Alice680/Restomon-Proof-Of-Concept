using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private DungeonMap map;
    private List<Unit> units;
    private List<Actor> actors;


    private Unit current_unit;

    //temp
    public DungeonLayout layout;
    public GameObject player_model;
    public Player controller;

    private void Start()
    {
        map = layout.GenerateDungeon();

        units = new List<Unit>();
        actors = new List<Actor>();

        controller = new Player(this);

        current_unit = new Unit(player_model, controller);
        map.MoveUnit(5, 5, current_unit);

        units.Add(current_unit);
        actors.Add(controller);
    }

    private void Update()
    {
        current_unit.GetOwner().Run();
    }

    public void Move(Direction dir)
    {
        Vector3Int vec = current_unit.GetPosition();

        if (dir == Direction.Up)
            vec += new Vector3Int(0, 1, 0);
        else if (dir == Direction.Down)
            vec += new Vector3Int(0, -1, 0);
        else if (dir == Direction.Right)
            vec += new Vector3Int(1, 0, 0);
        else if (dir == Direction.Left)
            vec += new Vector3Int(-1, 0, 0);

        //Update later
        if (map.GetTileType(vec.x, vec.y) == DungeonTileType.Ground)
            map.MoveUnit(vec.x, vec.y, current_unit);
    }
}