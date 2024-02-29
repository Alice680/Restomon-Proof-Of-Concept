using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string item_name, description;
    [SerializeField] private int value;
    [SerializeField] private bool is_active, is_sellable;
    [SerializeField] private Attack effect;
    [SerializeField] private GameObject dungeon_model, inventory_model;

    public string GetInfo(out string info)
    {
        info = description;
        return item_name;
    }

    public Attack GetEffect(bool has_effect)
    {
        has_effect = is_active;
        return effect;
    }

    public int GetValue(bool has_value)
    {
        has_value = is_sellable;
        return value;
    }

    public GameObject GetDungeonModel()
    {
        return Instantiate(dungeon_model);
    }

    public GameObject GetInventoryModel()
    {
        return Instantiate(inventory_model);
    }
}