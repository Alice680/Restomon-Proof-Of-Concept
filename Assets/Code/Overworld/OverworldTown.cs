using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TownFeatureType { None, Home, Shop, Smith, Atelier, Workshop }

[CreateAssetMenu(fileName = "Town", menuName = "ScriptableObjects/Town")]
public class OverworldTown : ScriptableObject
{
    [Serializable]
    private class TownArea
    {
        public string area_name, area_description;

        public string back_txt;
        public string back_description;

        public int back_int;

        public TownEvent[] area_events;
    }

    [Serializable]
    private class AreaText
    {
        public string txt;
        public EventTrigger event_trigger;
    }

    [Serializable]
    private class TownEvent
    {
        public string txt;
        public string description;
        public int event_type;

        public int next_area;

        public DialogueTree dialogue;

        public TownFeatureType feature_type;

        public int feature_int;

        public EventTrigger event_trigger;
    }

    [SerializeField] private TownArea[] town_areas;

    public string GetBodyText(int index, out string area_name)
    {
        area_name = town_areas[index].area_name;
        return town_areas[index].area_description;
    }

    public string[] GetChoiceText(int index, PermDataHolder data_holder, out int[] icons, out string[] descriptions)
    {
        string[] temp_string = new string[8];
        icons = new int[8];
        descriptions = new string[8];

        TownArea temp_area = town_areas[index];

        int current_box = 0;

        for (int i = 0; i < temp_area.area_events.Length; ++i)
        {
            if (current_box == 7)
                break;

            if(temp_area.area_events[i].event_trigger.Check(data_holder))
            {
                temp_string[current_box] = temp_area.area_events[i].txt;
                descriptions[current_box] = temp_area.area_events[i].description;
                icons[current_box] = temp_area.area_events[i].event_type;

                ++current_box;
            }
        }

        temp_string[7] = temp_area.back_txt;
        descriptions[7] = temp_area.back_description;

        return temp_string;
    }

    public int GetSelection(PermDataHolder data_holder, int index, int choice, out DialogueTree dialogue , out TownFeatureType feature_type, out int feature_int)
    {
        TownArea temp_area = town_areas[index];

        if (choice == 7)
        {
            dialogue = null;
            feature_type = TownFeatureType.None;
            feature_int = 0;
            return temp_area.back_int;
        }

        choice = (choice / 2) + ((choice % 2) * 4);

        int current_box = 0;

        for (int i = 0; i < temp_area.area_events.Length; ++i)
        {
            if (temp_area.area_events[i].event_trigger.Check(data_holder))
            {
                if (current_box == choice)
                {
                    dialogue = temp_area.area_events[i].dialogue;
                    feature_type = temp_area.area_events[i].feature_type;
                    feature_int = temp_area.area_events[i].feature_int;
                    return (temp_area.area_events[i].next_area);
                }

                ++current_box;
            }
        }

        dialogue = null;
        feature_type = TownFeatureType.None;
        feature_int = 0;
        return -2;
    }
}