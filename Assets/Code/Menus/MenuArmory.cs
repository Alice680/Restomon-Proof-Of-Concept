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
    [Serializable]
    private class ClassData
    {
        public string name;
        public string[] traits;
        public string[] weapons, armors, accessories;
        public SubclassData[] subclasses;
    }

    [Serializable]
    private class SubclassData
    {
        public string name;
        public string traita;
        public string traitb;
    }

    //0 class | 1 subclass | 2-9 trait | 10 Weapon | 11 Armor | 12 Accessory
    [SerializeField] private Text[] text_boxes;

    //0-7 Traits
    [SerializeField] private GameObject[] markers;
    [SerializeField] private bool[] trait_selected;

    //0 subclass | 1 Weapon | 2 Armor | 3 Accessory
    [SerializeField] private int current_class;
    [SerializeField] private int[] current_values;

    [SerializeField] private ClassData[] class_data;

    public override void Activate()
    {
        base.Activate();

        Display();
    }

    public override void UpdateMenu(Direction dir)
    {
        if ((dir == Direction.Up || dir == Direction.Down) && (y_value <= 1 || y_value >= 10))
        {
            base.UpdateMenu(dir);
        }
        else if ((dir == Direction.Left || dir == Direction.Right) && (y_value <= 1 || y_value >= 10))
        {
            int dead_variable;
            switch (y_value)
            {
                case 0:
                    GetInputValue(current_class, 0, 3, 1, dir, out current_class, out dead_variable);
                    break;
                case 1:
                    GetInputValue(current_values[0], 0, 3, 1, dir, out current_values[0], out dead_variable);
                    break;
                case 10:
                    GetInputValue(current_values[1], 0, 4, 1, dir, out current_values[1], out dead_variable);
                    break;
                case 11:
                    GetInputValue(current_values[2], 0, 4, 1, dir, out current_values[2], out dead_variable);
                    break;
                case 12:
                    GetInputValue(current_values[3], 0, 4, 1, dir, out current_values[3], out dead_variable);
                    break;
            }

            Display();
        }
        else if ((dir == Direction.Up || dir == Direction.Down))
        {
            if (dir == Direction.Up)
            {
                if (y_value <= 5)
                    y_value = 1;
                else
                    y_value -= 4;
            }
            else
            {
                if (y_value >= 6)
                    y_value = 10;
                else
                    y_value += 4;
            }
        }
        else
        {
            GetInputValue(y_value - 2, 0, 8, 1, dir, out y_value, out int dead_value);
            y_value += 2;
        }

        base.UpdateMenu(Direction.None);
    }

    public void SetData(PermDataHolder data_holder)
    {
        data_holder.SetPlayer(current_class, current_values[0], current_values[1], current_values[2], current_values[3], 0, 0, 0);
    }

    private void Display()
    {
        for (int i = 0; i < 8; ++i)
        {
            markers[i].SetActive(trait_selected[i]);
        }

        text_boxes[0].text = class_data[current_class].name;
        text_boxes[1].text = class_data[current_class].subclasses[current_values[0]].name;
        text_boxes[2].text = class_data[current_class].traits[0];
        text_boxes[3].text = class_data[current_class].traits[1];
        text_boxes[4].text = class_data[current_class].traits[2];
        text_boxes[5].text = class_data[current_class].traits[3];
        text_boxes[6].text = class_data[current_class].traits[4];
        text_boxes[7].text = class_data[current_class].traits[5];
        text_boxes[8].text = class_data[current_class].subclasses[current_values[0]].traita;
        text_boxes[9].text = class_data[current_class].subclasses[current_values[0]].traitb;
        text_boxes[10].text = class_data[current_class].weapons[current_values[1]];
        text_boxes[11].text = class_data[current_class].armors[current_values[2]];
        text_boxes[12].text = class_data[current_class].accessories[current_values[3]];
    }
}