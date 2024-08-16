using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MenuStorage : MenuSwapIcon
{
    [SerializeField] private Text[] name_text;
    [SerializeField] private Text inventory_page_text, storage_page_text;

    private PermDataHolder data_holder;
    private int inventory_page, storage_page, inventory_page_count, storage_page_count;
    private int[] storage_index;
    private string[] inventory_names, inventory_descriptions, storage_names, storage_descriptions;

    public void SetData(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;
    }

    public override void Activate()
    {
        inventory_page = 0;
        storage_page = 0;

        UpdateString();

        base.Activate();
    }

    public bool Run(Inputer inputer)
    {
        if (inputer.GetDir() != Direction.None && inputer.GetMoveKeyUp())
        {
            GetInputValue(x_value, y_value, 2, 8, inputer.GetDir(), out x_value, out y_value);

            for (int i = 0; i < icons.Length; ++i)
                icons[i].SetActive(false);

            icons[y_value + (x_value * 8)].SetActive(true);
        }
        else if (inputer.GetEnter())
        {
            if (x_value == 0)
            {
                if ((inventory_page * 8) + y_value < data_holder.GetInventoryCount())
                {
                    data_holder.ChangeStorage(data_holder.GetInventorySlot((inventory_page * 8) + y_value), 1);

                    data_holder.RemoveInventory((inventory_page * 8) + y_value);

                    UpdateString();
                }
            }
            else
            {
                if (data_holder.GetInventoryCount() < data_holder.GetInventorySize() && storage_names.Length != 0)
                {
                    data_holder.ChangeStorage(storage_index[(storage_page * 8) + y_value], -1);

                    data_holder.AddInventory(storage_index[(storage_page * 8) + y_value]);

                    UpdateString();
                }
            }
        }
        else if (inputer.GetBack())
        {
            return true;
        }

        return false;
    }

    private void UpdateText()
    {
        for (int i = 0; i < 8; ++i)
        {
            if ((inventory_page * 8) + i < inventory_names.Length)
                name_text[i].text = inventory_names[(inventory_page * 8) + i];
            else
                name_text[i].text = "";

            if ((storage_page * 8) + i < storage_names.Length)
                name_text[i + 8].text = storage_names[(inventory_page * 8) + i];
            else
                name_text[i + 8].text = "";
        }

        inventory_page_text.text = (inventory_page + 1) + "/" + (inventory_names.Length / 8 + 1);
        storage_page_text.text = (storage_page + 1) + "/" + (storage_names.Length / 8 + 1);
    }

    private void UpdateString()
    {
        inventory_names = new string[data_holder.GetInventoryCount()];
        inventory_descriptions = new string[data_holder.GetInventoryCount()];

        for (int i = 0; i < data_holder.GetInventoryCount(); ++i)
        {
            inventory_names[i] = ItemHolder.GetItem(data_holder.GetInventorySlot(i)).GetInfo(out string inventory_info);
            inventory_descriptions[i] = inventory_info;
        }

        storage_names = data_holder.GetStorageData(out storage_index, out storage_descriptions);

        UpdateText();
    }
}