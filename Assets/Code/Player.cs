using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    private enum State { startup, idle, aim_attack, view_attack };

    private DungeonManager manager_ref;

    private Inputer inputer;

    private State state;

    private Vector3Int target;

    private int attack_num;

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
            if (manager_ref.GetActions() == 0)
                return;
            
            target = manager_ref.GetPosition(0);

            attack_num = 0;

            manager_ref.ShowAttackArea(target, attack_num);

            state = State.aim_attack;
            return;
        }

        if(inputer.GetBack())
        {
            manager_ref.EndTurn();
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

            manager_ref.ShowAttackArea(target, attack_num);

            return;
        }

        if (inputer.GetEnter())
        {
            if (manager_ref.AttackTargetValid(target, attack_num))
            {
                manager_ref.RemoveMarker();

                manager_ref.ShowAttackTarget(target, attack_num);

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
            manager_ref.Attack(target, attack_num);

            manager_ref.RemoveMarker();

            state = State.idle;
        }

        if (inputer.GetBack())
        {
            manager_ref.RemoveMarker();

            manager_ref.ShowAttackArea(target, attack_num);

            state = State.aim_attack;
            return;
        }
    }
}