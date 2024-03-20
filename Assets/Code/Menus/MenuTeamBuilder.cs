using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MenuTeamBuilder : MenuSwapIcon
{
    private enum State { Initial, Catalyst, SelectSlot, SelectRestomon, ChoseRestomon, EditRestomon };

    [SerializeField] private MenuSwapIcon initial_menu, selector_menu, restomon_menu;

    [SerializeField] private MenuRestomonEditor edit_menu;

    [SerializeField] private Text[] selector_texts, restomon_texts;
    [SerializeField] private Text selector_box;
    [SerializeField] private GameObject[] selector_stars;

    private int int_variable_a, int_variable_b;

    private PermDataHolder data_holder;

    private State current_state;

    public void SetData(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;

        edit_menu.SetData(data_holder);

        current_state = State.Initial;
    }

    public override void Activate()
    {
        CloseMenus();
        OpenInitial();
        current_state = State.Initial;
    }

    public override void DeActivate()
    {

    }

    private void CloseMenus()
    {
        initial_menu.DeActivate();
        selector_menu.DeActivate();
        selector_menu.Reset();
        restomon_menu.DeActivate();

        for (int i = 0; i < 8; ++i)
        {
            selector_texts[i].text = "";
            selector_stars[i].SetActive(false);
        }

        for (int i = 0; i < 36; ++i)
            restomon_texts[i].text = "";

        selector_box.text = "";
    }

    private void OpenInitial()
    {
        initial_menu.Activate();
    }

    private void OpenCatalyst()
    {
        selector_menu.Activate();

        for (int i = 0; i < 8; ++i)
        {
            if (data_holder.CatalystUnloked(i))
                selector_texts[i].text = data_holder.GetCatalyst(i).GetName();
            else
                selector_texts[i].text = "Locked";
        }

        selector_menu.GetValues(out int x, out int y);

        if (data_holder.CatalystUnloked(y + (x * 4)))
            selector_box.text = data_holder.GetCatalyst(y + (x * 4)).GetDescription();
        else
            selector_box.text = "Catalyst Locked";

        selector_stars[data_holder.GetCatalystInt()].SetActive(true);
    }

    private void OpenSelectSlot()
    {
        selector_menu.Activate();

        for (int i = 0; i < 8; ++i)
        {
            if (i < data_holder.GetCatalyst(int_variable_a).GetTeamSize())
            {
                if (data_holder.GetRestomonInt(i) != -1)
                    selector_texts[i].text = data_holder.GetRestomonData(data_holder.GetRestomonInt(i)).GetName();
                else
                    selector_texts[i].text = "Empty";
            }
            else
            {
                selector_texts[i].text = "Void";
            }
        }

        selector_menu.GetValues(out int x, out int y);
        if (y + (x * 4) < data_holder.GetCatalyst(int_variable_a).GetTeamSize() && data_holder.GetTeam(y + (x * 4)) != null)
            selector_box.text = data_holder.GetTeam(y + (x * 4)).GetDescription();
        else
            selector_box.text = "";
    }

    private void OpenSelectRestomon()
    {
        restomon_menu.Activate();

        for (int i = 0; i < 36; ++i)
        {
            if (data_holder.GetRestomonUnlocked(i))
                restomon_texts[i].text = data_holder.GetRestomonData(i).GetName();
            else
                restomon_texts[i].text = "Locked";
        }
    }

    private void OpenChoseRestomon()
    {
        restomon_menu.Activate();

        for (int i = 0; i < 36; ++i)
        {
            if (data_holder.GetRestomonUnlocked(i))
                restomon_texts[i].text = data_holder.GetRestomonData(i).GetName();
            else
                restomon_texts[i].text = "Locked";
        }
    }

    private void OpenEditRestomon()
    {
        edit_menu.Activate(int_variable_a);
    }

    public bool Run(Inputer input)
    {
        if (input.GetBack() && current_state == State.Initial)
        {
            CloseMenus();
            return true;
        }

        switch (current_state)
        {
            case State.Initial:
                Initial(input);
                break;
            case State.Catalyst:
                Catalyst(input);
                break;
            case State.SelectSlot:
                SelectSlot(input);
                break;
            case State.SelectRestomon:
                SelectRestomon(input);
                break;
            case State.ChoseRestomon:
                ChoseRestomon(input);
                break;
            case State.EditRestomon:
                EditRestomon(input);
                break;
        }

        return false;
    }

    private void Initial(Inputer input)
    {
        if (input.GetDir() != Direction.None)
        {
            initial_menu.UpdateMenu(input.GetDir());
        }
        else if (input.GetEnter())
        {
            initial_menu.GetValues(out int x, out int y);

            if (x == 0)
            {
                CloseMenus();
                OpenCatalyst();
                current_state = State.Catalyst;
            }
            else
            {
                CloseMenus();
                OpenChoseRestomon();
                current_state = State.ChoseRestomon;
            }
        }
        else if (input.GetBack())
        {

        }
    }

    private void Catalyst(Inputer input)
    {
        if (input.GetDir() != Direction.None)
        {
            selector_menu.UpdateMenu(input.GetDir());

            selector_menu.GetValues(out int x, out int y);

            if (data_holder.CatalystUnloked(y + (x * 4)))
                selector_box.text = data_holder.GetCatalyst(y + (x * 4)).GetDescription();
            else
                selector_box.text = "Catalyst Locked";
        }
        else if (input.GetEnter())
        {
            selector_menu.GetValues(out int x, out int y);

            if (data_holder.CatalystUnloked(y + (x * 4)))
            {
                int_variable_a = y + (x * 4);

                data_holder.SetCatalyst(int_variable_a);

                CloseMenus();
                OpenSelectSlot();
                current_state = State.SelectSlot;
            }
        }
        else if (input.GetBack())
        {
            CloseMenus();
            OpenInitial();
            current_state = State.Initial;
        }
    }

    private void SelectSlot(Inputer input)
    {
        if (input.GetDir() != Direction.None)
        {
            selector_menu.UpdateMenu(input.GetDir());

            selector_menu.GetValues(out int x, out int y);
            if (y + (x * 4) < data_holder.GetCatalyst(int_variable_a).GetTeamSize() && data_holder.GetTeam(y + (x * 4)) != null)
                selector_box.text = data_holder.GetTeam(y + (x * 4)).GetDescription();
            else
                selector_box.text = "";
        }
        else if (input.GetEnter())
        {
            selector_menu.GetValues(out int x, out int y);
            if (y + (x * 4) < data_holder.GetCatalyst(int_variable_a).GetTeamSize())
            {
                int_variable_b = y + (x * 4);

                CloseMenus();
                OpenSelectRestomon();
                current_state = State.SelectRestomon;
            }
        }
        else if (input.GetBack())
        {
            CloseMenus();
            OpenCatalyst();
            current_state = State.Catalyst;
        }
    }

    private void SelectRestomon(Inputer input)
    {
        if (input.GetDir() != Direction.None)
        {
            restomon_menu.UpdateMenu(input.GetDir());
        }
        else if (input.GetEnter())
        {
            restomon_menu.GetValues(out int x, out int y);
            data_holder.SetRestomon(int_variable_b, x + (y * 6));

            CloseMenus();
            OpenSelectSlot();
            current_state = State.SelectSlot;
        }
        else if (input.GetBack())
        {
            data_holder.SetRestomon(int_variable_b, -1);

            CloseMenus();
            OpenSelectSlot();
            current_state = State.SelectSlot;
        }
    }

    private void ChoseRestomon(Inputer input)
    {
        if (input.GetDir() != Direction.None)
        {
            restomon_menu.UpdateMenu(input.GetDir());
        }
        else if (input.GetEnter())
        {
            restomon_menu.GetValues(out int x, out int y);

            if (data_holder.GetRestomonUnlocked(x + (y * 6)))
            {
                int_variable_a = x + (y * 6);

                CloseMenus();
                OpenEditRestomon();
                current_state = State.EditRestomon;
            }
        }
        else if (input.GetBack())
        {
            CloseMenus();
            OpenInitial();
            current_state = State.Initial;
        }
    }

    private void EditRestomon(Inputer input)
    {
        if (edit_menu.Run(input))
        {
            edit_menu.DeActivate();
            CloseMenus();
            OpenInitial();
            current_state = State.Initial;
        }
    }
}