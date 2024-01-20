using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EventEffect
{
    [Serializable]
    private class DataEffect
    {
        public EventDataType event_type;
        public int index, value;
    }

    [SerializeField] private DataEffect[] data_effect;

    public void Apply(PermDataHolder data_holder)
    {
        foreach (DataEffect effect in data_effect)
            ApplyDataEffect(data_holder, effect);
    }

    private void ApplyDataEffect(PermDataHolder data_holder, DataEffect effect)
    {
        data_holder.SetEventData(effect.event_type, effect.index, effect.value);
    }
}