using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Town", menuName = "ScriptableObjects/Town")]
public class OverworldTown : ScriptableObject
{
    [Serializable]
    private class TownArea
    {
        public string back_txt;

        public AreaText[] text_options;

        public TownEvent[] area_events;
    }

    [Serializable]
    private class AreaText
    {
        public string name;
        public string txt;
        public EventTrigger event_trigger;
    }

    [Serializable]
    private class TownEvent
    {
        public string txt;

        public int next_area;

        public EventTrigger event_trigger;
    }

    [SerializeField] private TownArea[] town_areas;

    public string GetBodyText(int index, PermDataHolder data_holder, out string area_name)
    {
        area_name = town_areas[index].text_options[0].name;
        return town_areas[index].text_options[0].txt;
    }

    public string[] GetChoiceText(int index, PermDataHolder data_holder)
    {
        string[] temp_string = new string[8];

        TownArea temp_area = town_areas[index];

        int current_box = 0;

        for (int i = 0; i < temp_area.area_events.Length; ++i)
        {
            if (current_box == 7)
                break;

            if(temp_area.area_events[i].event_trigger.Check(data_holder))
            {
                temp_string[current_box] = temp_area.area_events[i].txt;

                ++current_box;
            }
        }

        temp_string[7] = temp_area.back_txt;

        return temp_string;
    }

    public int GetSelection(PermDataHolder data_holder, int index, int choice)
    {
        if (choice == 7)
            return -1;

        TownArea temp_area = town_areas[index];

        int current_box = 0;

        for (int i = 0; i < temp_area.area_events.Length; ++i)
        {
            if (temp_area.area_events[i].event_trigger.Check(data_holder))
            {
                if (current_box == choice)
                    return (temp_area.area_events[i].next_area);

                ++current_box;
            }
        }

        return -2;
    }
}