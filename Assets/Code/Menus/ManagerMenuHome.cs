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
    [SerializeField] private MenuSwapIcon storage_menu;

    public void SetData(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;

        class_menu.SetData(data_holder);

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
                    //current_state = State.resting;
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
                    //current_state = State.storing;
                    return false;
                case 4:
                    core_menu.DeActivate();
                    return true;
            }
        }

        return false;
    }

    private void RestState(Inputer inputer)
    {

    }

    private void ClassState(Inputer inputer)
    {
        if(class_menu.Run(inputer))
        {
            core_menu.Activate();
            current_state = State.core;
        }
    }

    private void TeamState(Inputer inputer)
    {

    }

    private void StorageState(Inputer inputer)
    {

    }
}