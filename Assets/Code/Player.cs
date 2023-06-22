using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    DungeonManager manager_ref;

    public Player(DungeonManager manager_ref)
    {
        this.manager_ref = manager_ref;
    }

    public override void Run()
    {
        if(Input.GetKeyDown(KeyCode.W))
            manager_ref.Move(Direction.Up);
        if (Input.GetKeyDown(KeyCode.D))
            manager_ref.Move(Direction.Right);
        if (Input.GetKeyDown(KeyCode.S))
            manager_ref.Move(Direction.Down);
        if (Input.GetKeyDown(KeyCode.A))
            manager_ref.Move(Direction.Left);
    }
}