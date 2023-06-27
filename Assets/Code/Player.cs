using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    private class Inputer
    {
        private Direction dir;
        private bool enter;
        private bool back;

        private float last_input;

        public void Run()
        {
            dir = Direction.None;
            enter = false;
            back = false;

            if (Time.time - last_input <= 0.1F)
                return;

            if (Input.GetKey(KeyCode.W))
                dir = Direction.Up;
            if (Input.GetKey(KeyCode.D))
                dir = Direction.Right;
            if (Input.GetKey(KeyCode.S))
                dir = Direction.Down;
            if (Input.GetKey(KeyCode.A))
                dir = Direction.Left;

            if (Input.GetKeyDown(KeyCode.Return) && Time.time - last_input > 0.15)
                enter = true;

            if (Input.GetKeyDown(KeyCode.Escape) && Time.time - last_input > 0.15)
                back = true;

            if (dir != Direction.None || enter || back)
                last_input = Time.time;
        }

        public Direction GetDir()
        {
            return dir;
        }

        public bool GetEnter()
        {
            return enter;
        }

        public bool GetBack()
        {
            return back;
        }
    }

    private enum State { startup, idle, aim_attack, view_attack };

    private State state;

    private Vector3Int target;

    private DungeonManager manager_ref;
    private Inputer inputer;

    public Player(DungeonManager manager_ref)
    {
        this.manager_ref = manager_ref;

        inputer = new Inputer();

        state = new State();
        state = State.startup;
    }

    public override void Run()
    {
        inputer.Run();

        switch (state)
        {
            case State.startup:
                Startup();
                break;
            case State.idle:
                Idle();
                break;
            case State.aim_attack:
                AimAttack();
                break;
            case State.view_attack:
                ViewAttack();
                break;
        }
    }

    //States
    private void Startup()
    {
        state = State.idle;
    }

    private void Idle()
    {
        if (inputer.GetDir() != Direction.None)
        {
            if (manager_ref.MoveValid(inputer.GetDir()))
                manager_ref.Move(inputer.GetDir());
            return;
        }

        if (inputer.GetEnter())
        {
            target = manager_ref.GetPosition(0);

            manager_ref.ShowAttackArea(target);

            state = State.aim_attack;
            return;
        }
    }

    private void AimAttack()
    {
        if (inputer.GetDir() != Direction.None)
        {
            if (inputer.GetDir() == Direction.Up && manager_ref.PositionValid(target + new Vector3Int(0, 1, 0)))
                target += new Vector3Int(0, 1, 0);
            else if (inputer.GetDir() == Direction.Down && manager_ref.PositionValid(target + new Vector3Int(0, -1, 0)))
                target += new Vector3Int(0, -1, 0);
            else if (inputer.GetDir() == Direction.Right && manager_ref.PositionValid(target + new Vector3Int(1, 0, 0)))
                target += new Vector3Int(1, 0, 0);
            else if (inputer.GetDir() == Direction.Left && manager_ref.PositionValid(target + new Vector3Int(-1, 0, 0)))
                target += new Vector3Int(-1, 0, 0);

            manager_ref.ShowAttackArea(target);

            return;
        }

        if (inputer.GetEnter())
        {
            if (manager_ref.AttackTargetValid(target))
            {
                manager_ref.RemoveMarker();

                manager_ref.ShowAttackTarget(target);

                state = State.view_attack;
            }

            return;
        }

        if (inputer.GetBack())
        {
            target = new Vector3Int();

            manager_ref.RemoveMarker();

            state = State.idle;
            return;
        }
    }

    private void ViewAttack()
    {
        if (inputer.GetEnter())
        {
            manager_ref.Attack(target);

            manager_ref.RemoveMarker();

            state = State.idle;
        }

        if (inputer.GetBack())
        {
            manager_ref.RemoveMarker();

            manager_ref.ShowAttackArea(target);

            state = State.aim_attack;
            return;
        }
    }
}