using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EventTrigger
{
    [Serializable]
    private class DataTriggers
    {
        public EventDataType event_type;
        public int index, value;
    }

    [SerializeField] private DataTriggers[] data_triggers;

    public bool Check(PermDataHolder data_holder)
    {
        foreach (DataTriggers trigger in data_triggers)
            if (!CheckDataTrigger(data_holder, trigger))
                return false;
        
        return true;
    }

    private bool CheckDataTrigger(PermDataHolder data_holder, DataTriggers trigger)
    {
        return data_holder.GetEventData(trigger.event_type, trigger.index) == trigger.value;
    }
}