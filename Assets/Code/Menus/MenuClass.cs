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

    [SerializeField] private GameObject[] attack_data, trait_data, gear_data, free_data, free_marked;

    [SerializeField] private Text free_traits;
    [SerializeField] private Text[] attack_text, stat_text, trait_text, gear_text, free_text;
    [SerializeField] private Text[] text_boxes;

    private int class_chosen;

    private int[] gear_chosen, traits_chosen;

    private bool[] traits_unlocked;

    private string[,] sub_string, weapon_string, trinket_string, trait_string;

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

        for (int i = 0; i < 8; ++i)
            attack_data[i].SetActive(false);

        for (int i = 0; i < 4; ++i)
            trait_data[i].SetActive(false);

        for (int i = 0; i < 4; ++i)
            gear_data[i].SetActive(false);

        for (int i = 0; i < 15; ++i)
        {
            free_data[i].SetActive(false);
            free_marked[i].SetActive(false);
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
        x_value = 0;
        y_value = 0;

        stats_menu.SetActive(true);
        attack_data[0].SetActive(true);

        Human temp_human = data_holder.GetPlayer();

        text_boxes[0].text = "" + temp_human.GetAttack(x_value + (y_value * 2)).GetDescription();

        stat_text[0].text = "" + temp_human.GetHp() + "+" + temp_human.GetApr();

        for (int i = 0; i < 9; ++i)
            stat_text[i + 1].text = "" + temp_human.GetStat(i);

        for (int i = 0; i < 8; ++i)
            attack_text[i].text = temp_human.GetAttack(i).GetName();

        for (int i = 0; i < 4; ++i)
            trait_text[i].text = temp_human.GetTraits()[i].GetName();
    }

    private void OpenGear()
    {
        gear_menu.SetActive(true);
        gear_chosen = data_holder.GetPlayerGear(out sub_string, out weapon_string, out trinket_string);

        x_value = 0;
        y_value = 0;

        for (int i = 0; i < 4; ++i)
            gear_data[i].SetActive(false);

        gear_data[y_value].SetActive(true);

        gear_text[0].text = sub_string[gear_chosen[0], 0];
        gear_text[1].text = weapon_string[gear_chosen[1], 0];
        gear_text[2].text = trinket_string[gear_chosen[2], 0];
        gear_text[3].text = trinket_string[gear_chosen[3], 0];

        text_boxes[1].text = sub_string[gear_chosen[0], 1];
    }

    private void OpenTraits()
    {
        traits_menu.SetActive(true);
        trait_string = data_holder.GetPlayerTraits(out traits_unlocked, out traits_chosen);

        x_value = 0;
        y_value = 0;

        free_data[y_value + (x_size * 5)].SetActive(true);

        for (int i = 0; i < 5; ++i)
        {
            free_text[i].text = trait_string[i, 0];
            free_text[i + 5].text = trait_string[i + 10, 0];
            free_text[i + 10].text = trait_string[i + 20, 0];
        }

        text_boxes[2].text = trait_string[y_value + (x_value * 10), 1];

        int temp_chosen = 0;

        for (int i = 0; i < traits_chosen.Length; ++i)
        {
            if (traits_chosen[i] != -1)
            {
                ++temp_chosen;

                if ((traits_chosen[i] % 10 < 5 && y_value < 5))
                    free_marked[(traits_chosen[i] / 10 * 5) + (traits_chosen[i] % 5)].SetActive(true);

                if ((traits_chosen[i] % 10 >= 5 && y_value >= 5))
                    free_marked[(traits_chosen[i] / 10 * 5) + (traits_chosen[i] % 5)].SetActive(true);
            }
        }

        free_traits.text = temp_chosen + "/" + traits_chosen.Length;
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

            data_holder.SetPlayerClass(class_chosen);

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
            CloseMenus();
            OpenInitial();

            current_state = State.Initial;
        }
    }

    private void StatsState(Inputer input)
    {
        if (input.GetDir() != Direction.None && input.GetMoveKeyUp())
        {
            for (int i = 0; i < 8; ++i)
                attack_data[i].SetActive(false);
            for (int i = 0; i < 4; ++i)
                trait_data[i].SetActive(false);

            GetInputValue(x_value, y_value, 3, 4, input.GetDir(), out x_value, out y_value);

            if (x_value < 2)
            {
                attack_data[x_value + (y_value * 2)].SetActive(true);
                text_boxes[0].text = "" + data_holder.GetPlayer().GetAttack(x_value + (y_value * 2)).GetDescription();
            }
            else
            {
                trait_data[y_value].SetActive(true);
                text_boxes[0].text = "" + data_holder.GetPlayer().GetTraits()[y_value].GetDescription();
            }
        }
        else if (input.GetEnter())
        {

        }
        else if (input.GetBack())
        {
            CloseMenus();
            x_value = 0;
            OpenCore();

            current_state = State.Core;
        }
    }

    private void GearState(Inputer input)
    {

        if (input.GetDir() != Direction.None && input.GetMoveKeyUp())
        {
            if (input.GetDir() == Direction.Up || input.GetDir() == Direction.Down)
                GetInputValue(x_value, y_value, 1, 4, input.GetDir(), out x_value, out y_value);
            else
            {
                if (y_value < 2)
                    GetInputValue(gear_chosen[y_value], 0, 5, 1, input.GetDir(), out gear_chosen[y_value], out int temp_value);
                else
                    GetInputValue(gear_chosen[y_value], 0, 6, 1, input.GetDir(), out gear_chosen[y_value], out int temp_value);
            }

            for (int i = 0; i < 4; ++i)
                gear_data[i].SetActive(false);

            gear_data[y_value].SetActive(true);

            gear_text[0].text = sub_string[gear_chosen[0], 0];
            if (y_value == 0)
                text_boxes[1].text = sub_string[gear_chosen[0], 1];

            gear_text[1].text = weapon_string[gear_chosen[1], 0];
            if (y_value == 1)
                text_boxes[1].text = weapon_string[gear_chosen[1], 1];

            gear_text[2].text = trinket_string[gear_chosen[2], 0];
            if (y_value == 2)
                text_boxes[1].text = trinket_string[gear_chosen[2], 1];

            gear_text[3].text = trinket_string[gear_chosen[3], 0];
            if (y_value == 3)
                text_boxes[1].text = trinket_string[gear_chosen[3], 1];
        }
        else if (input.GetEnter())
        {

        }
        else if (input.GetBack())
        {
            data_holder.SetPlayerGear(gear_chosen[0], gear_chosen[1], gear_chosen[2], gear_chosen[3]);

            CloseMenus();
            x_value = 1;
            OpenCore();

            current_state = State.Core;
        }
    }

    private void TraitsState(Inputer input)
    {

        if (input.GetDir() != Direction.None && input.GetMoveKeyUp())
        {
            GetInputValue(x_value, y_value, 3, 10, input.GetDir(), out x_value, out y_value);

            for (int i = 0; i < 15; ++i)
                free_data[i].SetActive(false);

            free_data[y_value % 5 + (x_value * 5)].SetActive(true);

            if (y_value < 5)
                for (int i = 0; i < 5; ++i)
                {
                    free_text[i + 0].text = trait_string[i + 0, 0];
                    free_text[i + 5].text = trait_string[i + 10, 0];
                    free_text[i + 10].text = trait_string[i + 20, 0];
                }
            else
                for (int i = 0; i < 5; ++i)
                {
                    free_text[i + 0].text = trait_string[i + 5, 0];
                    free_text[i + 5].text = trait_string[i + 15, 0];
                    free_text[i + 10].text = trait_string[i + 25, 0];
                }

            text_boxes[2].text = trait_string[y_value + (x_value * 10), 1];
        }
        else if (input.GetEnter())
        {
            if (traits_unlocked[y_value + (x_value * 10)])
            {
                int i;

                for (i = 0; i < traits_chosen.Length; ++i)
                    if (traits_chosen[i] == y_value + (x_value * 10))
                    {
                        traits_chosen[i] = -1;
                        break;
                    }

                if (i == traits_chosen.Length)
                    for (i = 0; i < traits_chosen.Length; ++i)
                        if (traits_chosen[i] == -1)
                        {
                            traits_chosen[i] = y_value + (x_value * 10);
                            break;
                        }
            }
        }
        else if (input.GetBack())
        {
            CloseMenus();
            x_value = 2;
            OpenCore();

            data_holder.SetPlayerTraits(traits_chosen);

            current_state = State.Core;
            return;
        }
        else
        {
            return;
        }

        for (int i = 0; i < 15; ++i)
            free_marked[i].SetActive(false);

        int temp_chosen = 0;

        for (int i = 0; i < traits_chosen.Length; ++i)
        {
            if (traits_chosen[i] != -1)
            {
                ++temp_chosen;

                if ((traits_chosen[i] % 10 < 5 && y_value < 5))
                    free_marked[(traits_chosen[i] / 10 * 5) + (traits_chosen[i] % 5)].SetActive(true);

                if ((traits_chosen[i] % 10 >= 5 && y_value >= 5))
                    free_marked[(traits_chosen[i] / 10 * 5) + (traits_chosen[i] % 5)].SetActive(true);
            }
        }

        free_traits.text = temp_chosen + "/" + traits_chosen.Length;
    }
}