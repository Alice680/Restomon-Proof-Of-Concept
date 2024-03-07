using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MenuShop : MenuSwapIcon
{
    private enum State { Core, Buy, Sell };


    [SerializeField] private GameObject[] core_options;
    [SerializeField] private Text[] item_text;
    [SerializeField] private Text money_text, description_text;

    private PermDataHolder data_holder;

    private State current_state;

    private Vector2Int[] buy_values;

    private bool buying_selling;

    private int y_overflow, y_max_overflow;

    private string[] current_names, current_descriptions;
    private int[] current_index, current_values;

    public void Startup(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;
    }

    public void ActivateEX(Vector2Int[] values)
    {
        base.Activate();

        buy_values = values;

        buying_selling = true;
        core_options[0].SetActive(buying_selling);
        core_options[1].SetActive(!buying_selling);

        y_value = 0;

        current_state = State.Core;
        ReloadString();
    }

    public bool Change(Inputer inputer)
    {
        if (inputer.GetBack() && current_state == State.Core)
        {
            DeActivate();
            return true;
        }

        switch (current_state)
        {
            case State.Core:
                CoreState(inputer);
                return false;
            case State.Buy:
                BuyingState(inputer);
                return false;
            case State.Sell:
                SellingState(inputer);
                return false;
        }

        return false;
    }

    private void CoreState(Inputer inputer)
    {
        if ((inputer.GetDir() == Direction.Left || inputer.GetDir() == Direction.Right) && inputer.GetMoveKeyUp())
        {
            buying_selling = !buying_selling;
            core_options[0].SetActive(buying_selling);
            core_options[1].SetActive(!buying_selling);
            return;
        }

        if (inputer.GetEnter())
        {
            if (buying_selling)
                current_state = State.Buy;
            else
                current_state = State.Sell;

            icons[y_value].SetActive(true);
            ReloadString();
        }
    }

    private void BuyingState(Inputer inputer)
    {
        if (inputer.GetDir() != 0)
        {
            base.UpdateMenu(inputer.GetDir());
            ReloadText();
            return;
        }

        if (inputer.GetEnter())
        {
            if ((y_overflow * 8) + y_value < buy_values.Length && data_holder.GetMoney() >= buy_values[(y_overflow * 8) + y_value].y)
            {
                data_holder.AddItem(buy_values[(y_overflow * 8) + y_value].x, 1);

                data_holder.ChangeMoney(-buy_values[(y_overflow * 8) + y_value].y);
                ReloadText();
            }

            return;
        }

        if (inputer.GetBack())
        {
            y_value = 0;

            for (int i = 0; i < 8; ++i)
                icons[i].SetActive(false);

            current_state = State.Core;
            ReloadString();
            return;
        }
    }

    private void SellingState(Inputer inputer)
    {
        if (inputer.GetDir() != 0)
        {
            base.UpdateMenu(inputer.GetDir());
            ReloadText();
            return;
        }

        if (inputer.GetEnter())
        {
            if ((y_overflow * 8) + y_value < current_index.Length)
            {
                data_holder.RemoveItem(current_index[(y_overflow * 8) + y_value], 1);

                data_holder.ChangeMoney(current_values[(y_overflow * 8) + y_value]);

                ReloadString();
            }

            return;
        }

        if (inputer.GetBack())
        {
            y_value = 0;

            for (int i = 0; i < 8; ++i)
                icons[i].SetActive(false);

            current_state = State.Core;
            ReloadString();
            return;
        }
    }

    private void ReloadText()
    {
        money_text.text = "Money : " + data_holder.GetMoney();

        for (int i = 0; i < 8; ++i)
        {
            if ((y_overflow * 8) + i >= current_names.Length)
                item_text[i].text = "";
            else
                item_text[i].text = current_names[(y_overflow * 8) + i];
        }

        if ((y_overflow * 8) + y_value >= current_descriptions.Length)
            description_text.text = "";
        else
            description_text.text = current_descriptions[(y_overflow * 8) + y_value];
    }

    private void ReloadString()
    {
        if (current_state == State.Core)
        {
            y_overflow = 0;
            y_max_overflow = 0;

            current_names = new string[0];
            current_descriptions = new string[0];
        }
        else if (current_state == State.Buy)
        {
            y_overflow = 0;
            y_max_overflow = (buy_values.Length / 8) + 1;

            current_names = new string[buy_values.Length];
            current_descriptions = new string[buy_values.Length];

            for (int i = 0; i < buy_values.Length; ++i)
            {
                current_names[i] = ItemHolder.GetItem(buy_values[i].x).GetInfo(out current_descriptions[i]);
                current_names[i] += "   " + buy_values[i].y;
            }
        }
        else if (current_state == State.Sell)
        {
            string[] temp_names = data_holder.GetsellableItems(out string[] temp_descriptions, out int[] temp_index, out int[] temp_values);

            y_overflow = 0;
            y_max_overflow = (temp_names.Length / 8) + 1;

            current_names = temp_names;
            current_descriptions = temp_descriptions;
            current_index = temp_index;
            current_values = temp_values;
        }

        ReloadText();
    }
}