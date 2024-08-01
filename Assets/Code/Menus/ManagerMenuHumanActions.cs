using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Menu that handles actions in combat.
 * It is always called by the player from the idle state.
 * Its return determins what state the player transfers to.
 * Its output determins what the have selected
 * 
 * Notes:
 */
public class ManagerMenuHumanActions : MonoBehaviour
{
    private enum State { main, attack, summon, item };

    [SerializeField] private DungeonManager manager_ref;
    [SerializeField] private PermDataHolder data_holder;

    [SerializeField] private GameObject text_box;

    [SerializeField] private MenuSwapIcon main_menu;
    [SerializeField] private MenuSwapIcon attack_menu, summon_menu;
    [SerializeField] private MenuInventory item_menu;

    [SerializeField] private Text[] attack_text, summon_text;
    [SerializeField] private Text attack_box_text, attack_hp_text;

    private State state;

    public void SetDataHolder(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;
        item_menu.Startup(data_holder);
    }

    public void OpenActionMenu()
    {
        main_menu.Activate();

        state = State.main;
    }

    public int EnterMenu(out int exit_value)
    {
        if (state == State.main)
        {
            int value;
            main_menu.GetValues(out int empty, out value);

            int x, y;

            switch (value)
            {
                case 0:
                    main_menu.DeActivate();

                    exit_value = -1;
                    return 0;

                case 1:
                    main_menu.DeActivate();
                    attack_menu.Activate();

                    for (int i = 0; i < 8; ++i)
                        attack_text[i].text = manager_ref.GetAttackName(manager_ref.GetIDFromActive(), i);

                    attack_menu.GetValues(out x, out y);
                    attack_box_text.text = manager_ref.GetAttackDescription(manager_ref.GetIDFromActive(), (x * 4) + y);
                    attack_hp_text.text = manager_ref.GetAttackCost(manager_ref.GetIDFromActive(), (x * 4) + y) + "";

                    text_box.SetActive(true);

                    state = State.attack;
                    exit_value = 0;
                    return -1;

                case 2:
                    main_menu.DeActivate();
                    summon_menu.Activate();

                    Catalyst temp_catalyst = data_holder.GetCatalyst();

                    for (int i = 0; i < 4; ++i)
                    {
                        if (i < temp_catalyst.GetRestomonAmount() && data_holder.GetTeam(i) != null)
                            summon_text[i].text = data_holder.GetTeam(i).ToString();
                        else
                            summon_text[i].text = "Empty Slot";
                    }

                    summon_menu.GetValues(out x, out y);

                    if (y < data_holder.GetCatalyst().GetRestomonAmount() && data_holder.GetTeam(y) != null)
                        attack_hp_text.text = data_holder.GetCatalyst().GetSummonCost(data_holder.GetTeam(y).GetSummonCost(RestomonEvolution.None, -1)) + "";
                    else
                        attack_hp_text.text = "0";

                    text_box.SetActive(true);

                    state = State.summon;
                    exit_value = 0;
                    return -1;

                case 3:
                    main_menu.DeActivate();

                    exit_value = -1;
                    return 3;

                case 4:
                    main_menu.DeActivate();
                    item_menu.Activate();

                    text_box.SetActive(true);
                    attack_box_text.text = item_menu.GetDescription();

                    state = State.item;
                    exit_value = 0;
                    return -1;

                case 5:
                    main_menu.DeActivate();

                    exit_value = -1;
                    return 5;
            }
        }
        else if (state == State.attack)
        {
            int x, y;

            attack_menu.GetValues(out x, out y);

            int id_temp = manager_ref.GetIDFromActive();

            int attack_temp = y + (x * 4);

            if (manager_ref.GetActions() == 0 || manager_ref.GetAttackCost(id_temp, attack_temp) > manager_ref.GetHP(id_temp))
            {
                exit_value = 0;
                return -1;
            }
            else
            {
                attack_menu.DeActivate();

                text_box.SetActive(false);
                attack_box_text.text = "";
                attack_hp_text.text = "";

                state = State.main;
                exit_value = attack_temp;
                return 1;
            }
        }
        else if (state == State.summon)
        {
            summon_menu.GetValues(out int nothing, out exit_value);

            if (!manager_ref.SummonValid(exit_value))
            {
                return -1;
            }
            else
            {
                summon_menu.DeActivate();

                text_box.SetActive(false);
                attack_box_text.text = "";
                attack_hp_text.text = "";

                state = State.main;
                return 2;
            }
        }
        else if (state == State.item)
        {
            exit_value = item_menu.GetItem(out bool is_active_item);

            if (is_active_item && manager_ref.GetActions() > 0)
            {
                item_menu.DeActivate();

                text_box.SetActive(false);
                attack_box_text.text = "";
                attack_hp_text.text = "";

                state = State.main;

                return 4;
            }
        }

        exit_value = -1;
        return -1;
    }

    public void DirectionMenu(Direction dir)
    {
        int x, y;

        switch (state)
        {
            case State.main:
                main_menu.UpdateMenu(dir);
                break;

            case State.attack:
                attack_menu.UpdateMenu(dir);
                attack_menu.GetValues(out x, out y);
                attack_box_text.text = manager_ref.GetAttackDescription(manager_ref.GetIDFromActive(), (x * 4) + y);
                attack_hp_text.text = manager_ref.GetAttackCost(manager_ref.GetIDFromActive(), (x * 4) + y) + "";
                break;

            case State.summon:
                summon_menu.UpdateMenu(dir);

                summon_menu.GetValues(out x, out y);
                if (y < data_holder.GetCatalyst().GetRestomonAmount() && data_holder.GetTeam(y) != null)
                    attack_hp_text.text = data_holder.GetCatalyst().GetSummonCost(data_holder.GetTeam(y).GetSummonCost(RestomonEvolution.None, -1)) + "";
                else
                    attack_hp_text.text = "0";
                break;

            case State.item:
                item_menu.UpdateMenu(dir);
                attack_box_text.text = item_menu.GetDescription();

                break;
        }
    }

    public bool ReturnMenu()
    {
        if (state == State.main)
        {
            main_menu.DeActivate();
            return true;
        }
        else
        {
            switch (state)
            {
                case State.attack:
                    attack_menu.DeActivate();
                    break;
                case State.summon:
                    summon_menu.DeActivate();
                    break;
                case State.item:
                    item_menu.DeActivate();
                    break;
            }

            text_box.SetActive(false);
            attack_box_text.text = "";
            attack_hp_text.text = "";

            main_menu.Activate();

            state = State.main;
            return false;
        }
    }
}