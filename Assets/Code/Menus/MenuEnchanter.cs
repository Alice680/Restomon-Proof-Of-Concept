using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[System.Serializable]
public class MenuEnchanter : MenuSwapIcon
{
    private enum State { Core, Moves, Traits, Evolve };

    [SerializeField] private GameObject[] restomon_marker, choice_marker;
    [SerializeField] private GameObject cost_marker;

    [SerializeField] private Text[] restmon_text, choice_text;
    [SerializeField] private Text cost_text, page_text, dialogue_text;

    private PermDataHolder data_holder;

    private State current_state;

    private int restomon_chosen;

    private bool in_restomon, can_get;

    private string[] restomon_names;

    public void Startup(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;
    }

    public override void Activate()
    {
        menu.SetActive(true);

        CloseEverything();

        current_state = State.Core;

        icons[0].SetActive(true);
    }

    public bool Change(Inputer inputer)
    {
        if (current_state == State.Core)
        {
            if (CoreState(inputer))
            {
                CloseEverything();
                DeActivate();
                return true;
            }
            return false;
        }
        else if (!in_restomon)
        {
            ChoiceState(inputer);
            return false;
        }
        else
        {
            RestomonState(inputer);
            return false;
        }
    }

    private void CloseEverything()
    {
        x_value = 0;
        y_value = 0;

        restomon_chosen = -1;

        for (int i = 0; i < 3; ++i)
            icons[i].SetActive(false);

        for (int i = 0; i < 18; ++i)
        {
            restomon_marker[i].SetActive(false);
            restmon_text[i].text = "";
        }

        for (int i = 0; i < 8; ++i)
        {
            choice_marker[i].SetActive(false);
            choice_text[i].text = "";
        }

        cost_marker.SetActive(false);
        cost_text.text = "";
        page_text.text = "";
        dialogue_text.text = "";
    }
   
    private void OpenChoice()
    {
        x_value = 0;
        y_value = 0;

        choice_text[0].text = "yes";
        choice_text[1].text = "no";

        choice_marker[0].SetActive(true);

        cost_marker.SetActive(true);
    }

    private void OpenRestomon()
    {
        x_value = 0;
        y_value = 0;

        restomon_names = new string[36];
        for (int i = 0; i < 36; ++i)
        {
            if (data_holder.GetRestomonUnlocked(i))
                restomon_names[i] = data_holder.GetRestomonData(i).GetName();
            else
                restomon_names[i] = "Locked";
        }

        for (int i = 0; i < 18; ++i)
            restmon_text[i].text = restomon_names[i];

        restomon_marker[0].SetActive(true);
        page_text.text = "1/2";
    }

    private bool CoreState(Inputer inputer)
    {
        if (inputer.GetDir() != 0 && inputer.GetMoveKeyUp())
        {
            GetInputValue(x_value, y_value, 3, 1, inputer.GetDir(), out x_value, out y_value);

            for (int i = 0; i < 3; ++i)
                icons[i].SetActive(false);
            icons[x_value].SetActive(true);

            return false;
        }

        if (inputer.GetEnter())
        {
            switch (x_value)
            {
                case 0:
                    in_restomon = true;
                    current_state = State.Moves;
                    OpenRestomon();
                    break;
                case 1:
                    in_restomon = true;
                    current_state = State.Traits;
                    OpenRestomon();
                    break;
                case 2:
                    in_restomon = true;
                    current_state = State.Evolve;
                    OpenRestomon();
                    break;
            }

            return false;
        }

        if (inputer.GetBack())
        {

            return true;
        }
        return false;
    }

    private void ChoiceState(Inputer inputer)
    {
        if (inputer.GetDir() != 0 && inputer.GetMoveKeyUp())
        {
            GetInputValue(x_value, y_value, 2, 1, inputer.GetDir(), out x_value, out y_value);

            for (int i = 0; i < 2; ++i)
                choice_marker[i].SetActive(false);

            choice_marker[x_value].SetActive(true);
        }

        if (inputer.GetEnter())
        {
            if (x_value == 0)
            {
                if (can_get)
                {
                    switch (current_state)
                    {
                        case State.Moves:
                            data_holder.PurchasRankUp();
                            cost_text.text = data_holder.GetRankUp(out can_get);
                            break;
                        case State.Traits:
                            data_holder.PurchasRefineUp(restomon_chosen);
                            cost_text.text = data_holder.GetRefineUp(restomon_chosen, out can_get);
                            break;
                        case State.Evolve:
                            data_holder.PurchasReforgeUp(restomon_chosen);
                            cost_text.text = data_holder.GetReforgeUp(restomon_chosen, out can_get);
                            break;
                    }
                }
            }
            else
            {
                CloseEverything();

                switch (current_state)
                {
                    case State.Moves:
                        x_value = 0;
                        break;
                    case State.Traits:
                        x_value = 1;
                        break;
                    case State.Evolve:
                        x_value = 2;
                        break;
                }

                in_restomon = false;
                icons[x_value].SetActive(true);
                current_state = State.Core;
            }
        }

        if (inputer.GetBack())
        {
            CloseEverything();

            switch (current_state)
            {
                case State.Moves:
                    x_value = 0;
                    break;
                case State.Traits:
                    x_value = 1;
                    break;
                case State.Evolve:
                    x_value = 2;
                    break;
            }

            in_restomon = false;
            icons[x_value].SetActive(true);
            current_state = State.Core;
        }
    }

    private void RestomonState(Inputer inputer)
    {
        if (inputer.GetDir() != 0 && inputer.GetMoveKeyUp())
        {
            GetInputValue(x_value, y_value, 6, 6, inputer.GetDir(), out x_value, out y_value);

            for (int i = 0; i < 18; ++i)
            {
                restomon_marker[i].SetActive(false);
                restmon_text[i].text = restomon_names[i + ((x_value / 3) * 18)];
            }

            restomon_marker[(y_value * 3) + (x_value % 3)].SetActive(true);
            page_text.text = (x_value / 3 + 1) + "/2";
        }

        if (inputer.GetEnter())
        {
            if (restomon_names[(x_value / 3 * 18) + (y_value * 3) + (x_value % 3)] != "Locked")
            {
                CloseEverything();

                restomon_chosen = (x_value / 3 * 18) + (y_value * 3) + (x_value % 3);

                switch (current_state)
                {
                    case State.Moves:
                        cost_text.text = data_holder.GetRefineUp(restomon_chosen, out can_get);
                        x_value = 0;
                        break;
                    case State.Traits:
                        cost_text.text = data_holder.GetRefineUp(restomon_chosen, out can_get);
                        x_value = 1;
                        break;
                    case State.Evolve:
                        cost_text.text = data_holder.GetReforgeUp(restomon_chosen, out can_get);
                        x_value = 2;
                        break;
                }

                in_restomon = false;
                icons[x_value].SetActive(true);
                OpenChoice();
            }
        }

        if (inputer.GetBack())
        {
            CloseEverything();

            switch (current_state)
            {
                case State.Moves:
                    x_value = 0;
                    break;
                case State.Traits:
                    x_value = 1;
                    break;
                case State.Evolve:
                    x_value = 2;
                    break;
            }

            in_restomon = false;
            icons[x_value].SetActive(true);
            current_state = State.Core;
        }
    }
}