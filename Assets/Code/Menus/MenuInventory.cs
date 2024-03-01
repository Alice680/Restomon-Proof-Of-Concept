using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MenuInventory : MenuSwapIcon
{
    [SerializeField] private Text[] item_text;

    private PermDataHolder data_holder;

    private int layers, current_layer;
    private string[] item_names, item_descriptions;
    private bool[] item_active;
    private int[] item_id;

    public void Startup(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;
    }

    public override void Activate()
    {
        base.Activate();

        item_names = new string[data_holder.GetInventoryCount()];
        item_descriptions = new string[data_holder.GetInventoryCount()];
        item_active = new bool[data_holder.GetInventoryCount()];
        item_id = new int[data_holder.GetInventoryCount()];

        current_layer = 0;
        layers = (item_names.Length / 8) + 1;

        for (int i = 0; i < data_holder.GetInventoryCount(); ++i)
        {
            item_id[i] = data_holder.GetInventorySlot(i);
            ItemData temp_item = ItemHolder.GetItem(item_id[i]);

            item_names[i] = temp_item.GetInfo(out item_descriptions[i]);
            temp_item.GetEffect(out item_active[i]);
        }

        for (int i = 0; i < 8; ++i)
            item_text[i].text = (i < item_names.Length) ? item_names[i] : "";
    }

    public override void UpdateMenu(Direction dir)
    {
        if ((dir == Direction.Up && y_value == 0) || (dir == Direction.Down && y_value == 3))
        {
            GetInputValue(x_value, current_layer, x_size, layers, dir, out x_value, out current_layer);

            for (int i = 0; i < 8; ++i)
                item_text[i].text = (i + (current_layer * 8) < item_names.Length) ? item_names[i + (current_layer * 8)] : "";
        }

        base.UpdateMenu(dir);
    }

    public string GetDescription()
    {
        if ((current_layer * 8) + (y_value + (x_value * 4)) >= item_names.Length)
            return "";

        return item_descriptions[(current_layer * 8) + (y_value + (x_value * 4))];
    }

    public int GetItem(out bool is_active)
    {
        if ((current_layer * 8) + (y_value + (x_value * 4)) >= item_names.Length)
        {
            is_active = false;
            return -1;
        }

        is_active = item_active[(current_layer * 8) + (y_value + (x_value * 4))];
        return (current_layer * 8) + (y_value + (x_value * 4));
    }
}