using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EventTrigger
{
    [Serializable] private enum DataType { Class };

    [Serializable]
    private class DataTriggers
    {
        public EventDataType event_type;
        public bool is_equal;
        public int index, value;
    }

    [Serializable]
    private class DungeonTriggers
    {
        public DungeonDataType event_type;
        public bool is_equal;
        public int index;
        public bool value;
    }

    [Serializable]
    private class SaveTriggers
    {
        public DataType data_type;
        public bool is_equal;
        public int index, value;
    }

    [SerializeField] private DataTriggers[] data_triggers;
    [SerializeField] private DungeonTriggers[] dungeon_triggers;
    [SerializeField] private SaveTriggers[] save_triggers;

    public bool Check(PermDataHolder data_holder)
    {
        foreach (DataTriggers trigger in data_triggers)
            if ((!CheckDataTrigger(data_holder, trigger)))
                return false;

        foreach (DungeonTriggers trigger in dungeon_triggers)
            if (!CheckDungeonTrigger(data_holder, trigger))
                return false;

        foreach (SaveTriggers trigger in save_triggers)
            if (!CheckSaveTrigger(data_holder, trigger))
                return false;

        return true;
    }

    private bool CheckDataTrigger(PermDataHolder data_holder, DataTriggers trigger)
    {
        return data_holder.GetEventData(trigger.event_type, trigger.index) == trigger.value ^ !trigger.is_equal;
    }

    private bool CheckDungeonTrigger(PermDataHolder data_holder, DungeonTriggers trigger)
    {
        return data_holder.GetDungeonData(trigger.event_type, trigger.index) == trigger.value ^ !trigger.is_equal;
    }

    private bool CheckSaveTrigger(PermDataHolder data_holder, SaveTriggers trigger)
    {
        if (trigger.data_type == DataType.Class)
        {
           
            return (trigger.value == data_holder.GetPlayerClass()) ^ !trigger.is_equal;
        }

        return false;
    }
}