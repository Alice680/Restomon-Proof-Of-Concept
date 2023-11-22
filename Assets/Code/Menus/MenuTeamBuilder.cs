using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MenuTeamBuilder : MenuSwapIcon
{
    private PermDataHolder data_holder;

    private class RestomonData
    {
        public int restomon_id;
        public int[] form_value;

        public RestomonData()
        {
            restomon_id = 0;
            form_value = new int[4] { 0, 0, 0, 0 };
        }

        public RestomonData(int restomon_id, int[] form_value)
        {
            this.restomon_id = restomon_id;
            this.form_value = form_value;
        }
    }

    [Serializable]
    private class CreatureBuild
    {
        public string name, description;
        public string[] base_builds_name, builds_a_name, builds_b_name, builds_c_name;
        public string[] base_builds_description, builds_a_description, builds_b_description, builds_c_description;
    }

    [SerializeField] private CreatureBuild[] creature_builds;
    [SerializeField] private Text[] text_boxes;

    private int catalyst_value;
    private RestomonData[] restomon_values;

    public override void Activate()
    {
        menu.SetActive(true);

        UpdateMenu(Direction.None);
        Display();
    }

    public override void UpdateMenu(Direction dir)
    {
        int dead_variable;

        if (y_value == 0)
        {
            if (dir == Direction.Down)
                y_value = 1;

            if (dir == Direction.Right || dir == Direction.Left)
                GetInputValue(catalyst_value, 0, 3, 0, dir, out catalyst_value, out dead_variable);

            x_value = 0;

            for (int i = 0; i < 4; ++i)
                restomon_values[i] = new RestomonData();
        }
        else if (y_value == 1)
        {
            if (dir == Direction.Down)
                y_value = 2;

            if (dir == Direction.Up)
                y_value -= 1;

            if (dir == Direction.Right || dir == Direction.Left)
                GetInputValue(x_value, 0, data_holder.GetDataCatalyst(catalyst_value).GetRestomonAmount(), 0, dir, out x_value, out dead_variable);
        }
        else
        {
            if (dir == Direction.Down && y_value != 6)
                y_value += 1;

            if (dir == Direction.Up)
                y_value -= 1;


            if (dir == Direction.Right || dir == Direction.Left)
            {
                if (y_value == 2)
                {
                    GetInputValue(restomon_values[x_value].restomon_id, 0, 6, 0, dir, out restomon_values[x_value].restomon_id, out dead_variable);

                    for (int i = 0; i < 4; ++i)
                        restomon_values[x_value].form_value[i] = 0;
                }

                else if (y_value == 3)
                    GetInputValue(restomon_values[x_value].form_value[y_value - 3], 0, 3, 0, dir, out restomon_values[x_value].form_value[y_value - 3], out dead_variable);
                else
                    GetInputValue(restomon_values[x_value].form_value[y_value - 3], 0, 2, 0, dir, out restomon_values[x_value].form_value[y_value - 3], out dead_variable);

            }
        }

        for (int i = 0; i < 10; ++i)
            icons[i].SetActive(false);

        icons[x_value + 1].SetActive(true);

        if (y_value == 0)
            icons[0].SetActive(true);

        if (y_value > 1)
            icons[3 + y_value].SetActive(true);

        Display();
    }

    public void LoadData(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;

        catalyst_value = data_holder.GetCatalystInt();

        restomon_values = new RestomonData[4] { new RestomonData(), new RestomonData(), new RestomonData(), new RestomonData() };
    }

    public void SetData()
    {
        data_holder.SetCatalyst(catalyst_value);

        for (int i = 0; i < 4; ++i)
            data_holder.SetRestomon(i, restomon_values[i].restomon_id, restomon_values[i].form_value);
    }

    private void Display()
    {
        Catalyst temp_catalyst = data_holder.GetDataCatalyst(catalyst_value);

        text_boxes[0].text = "";

        if (y_value == 0)
        {
            text_boxes[1].text = "< " + temp_catalyst.GetName() + " >";
            text_boxes[0].text = temp_catalyst.GetDescription();
        }
        else
            text_boxes[1].text = temp_catalyst.GetName();

        int form_id = restomon_values[x_value].restomon_id;

        if (y_value == 2)
        {
            text_boxes[2].text = "< " + creature_builds[form_id].name + " >";
            text_boxes[0].text = creature_builds[form_id].description;
        }
        else
            text_boxes[2].text = creature_builds[form_id].name;


        if (y_value == 3)
        {
            text_boxes[3].text = "< " + creature_builds[form_id].base_builds_name[restomon_values[x_value].form_value[0]] + " >";
            text_boxes[0].text = creature_builds[form_id].base_builds_description[restomon_values[x_value].form_value[0]];
        }
        else
            text_boxes[3].text = creature_builds[form_id].base_builds_name[restomon_values[x_value].form_value[0]];

        if (y_value == 4)
        {
            text_boxes[4].text = "< " + creature_builds[form_id].builds_a_name[restomon_values[x_value].form_value[1]] + " >";
            text_boxes[0].text = creature_builds[form_id].builds_a_description[restomon_values[x_value].form_value[1]];
        }
        else
            text_boxes[4].text = creature_builds[form_id].builds_a_name[restomon_values[x_value].form_value[1]];

        if (y_value == 5)
        {
            text_boxes[5].text = "< " + creature_builds[form_id].builds_b_name[restomon_values[x_value].form_value[2]] + " >";
            text_boxes[0].text = creature_builds[form_id].builds_b_description[restomon_values[x_value].form_value[2]];
        }
        else
            text_boxes[5].text = creature_builds[form_id].builds_b_name[restomon_values[x_value].form_value[2]];

        if (y_value == 6)
        {
            text_boxes[6].text = "< " + creature_builds[form_id].builds_c_name[restomon_values[x_value].form_value[3]] + " >";
            text_boxes[0].text = creature_builds[form_id].builds_c_description[restomon_values[x_value].form_value[3]];
        }
        else
            text_boxes[6].text = creature_builds[form_id].builds_c_name[restomon_values[x_value].form_value[3]];
    }
}