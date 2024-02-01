using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MenuClass : MenuSwapIcon
{
    private enum State { Initial, Core, Stats, Gear, Traits };

    [SerializeField] private GameObject initial_menu, core_menu, stats_menu, gear_menu, traits_menu;
    [SerializeField] private GameObject[] initial_select, initial_star, core_select;

    [SerializeField] private GameObject[] attack_data, trait_data;
    [SerializeField] private Text[] attack_text, stat_text, trait_text;

    [SerializeField] private Text[] text_boxes;

    private int class_chosen;

    private PermDataHolder data_holder;

    private State current_state;

    public void SetData(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;
        current_state = State.Initial;
    }

    public override void Activate()
    {
        CloseMenus();
        OpenInitial();
    }

    public override void DeActivate()
    {

    }

    private void CloseMenus()
    {
        menu.SetActive(false);
        initial_menu.SetActive(false);
        core_menu.SetActive(false);
        stats_menu.SetActive(false);
        gear_menu.SetActive(false);
        traits_menu.SetActive(false);

        for (int i = 0; i < 3; ++i)
        {
            initial_select[i].SetActive(false);
            initial_star[i].SetActive(false);
            core_select[i].SetActive(false);
        }
    }

    private void OpenInitial()
    {
        initial_menu.SetActive(true);

        x_value = 0;
        y_value = data_holder.GetPlayerClass();

        initial_select[y_value].SetActive(true);
        initial_star[y_value].SetActive(true);
    }

    private void OpenCore()
    {
        y_value = 0;

        core_menu.SetActive(true);

        core_select[x_value].SetActive(true);
    }

    private void OpenStats()
    {

    }

    private void OpenGear()
    {

    }

    private void OpenTraits()
    {

    }

    public bool Run(Inputer input)
    {
        switch (current_state)
        {
            case State.Initial:
                InitialState(input, out bool leave);
                return leave;
            case State.Core:
                CoreState(input);
                return false;
            case State.Stats:
                StatsState(input);
                return false;
            case State.Gear:
                GearState(input);
                return false;
            case State.Traits:
                TraitsState(input);
                return false;
            default:
                return false;
        }
    }

    private void InitialState(Inputer input, out bool leave)
    {
        leave = false;

        if (input.GetDir() != Direction.None && input.GetMoveKeyUp())
        {
            for (int i = 0; i < 3; ++i)
                initial_select[i].SetActive(false);

            GetInputValue(x_value, y_value, 1, 3, input.GetDir(), out x_value, out y_value);

            initial_select[y_value].SetActive(true);
        }
        else if (input.GetEnter())
        {
            class_chosen = y_value;

            x_value = 0;

            CloseMenus();
            OpenCore();

            current_state = State.Core;
        }
        else if (input.GetBack())
        {
            CloseMenus();
            leave = true;
        }
    }

    private void CoreState(Inputer input)
    {

        if (input.GetDir() != Direction.None && input.GetMoveKeyUp())
        {
            for (int i = 0; i < 3; ++i)
                core_select[i].SetActive(false);

            GetInputValue(x_value, y_value, 3, 1, input.GetDir(), out x_value, out y_value);

            core_select[x_value].SetActive(true);
        }
        else if (input.GetEnter())
        {
            switch (x_value)
            {
                case 0:
                    OpenStats();
                    current_state = State.Stats;
                    break;
                case 1:
                    OpenGear();
                    current_state = State.Gear;
                    break;
                case 2:
                    OpenTraits();
                    current_state = State.Traits;
                    break;
            }
        }
        else if (input.GetBack())
        {
            UpdateData();

            CloseMenus();
            OpenInitial();

            current_state = State.Initial;
        }
    }

    private void StatsState(Inputer input)
    {

    }

    private void GearState(Inputer input)
    {

    }

    private void TraitsState(Inputer input)
    {

    }

    private void UpdateData()
    {

    }
}