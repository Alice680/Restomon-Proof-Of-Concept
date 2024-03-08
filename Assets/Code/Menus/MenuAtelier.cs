using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MenuAtelier : MenuSwapIcon
{
    private enum State { Core, Craft, Research };

    [SerializeField] private GameObject[] core_options;
    [SerializeField] private Text[] item_text;
    [SerializeField] private Text material_text, description_text;

    private PermDataHolder data_holder;

    private State current_state;

    private int[] research_options;

    private bool crafting_reseraching;

    private int y_overflow, y_max_overflow;

    private string[] current_names, current_descriptions, current_costs;
    private int[] current_index;

    public void Startup(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;
    }

    public void ActivateEX(int[] research_options)
    {
        base.Activate();

        crafting_reseraching = true;
        core_options[0].SetActive(crafting_reseraching);
        core_options[1].SetActive(!crafting_reseraching);

        this.research_options = research_options;

        y_value = 0;

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
            case State.Craft:
                CraftState(inputer);
                return false;
            case State.Research:
                ResearchState(inputer);
                return false;
        }

        return false;
    }

    private void CoreState(Inputer inputer)
    {
        if ((inputer.GetDir() == Direction.Left || inputer.GetDir() == Direction.Right) && inputer.GetMoveKeyUp())
        {
            crafting_reseraching = !crafting_reseraching;
            core_options[0].SetActive(crafting_reseraching);
            core_options[1].SetActive(!crafting_reseraching);
            return;
        }

        if (inputer.GetEnter())
        {
            if (crafting_reseraching)
                current_state = State.Craft;
            else
                current_state = State.Research;

            icons[y_value].SetActive(true);
            ReloadString();
        }
    }

    private void CraftState(Inputer inputer)
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
                data_holder.PurchaseResearch(current_index[(y_overflow * 8) + y_value]);
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

    private void ResearchState(Inputer inputer)
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
                data_holder.UnlockResearch(research_options[(y_overflow * 8) + y_value]);
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
        else if (current_state == State.Craft)
        {
            data_holder.GetRearchPurchase(out current_names, out current_descriptions, out current_costs, out current_index);
        }
        else if (current_state == State.Research)
        {
            data_holder.GetRearchUnlock(research_options, out current_names, out current_descriptions, out current_costs);
        }

        ReloadText();
    }
}