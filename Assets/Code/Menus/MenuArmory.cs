using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Menu for chosing your gear and class.
 *  
 * Notes:
 * Class, sub class and gear is chosen by sliding text
 * Traits will be chosen by binary choice
 * Tammer class and second trinket are excluded
 */
// TODO add in trait selection once working on traits
[Serializable]
public class MenuArmory : MenuSwapIcon
{
    private PermDataHolder data_holder;

    //0 class | 1 subclass  | 2 Weapon | 3 Armor | 4 Accessory| 5-12 trait
    [SerializeField] private Text[] text_boxes;

    [SerializeField] private GameObject[] core_icons;
    [SerializeField] private GameObject[] trait_icons;
    [SerializeField] private GameObject[] trait_markers;

    //0 subclass | 1 Weapon | 2 Armor | 3 Accessory
    [SerializeField] private int current_class;
    [SerializeField] private int[] current_values;

    [SerializeField] private Text trait_text;
    [SerializeField] private Text description_text;

    [SerializeField] private int[] traits = new int[3] { -1, -1, -1 };

    public override void Activate()
    {
        base.Activate();

        Display();
    }

    public override void UpdateMenu(Direction dir)
    {
        int dead_variable;

        if (y_value == 0)
        {
            if (dir == Direction.Down)
                y_value = 1;

            if (dir == Direction.Left || dir == Direction.Right)
                GetInputValue(x_value, 0, 2, 1, dir, out x_value, out dead_variable);
        }
        else if (x_value == 0)
        {
            if (dir == Direction.Up)
                y_value -= 1;

            if (dir == Direction.Down && y_value != 5)
                y_value += 1;

            if (dir == Direction.Right || dir == Direction.Left)
            {
                if (y_value == 1)
                {
                    GetInputValue(current_class, 0, 3, 1, dir, out current_class, out dead_variable);

                    for (int i = 0; i < 4; ++i)
                        current_values[i] = 0;

                    for (int i = 0; i < 3; ++i)
                        traits[i] = 0;
                }
                else if (y_value == 2)
                {
                    GetInputValue(current_values[0], 0, 3, 1, dir, out current_values[0], out dead_variable);

                    for (int i = 1; i < 4; ++i)
                        current_values[i] = 0;

                    for (int i = 0; i < 3; ++i)
                        traits[i] = 0;
                }
                else if (y_value == 4 || y_value == 5)
                {
                    if (dir == Direction.Right)
                    {
                        current_values[y_value - 2] += 1;

                        if (current_values[y_value - 2] == 6)
                            current_values[y_value - 2] = 0;

                        if (current_values[2] == current_values[3] && current_values[3] != 0)
                            current_values[y_value - 2] += 1;

                        if (current_values[y_value - 2] == 6)
                            current_values[y_value - 2] = 0;
                    }
                    if (dir == Direction.Left)
                    {
                        current_values[y_value - 2] -= 1;

                        if (current_values[y_value - 2] == -1)
                            current_values[y_value - 2] = 5;

                        if (current_values[2] == current_values[3] && current_values[3] != 0)
                            current_values[y_value - 2] -= 1;

                        if (current_values[y_value - 2] == -1)
                            current_values[y_value - 2] = 5;
                    }
                }
                else
                    GetInputValue(current_values[y_value - 2], 0, 4, 1, dir, out current_values[y_value - 2], out dead_variable);
            }
        }
        else
        {
            if (dir == Direction.Down)
            {
                if (y_value != 4 && y_value != 8)
                    y_value += 1;
            }

            if (dir == Direction.Up)
            {
                if (y_value == 1 || y_value == 5)
                    y_value = 0;
                else
                    y_value -= 1;
            }

            if (dir == Direction.Right || dir == Direction.Left)
            {
                if ((y_value - 1) / 4 == 0)
                    y_value += 4;
                else
                    y_value -= 4;
            }
        }

        for (int i = 0; i < 2; ++i)
            icons[i].SetActive(false);

        for (int i = 0; i < 5; ++i)
            core_icons[i].SetActive(false);

        for (int i = 0; i < 8; ++i)
            trait_icons[i].SetActive(false);

        if (y_value == 0)
            icons[x_value].SetActive(true);
        else if (x_value == 0)
            core_icons[y_value - 1].SetActive(true);
        else
            trait_icons[y_value - 1].SetActive(true);

        Display();
    }

    public void SetTrait()
    {
        if (x_value != 1 || y_value == 0)
            return;

        for (int i = 0; i < 3; ++i)
            if (traits[i] == y_value)
            {
                traits[i] = 0;
                Display();
                return;
            }

        for (int i = 0; i < 3; ++i)
            if (traits[i] == 0)
            {
                traits[i] = y_value;
                Display();
                return;
            }
    }

    public void LoadData(PermDataHolder data_holder)
    {
        /*this.data_holder = data_holder;

        int[] temp_int = data_holder.GetPlayerInt();
        current_class = temp_int[0];

        for (int i = 0; i < 4; ++i)
            current_values[i] = temp_int[i + 1];

        for (int i = 0; i < 3; ++i)
            traits[i] = temp_int[i + 5];*/
    }

    public void SetData()
    {
       // data_holder.SetPlayer(current_class, current_values[0], current_values[1], current_values[2], current_values[3], traits[0], traits[1], traits[2]);
    }

    private void Display()
    {
        HumanClass temp_human = data_holder.GetDataHuman(current_class);

        if (y_value == 1 && x_value == 0)
        {
            description_text.text = temp_human.GetClassDescription();
            text_boxes[0].text = "< " + temp_human.GetClassName() + " >";
        }
        else
            text_boxes[0].text = temp_human.GetClassName();

        if (y_value == 2 && x_value == 0)
        {
            description_text.text = temp_human.GetSubclassDescription(current_values[0]);
            text_boxes[1].text = "< " + temp_human.GetSubclassName(current_values[0]) + " >";
        }
        else
            text_boxes[1].text = temp_human.GetSubclassName(current_values[0]);

        if (y_value == 3 && x_value == 0)
        {
            description_text.text = temp_human.GetWeaponDescription(current_values[1]);
            text_boxes[2].text = "< " + temp_human.GetWeaponName(current_values[1]) + " >";
        }
        else
            text_boxes[2].text = temp_human.GetWeaponName(current_values[1]);

        if (y_value == 4 && x_value == 0)
        {
            description_text.text = temp_human.GetTrinketDescription(current_values[2]);
            text_boxes[3].text = "< " + temp_human.GetTrinketName(current_values[2]) + " >";
        }
        else
            text_boxes[3].text = temp_human.GetTrinketName(current_values[2]);

        if (y_value == 5 && x_value == 0)
        {
            description_text.text = temp_human.GetTrinketDescription(current_values[3]);
            text_boxes[4].text = "< " + temp_human.GetTrinketName(current_values[3]) + " >";
        }
        else
            text_boxes[4].text = temp_human.GetTrinketName(current_values[3]);

        for (int i = 0; i < 6; ++i)
        {
            text_boxes[5 + i].text = temp_human.GetTraitName(i + 1);

            if (x_value == 1 && y_value == i + 1)
                description_text.text = temp_human.GetTraitDescription(i + 1);
        }

        for (int i = 0; i < 2; ++i)
        {
            text_boxes[11 + i].text = temp_human.GetSubTraitName(current_values[0], i);

            if (x_value == 1 && y_value-7 == i)
                description_text.text = temp_human.GetSubTraitDescription(current_values[0], i);
        }

        for (int i = 0; i < 8; ++i)
            trait_markers[i].SetActive(false);

        int num_traits = 0;

        for (int i = 0; i < 3; ++i)
            if (traits[i] != 0)
            {
                ++num_traits;
                trait_markers[traits[i] - 1].SetActive(true);
            }

        trait_text.text = num_traits + "/3";
    }
}