using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MenuSmith : MenuSwapIcon
{
    private enum State { Core, Weapons, Accessories };

    [SerializeField] private GameObject[] core_options;
    [SerializeField] private Text[] item_text;
    [SerializeField] private Text material_text, description_text, page_text;

    private PermDataHolder data_holder;

    private State current_state;

    private bool weapons_accessories;

    private int y_overflow, y_max_overflow;

    private int[] weapon_values, accessory_values;

    private string[] current_names, current_descriptions, current_costs;

    public void Startup(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;
    }

    public void ActivateEX(int[] weapon_values, int[] accessory_values)
    {
        base.Activate();

        weapons_accessories = true;
        core_options[0].SetActive(weapons_accessories);
        core_options[1].SetActive(!weapons_accessories);

        y_value = 0;

        this.weapon_values = weapon_values;
        this.accessory_values = accessory_values;

        for (int i = 0; i < 8; ++i)
            icons[i].SetActive(false);

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
            case State.Weapons:
                WeaponState(inputer);
                return false;
            case State.Accessories:
                AccessorieState(inputer);
                return false;
        }

        return false;
    }

    private void CoreState(Inputer inputer)
    {
        if ((inputer.GetDir() == Direction.Left || inputer.GetDir() == Direction.Right) && inputer.GetMoveKeyUp())
        {
            weapons_accessories = !weapons_accessories;
            core_options[0].SetActive(weapons_accessories);
            core_options[1].SetActive(!weapons_accessories);
            return;
        }

        if (inputer.GetEnter())
        {
            if (weapons_accessories)
                current_state = State.Weapons;
            else
                current_state = State.Accessories;

            icons[y_value].SetActive(true);
            ReloadString();
        }
    }

    private void WeaponState(Inputer inputer)
    {
        if (inputer.GetDir() != 0)
        {
            base.UpdateMenu(inputer.GetDir());
            ReloadText();
            return;
        }

        if (inputer.GetEnter())
        {
            if ((y_overflow * 8) + y_value < current_names.Length)
            {
                data_holder.PurchaseGear(true, weapon_values[(y_overflow * 8) + y_value]);
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

    private void AccessorieState(Inputer inputer)
    {

        if (inputer.GetDir() != 0)
        {
            base.UpdateMenu(inputer.GetDir());
            ReloadText();
            return;
        }

        if (inputer.GetEnter())
        {
            if ((y_overflow * 8) + y_value < current_names.Length)
            {
                data_holder.PurchaseGear(false, accessory_values[(y_overflow * 8) + y_value]);
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

        if ((y_overflow * 8) + y_value >= current_costs.Length)
            material_text.text = "";
        else
            material_text.text = current_costs[(y_overflow * 8) + y_value];

        if (current_state == State.Core)
            page_text.text = "";
        else
            page_text.text = (y_overflow + 1) + "/" + (current_descriptions.Length / 8 + 1);
    }

    private void ReloadString()
    {
        if (current_state == State.Core)
        {
            y_overflow = 0;
            y_max_overflow = 0;

            current_names = new string[0];
            current_descriptions = new string[0];
            current_costs = new string[0];
        }
        else if (current_state == State.Weapons)
            data_holder.GetGearData(true, weapon_values, out current_names, out current_descriptions, out current_costs);
        else if (current_state == State.Accessories)
            data_holder.GetGearData(false, accessory_values, out current_names, out current_descriptions, out current_costs);

        ReloadText();
    }
}