using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MenuRestomonEditor : MenuSwapIcon
{
    private enum State { None, Initial, Core, Sub, Choice }

    [SerializeField] private Text name_text;
    [SerializeField] private Text choice_description;
    [SerializeField] private Text[] core_text, sub_text, choice_text;
    [SerializeField] private GameObject core_menu, sub_menu, choice_menu;
    [SerializeField] private GameObject[] core_icons, sub_icons, choice_icons;

    private int restomon_index;
    private RestomonBase restomon_base;

    private int menu_a_index;
    private bool menu_a_tracker;

    private int choice_x, choice_y;

    private string[] stat_icons = new string[12] { "Str", "Mag", "Frc", "Def", "Shd", "Wil", "Spd", "Mov", "Act", "Cst", "Drn", "Abs" };

    private PermDataHolder data_holder;

    private int choice_value;
    private string[] choice_descriptions_values;
    private State previous_state;
    private State current_state;

    //Restomon Current Data
    private Vector2Int[] current_attacks;

    //Restomon Unlock Data
    private bool[] basic_attacks;
    private List<bool[]> attacks;
    private List<bool[]> traits;

    public void SetData(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;
        current_state = State.Initial;
    }

    public void Activate(int value)
    {
        y_value = 0;
        base.Activate();

        choice_descriptions_values = new string[24];

        restomon_index = value;
        restomon_base = data_holder.GetRestomonData(restomon_index);

        name_text.text = restomon_base.GetName();

        data_holder.GetRestomonUnlockInfo(restomon_index, out int rank, out int reforges, out int refinements, out bool[] mutations, out basic_attacks, out attacks, out traits);
        data_holder.GetRestomonInfo(restomon_index, out current_attacks);

        CloseMenus();
        OpenCore();

        current_state = State.Initial;
    }

    public override void DeActivate()
    {
        CloseMenus();

        base.DeActivate();
        name_text.text = "";
    }

    private void CloseMenus()
    {
        core_menu.SetActive(false);
        sub_menu.SetActive(false);

        for (int i = 0; i < 17; ++i)
        {
            core_text[i].text = "";
            core_icons[i].SetActive(false);
        }

        data_holder.SetRestomonInfo(restomon_index, current_attacks);

        CloseChoice();
    }

    private void OpenCore()
    {
        core_menu.SetActive(true);

        for (int i = 0; i < 12; ++i)
        {
            core_text[i].text = stat_icons[i] + " (" + "0" + ") " + "0";
        }

        core_text[12].text = restomon_base.GetBasicAttacks()[current_attacks[0].x].GetName();
        core_text[13].text = restomon_base.GetBasicAttacks()[current_attacks[0].y].GetName();
        core_text[14].text = "2";
        core_text[15].text = "3";
        core_text[16].text = "Locked";
    }

    private void OpenSub()
    {

    }

    private void CloseChoice()
    {
        choice_menu.SetActive(false);

        choice_description.text = "";

        for (int i = 0; i < 24; ++i)
        {
            choice_icons[i].SetActive(false);
            choice_text[i].text = "";
        }
    }

    private void OpenChoice()
    {
        choice_menu.SetActive(true);

        choice_x = 0;
        choice_y = 0;

        for (int i = 0; i < 24; ++i)
            choice_icons[i].SetActive(false);

        choice_icons[choice_x + (choice_y * 6)].SetActive(true);
    }

    public bool Run(Inputer input)
    {
        if (input.GetBack() && current_state == State.Initial)
        {
            return true;
        }

        switch (current_state)
        {
            case State.Initial:
                Initial(input);
                break;
            case State.Core:
                Core(input);
                break;
            case State.Sub:
                Sub(input);
                break;
            case State.Choice:
                Choice(input);
                break;

        }

        return false;
    }

    private void Initial(Inputer input)
    {
        if (input.GetDir() != Direction.None)
        {
            base.UpdateMenu(input.GetDir());

            if (y_value == 0)
                OpenCore();
            else
                Debug.Log("Finish Menu");
        }
        else if (input.GetEnter())
        {
            if (y_value == 0)
            {
                menu_a_index = 0;
                core_icons[menu_a_index].SetActive(true);
                current_state = State.Core;
            }
            else
            {

            }
        }
        else if (input.GetBack())
        {

        }
    }

    private void Core(Inputer input)
    {
        if (input.GetDir() != Direction.None)
        {
            if (menu_a_tracker)
            {

            }
            else
            {
                for (int i = 0; i < 17; ++i)
                    core_icons[i].SetActive(false);

                if ((input.GetDir() == Direction.Right || input.GetDir() == Direction.Left) && (menu_a_index == 0 || menu_a_index == 6))
                {
                    menu_a_index = menu_a_index == 0 ? 6 : 0;
                }
                else if (input.GetDir() == Direction.Right || input.GetDir() == Direction.Left)
                {
                    if (menu_a_index / 6 == 2)
                        menu_a_index++;
                    else if ((menu_a_index / 6 == 0 && input.GetDir() == Direction.Left) || (menu_a_index / 6 == 1 && input.GetDir() == Direction.Right))
                        menu_a_index--;

                    GetInputValue(menu_a_index / 6, 0, 3, 1, input.GetDir(), out int temp_x, out int temp_y);
                    menu_a_index = (menu_a_index % 6) + temp_x * 6;
                }
                else if (menu_a_index < 12)
                {
                    GetInputValue(0, menu_a_index % 6, 1, 6, input.GetDir(), out int temp_x, out int temp_y);
                    menu_a_index = menu_a_index - (menu_a_index % 6) + temp_y;
                }
                else
                {
                    GetInputValue(0, menu_a_index % 6, 1, 5, input.GetDir(), out int temp_x, out int temp_y);
                    menu_a_index = menu_a_index - (menu_a_index % 6) + temp_y;
                }
            }

            core_icons[menu_a_index].SetActive(true);
        }
        else if (input.GetEnter())
        {
            if (menu_a_tracker)
            {
                menu_a_tracker = false;
            }
            else
            {
                if (menu_a_index < 12)
                {
                    menu_a_tracker = true;
                }
                else if (menu_a_index == 12 || menu_a_index == 13)
                {
                    OpenChoice();

                    if (menu_a_index == 12)
                        choice_value = 0;
                    else
                        choice_value = 1;

                    for(int i = 0; i<24; i++)
                    {
                        choice_text[i].text = "";
                        choice_descriptions_values[i] = "";
                    }

                    for (int i = 0; i < basic_attacks.Length; ++i)
                    {
                        if (basic_attacks[i])
                        {
                            choice_text[i].text = restomon_base.GetBasicAttacks()[i].GetName();
                            choice_descriptions_values[i] = restomon_base.GetBasicAttacks()[i].GetDescription();
                        }
                        else
                        {
                            choice_text[i].text = "Locked";
                            choice_descriptions_values[i] = "Unlock this move to view more info";
                        }
                    }

                    choice_description.text = choice_descriptions_values[0];

                    previous_state = current_state;
                    current_state = State.Choice;
                }
            }

        }
        else if (input.GetBack())
        {
            if (menu_a_tracker)
            {
                menu_a_tracker = false;
            }
            else
            {
                for (int i = 0; i < 17; ++i)
                    core_icons[i].SetActive(false);

                current_state = State.Initial;
            }
        }
    }

    private void Sub(Inputer input)
    {
        if (input.GetDir() != Direction.None)
        {

        }
        else if (input.GetEnter())
        {

        }
        else if (input.GetBack())
        {

        }
    }

    private void Choice(Inputer input)
    {
        if (input.GetDir() != Direction.None)
        {
            GetInputValue(choice_x, choice_y, 6, 4, input.GetDir(), out choice_x, out choice_y);

            for (int i = 0; i < 24; ++i)
                choice_icons[i].SetActive(false);

            choice_icons[choice_x + (choice_y * 6)].SetActive(true);
            choice_description.text = choice_descriptions_values[choice_x + (choice_y * 6)];
        }
        else if (input.GetEnter())
        {
            if(previous_state == State.Core && choice_value <= 1)
            {
                if (!basic_attacks[choice_x + (choice_y * 6)])
                    return;

                if (choice_value == 0)
                {
                    if (current_attacks[0].y == choice_x + (choice_y * 6))
                        current_attacks[0].y = current_attacks[0].x;

                    current_attacks[0].x = choice_x + (choice_y * 6);
                }
                else
                {
                    if (current_attacks[0].x == choice_x + (choice_y * 6))
                        current_attacks[0].x = current_attacks[0].y;

                    current_attacks[0].y = choice_x + (choice_y * 6);
                }
            }

            current_state = previous_state;
            previous_state = State.None;
            CloseChoice();
            OpenCore();
        }
        else if (input.GetBack())
        {
            current_state = previous_state;
            previous_state = State.None;
            CloseChoice();
        }
    }
}