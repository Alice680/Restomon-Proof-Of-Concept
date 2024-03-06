using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerMenuHome : MonoBehaviour
{
    private enum State { core, resting, classing, teaming, storing };

    private PermDataHolder data_holder;
    private State current_state;

    [SerializeField] private MenuSwapIcon core_menu;
    [SerializeField] private MenuSwapIcon rest_menu;
    [SerializeField] private MenuClass class_menu;
    [SerializeField] private MenuSwapIcon team_menu;
    [SerializeField] private MenuStorage storage_menu;

    public void SetData(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;

        class_menu.SetData(data_holder);
        storage_menu.SetData(data_holder);

        current_state = State.core;
    }

    public void Activate()
    {
        core_menu.Activate();
    }

    public bool Change(Inputer inputer)
    {
        switch (current_state)
        {
            case State.core:
                return CoreState(inputer);
            case State.resting:
                RestState(inputer);
                return false;
            case State.classing:
                ClassState(inputer);
                return false;
            case State.teaming:
                TeamState(inputer);
                return false;
            case State.storing:
                StorageState(inputer);
                return false;
        }

        return false;
    }

    private bool CoreState(Inputer inputer)
    {
        if (inputer.GetDir() != Direction.None && inputer.GetMoveKeyUp())
        {
            core_menu.UpdateMenu(inputer.GetDir());
        }
        else if (inputer.GetEnter())
        {
            core_menu.GetValues(out int x, out int y);

            switch (y)
            {
                case 0:
                    core_menu.DeActivate();
                    rest_menu.Activate();
                    current_state = State.resting;
                    return false;
                case 1:
                    core_menu.DeActivate();
                    class_menu.Activate();
                    current_state = State.classing;
                    return false;
                case 2:
                    //current_state = State.teaming;
                    return false;
                case 3:
                    core_menu.DeActivate();
                    storage_menu.Activate();
                    current_state = State.storing;
                    return false;
                case 4:
                    core_menu.DeActivate();
                    return true;
            }
        }
        else if(inputer.GetBack())
        {
            core_menu.DeActivate();
            return true;
        }

        return false;
    }

    private void RestState(Inputer inputer)
    {
        if (inputer.GetDir() != Direction.None && inputer.GetMoveKeyUp())
        {
            rest_menu.UpdateMenu(inputer.GetDir());
        }
        else if (inputer.GetEnter())
        {
            rest_menu.GetValues(out int x, out int y);

            if (x == 0)
                data_holder.Rest();

            core_menu.Activate();
            rest_menu.DeActivate();
            current_state = State.core;
        }
        else if(inputer.GetBack())
        {
            core_menu.Activate();
            rest_menu.DeActivate();
            current_state = State.core;
        }
    }

    private void ClassState(Inputer inputer)
    {
        if (class_menu.Run(inputer))
        {
            core_menu.Activate();
            class_menu.DeActivate();
            current_state = State.core;
        }
    }

    private void TeamState(Inputer inputer)
    {

    }

    private void StorageState(Inputer inputer)
    {
        if (storage_menu.Run(inputer))
        {
            core_menu.Activate();
            storage_menu.DeActivate();
            current_state = State.core;
        }
    }
}