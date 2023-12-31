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
// TODO Add in summon
// TODO Add in evolution
// TODO ADD in items
// TODO Orginize once all options are added
public class ManagerMenuRestomonActions : MonoBehaviour
{
    private enum State { main, attack };

    [SerializeField] private DungeonManager manager_ref;
    [SerializeField] private PermDataHolder data_holder;

    [SerializeField] private MenuSwapIcon main_menu;
    [SerializeField] private MenuSwapIcon attack_menu;


    [SerializeField] private GameObject text_box;
    [SerializeField] private Text[] attack_text;
    [SerializeField] private Text attack_box_text;
    [SerializeField] private Text attack_hp_text;
    [SerializeField] private Text attack_mp_text;

    private State state;

    public void SetDataHolder(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;
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

                    text_box.SetActive(true);

                    state = State.attack;
                    exit_value = 0;
                    return -1;

                case 2:
                    main_menu.DeActivate();

                    exit_value = -1;
                    return 2;
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

                state = State.main;
                exit_value = attack_temp;
                return 1;
            }
        }

        exit_value = -1;
        return -1;
    }

    public void DirectionMenu(Direction dir)
    {
        switch (state)
        {
            case State.main:
                main_menu.UpdateMenu(dir);
                break;
            case State.attack:
                attack_menu.UpdateMenu(dir);
                int x, y;
                attack_menu.GetValues(out x, out y);
                attack_box_text.text = manager_ref.GetAttackDescription(manager_ref.GetIDFromActive(), (x * 4) + y);
                attack_hp_text.text = manager_ref.GetAttackCost(manager_ref.GetIDFromActive(), (x * 4) + y) + "";
                attack_mp_text.text = manager_ref.GetAttackMP(manager_ref.GetIDFromActive(), (x * 4) + y) + "";
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
            }

            text_box.SetActive(false);
            attack_box_text.text = "";
            attack_hp_text.text = "";
            attack_mp_text.text = "";

            main_menu.Activate();

            state = State.main;
            return false;
        }
    }
}