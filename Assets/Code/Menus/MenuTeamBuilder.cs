using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MenuTeamBuilder : MenuSwapIcon
{
    [Serializable]
    private class BaseBuild
    {
        public string name;
        public int[] attack_ids;
        public int[] trait_ids;
    }

    [Serializable]
    private class EvolutionBuild
    {
        public string name;
        public int[] attack_ids;
        public int[] trait_ids;
    }

    [Serializable]
    private class CreatureBuild
    {
        public string name;
        public BaseBuild[] base_build;
        public EvolutionBuild[] evo_one_builds;
        public EvolutionBuild[] evo_two_builds;
        public EvolutionBuild[] evo_three_builds;
    }

    [SerializeField] private string[] catalyst;
    [SerializeField] private CreatureBuild[] creature_builds;
    [SerializeField] private int[] values;
    [SerializeField] private Text[] text_boxes;

    public override void Activate()
    {
        base.Activate();

        foreach (GameObject icon in icons)
            icon.SetActive(false);

        icons[x_value + (y_value * x_size)].SetActive(true);

        Display();
    }

    public override void UpdateMenu(Direction dir)
    {
        switch (y_value)
        {
            case 0:
                if (dir == Direction.Down)
                {
                    y_value = 1;
                }
                else if (dir != Direction.Up)
                {
                    ChangeValue(dir, 0, 2);
                    for (int i = 1; i <= 18; ++i)
                        values[i] = 0;
                }
                break;

            case 1:
            case 7:
            case 13:
                if (dir == Direction.Up)
                {
                    y_value = 0;
                }
                else if (dir == Direction.Down)
                {
                    if (y_value == 1)
                        ++y_value;
                    if (y_value == 7 && values[0] > 0)
                        ++y_value;
                    if (y_value == 13 && values[0] > 1)
                        ++y_value;
                }
                else if (dir == Direction.Right)
                {
                    y_value += 6;

                    if (y_value > 13)
                        y_value = 1;
                }
                else if (dir == Direction.Left)
                {
                    y_value -= 6;

                    if (y_value < 1)
                        y_value = 13;
                }

                break;

            case 6:
            case 12:
            case 18:
                if (dir == Direction.Up)
                    --y_value;

                else if (dir != Direction.Down)
                    ChangeValue(dir, y_value, 1);
                break;

            default:
                if (dir == Direction.Up)
                    --y_value;

                else if (dir == Direction.Down)
                    ++y_value;

                else
                {
                    if (y_value == 2 || y_value == 8 || y_value == 14)
                    {
                        ChangeValue(dir, y_value, 5);

                        for (int i = y_value + 1; i <= y_value + 4; ++i)
                            values[i] = 0;
                    }
                    else if (y_value == 3 || y_value == 9 || y_value == 15)
                    {
                        ChangeValue(dir, y_value, 2);
                    }
                    else
                    {
                        ChangeValue(dir, y_value, 1);
                    }
                }
                break;
        }

        Display();
    }

    public void SetData(PermDataHolder data_holder)
    {
        data_holder.SetCatalyst(values[0]);

        for (int i = 0; i <= 2; ++i)
            data_holder.SetRestomon(i);

        int[] temp_attacks = new int[10];
        int[] temp_traits = new int[5];

        for (int i = 0; i <= values[0]; ++i)
        {
            int temp_id = values[2 + (i * 6)];

            for (int e = 0; e < 4; ++e)
                temp_attacks[e] = creature_builds[temp_id].base_build[values[3 + (i * 6)]].attack_ids[e];

            for (int e = 0; e < 4; ++e)
            {
                temp_attacks[4 + e] = 0;
            }

            data_holder.SetRestomon(i, temp_id, temp_attacks, temp_traits);
        }
    }

    private void ChangeValue(Direction dir, int row, int max)
    {
        if (dir == Direction.Right)
        {
            ++values[row];
            if (values[row] > max)
                values[row] = 0;
        }
        else if (dir == Direction.Left)
        {
            --values[row];
            if (values[row] < 0)
                values[row] = max;
        }
    }

    private void Display()
    {
        text_boxes[0].text = catalyst[values[0]];

        text_boxes[1].text = "Slot 1";
        text_boxes[2].text = creature_builds[values[2]].name;
        text_boxes[3].text = creature_builds[values[2]].base_build[values[3]].name;
        text_boxes[4].text = creature_builds[values[2]].evo_one_builds[values[4]].name;
        text_boxes[5].text = creature_builds[values[2]].evo_two_builds[values[5]].name;
        text_boxes[6].text = creature_builds[values[2]].evo_three_builds[values[6]].name;

        if (values[0] > 0)
        {
            text_boxes[7].text = "Slot 2";
            text_boxes[8].text = creature_builds[values[8]].name;
            text_boxes[9].text = creature_builds[values[8]].base_build[values[9]].name;
            text_boxes[10].text = creature_builds[values[8]].evo_one_builds[values[10]].name;
            text_boxes[11].text = creature_builds[values[8]].evo_two_builds[values[11]].name;
            text_boxes[12].text = creature_builds[values[8]].evo_three_builds[values[12]].name;
        }
        else
        {
            text_boxes[7].text = "Locked";
            for (int i = 8; i <= 12; ++i)
                text_boxes[i].text = "";
        }

        if (values[0] > 1)
        {
            text_boxes[13].text = "Slot 3";
            text_boxes[14].text = creature_builds[values[14]].name;
            text_boxes[15].text = creature_builds[values[14]].base_build[values[15]].name;
            text_boxes[16].text = creature_builds[values[14]].evo_one_builds[values[16]].name;
            text_boxes[17].text = creature_builds[values[14]].evo_two_builds[values[17]].name;
            text_boxes[18].text = creature_builds[values[14]].evo_three_builds[values[18]].name;
        }
        else
        {
            text_boxes[13].text = "Locked";
            for (int i = 14; i <= 18; ++i)
                text_boxes[i].text = "";
        }

        foreach (GameObject icon in icons)
            icon.SetActive(false);

        icons[x_value + (y_value * x_size)].SetActive(true);
    }
}