using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The actor controll by the player.
 * It uses a simple state machine of sorts to handle to various menus and the like.
 * 
 * Notes:
 * Once the action types are finalized, move the actions into their own methods to improve readbility
 */
// TODO move actions into their own methods.
// TODO restrcuture into a main method that splits off based on state and input.
public class Player : Actor
{
    private enum State { startup, idle, human_action_ui, restomon_action_ui, view, aim_attack, view_attack, view_summon, view_evolution };

    private DungeonManager manager_ref;

    private ManagerMenuHumanActions human_action_ref;
    private ManagerMenuRestomonActions restomon_action_ref;

    private Inputer inputer;

    private State state;

    private Vector3Int target;

    private int action_num;

    public Player(DungeonManager manager_ref)
    {
        this.manager_ref = manager_ref;

        human_action_ref = manager_ref.GetHumanActionMenu();
        restomon_action_ref = manager_ref.GetRestomonActionMenu();

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
            case State.human_action_ui:
                HumanActionUI();
                break;
            case State.restomon_action_ui:
                RestomonActionUI();
                break;
            case State.view:
                View();
                break;
            case State.aim_attack:
                AimAttack();
                break;
            case State.view_attack:
                ViewAttack();
                break;
            case State.view_summon:
                ViewSummon();
                break;
            case State.view_evolution:
                ViewEvolution();
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

            action_num = 0;

            target = manager_ref.GetPositionFromID(manager_ref.GetIDFromActive());

            manager_ref.ShowAttackArea(target, action_num);

            state = State.aim_attack;
            return;
        }

        if (inputer.GetActionOne())
        {
            if (manager_ref.GetCreatureTypeFromID(manager_ref.GetIDFromActive()) == CreatureType.Human)
            {
                human_action_ref.OpenActionMenu();
                state = State.human_action_ui;
            }
            else if (manager_ref.GetCreatureTypeFromID(manager_ref.GetIDFromActive()) == CreatureType.Restomon)
            {
                restomon_action_ref.OpenActionMenu();
                state = State.restomon_action_ui;
            }
            return;
        }

        if (inputer.GetBack())
        {
            inputer.Clear();
            manager_ref.EndTurn();
            return;
        }
    }

    private void HumanActionUI()
    {
        if (inputer.GetDir() != Direction.None && inputer.GetMoveKeyUp())
        {
            human_action_ref.DirectionMenu(inputer.GetDir());

            return;
        }

        if (inputer.GetEnter())
        {
            int exit_value;
            int exit_state = human_action_ref.EnterMenu(out exit_value);

            switch (exit_state)
            {
                case 0:
                    target = manager_ref.GetPositionFromID(manager_ref.GetIDFromActive());

                    manager_ref.ShowView(target);

                    state = State.view;
                    return;

                case 1:
                    action_num = exit_value;

                    target = manager_ref.GetPositionFromID(manager_ref.GetIDFromActive());

                    manager_ref.ShowAttackArea(target, action_num);

                    state = State.aim_attack;
                    return;

                case 2:
                    action_num = exit_value;

                    target = manager_ref.GetPositionFromID(manager_ref.GetIDFromActive());

                    manager_ref.ShownSummonTarget(target, action_num);

                    state = State.view_summon;
                    return;

                case 3:
                    action_num = 0;

                    target = manager_ref.GetPositionFromID(manager_ref.GetIDFromActive());

                    manager_ref.ShowEvolutionTarget(target, target, action_num);

                    state = State.view_evolution;

                    return;

                case 4:

                    return;

                case 5:
                    manager_ref.LoseDungeon();
                    return;
            }

            return;
        }

        if (inputer.GetBack())
        {
            if (human_action_ref.ReturnMenu())
                state = State.idle;

            return;
        }
    }

    private void RestomonActionUI()
    {
        if (inputer.GetDir() != Direction.None && inputer.GetMoveKeyUp())
        {
            restomon_action_ref.DirectionMenu(inputer.GetDir());

            return;
        }

        if (inputer.GetEnter())
        {
            int exit_value;
            int exit_state = restomon_action_ref.EnterMenu(out exit_value);

            switch (exit_state)
            {
                case 0:
                    target = manager_ref.GetPositionFromID(manager_ref.GetIDFromActive());

                    manager_ref.ShowView(target);

                    state = State.view;
                    return;

                case 1:
                    action_num = exit_value;

                    target = manager_ref.GetPositionFromID(manager_ref.GetIDFromActive());

                    manager_ref.ShowAttackArea(target, action_num);

                    state = State.aim_attack;
                    return;


                case 2:

                    return;

                case 3:
                    manager_ref.LoseDungeon();
                    return;
            }

            return;
        }

        if (inputer.GetBack())
        {
            if (restomon_action_ref.ReturnMenu())
                state = State.idle;

            return;
        }
    }

    private void View()
    {
        if (inputer.GetDir() != Direction.None)
        {
            if (manager_ref.PositionValid(target + DirectionMath.GetVectorChange(inputer.GetDir())))
                target += DirectionMath.GetVectorChange(inputer.GetDir());

            manager_ref.ShowView(target);

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

    private void AimAttack()
    {
        if (inputer.GetDir() != Direction.None)
        {
            if (manager_ref.PositionValid(target + DirectionMath.GetVectorChange(inputer.GetDir())))
                target += DirectionMath.GetVectorChange(inputer.GetDir());

            manager_ref.ShowAttackArea(target, action_num);

            return;
        }

        if (inputer.GetEnter())
        {
            if (manager_ref.AttackTargetValid(target, action_num))
            {
                manager_ref.RemoveMarker();

                manager_ref.ShowAttackTarget(target, action_num);

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
            manager_ref.Attack(target, action_num);

            manager_ref.RemoveMarker();

            state = State.idle;
        }

        if (inputer.GetBack())
        {
            manager_ref.ShowAttackArea(target, action_num);

            state = State.aim_attack;
            return;
        }
    }

    private void ViewSummon()
    {
        if (inputer.GetDir() != Direction.None)
        {
            if (manager_ref.PositionValid(target + DirectionMath.GetVectorChange(inputer.GetDir())))
                target += DirectionMath.GetVectorChange(inputer.GetDir());

            manager_ref.ShownSummonTarget(target, action_num);

            return;
        }

        if (inputer.GetEnter())
        {
            if (!manager_ref.SummonValid(target, action_num))
                return;

            manager_ref.SummonRestomon(target, action_num);

            target = new Vector3Int();

            manager_ref.RemoveMarker();

            state = State.idle;
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

    private void ViewEvolution()
    {
        if (inputer.GetDir() != Direction.None)
        {
            Vector3Int temp_target = target;

            if (manager_ref.PositionValid(target + DirectionMath.GetVectorChange(inputer.GetDir())))
                target += DirectionMath.GetVectorChange(inputer.GetDir());

            manager_ref.ShowEvolutionTarget(temp_target, target, action_num);

            return;
        }

        if (inputer.GetActionOne())
        {
            action_num = (action_num + 1) % 3;

            manager_ref.ShowEvolutionTarget(target, target, action_num);
        }

        if (inputer.GetEnter())
        {
            if (!manager_ref.EvolutionValid(target, action_num))
                return;

            manager_ref.EvolveRestomon(target, action_num);

            target = new Vector3Int();

            manager_ref.RemoveMarker();

            state = State.idle;
            return;
        }

        if (inputer.GetBack())
        {
            manager_ref.ShowEvolutionTarget(target, target, -1);

            target = new Vector3Int();

            manager_ref.RemoveMarker();

            state = State.idle;
            return;
        }
    }
}