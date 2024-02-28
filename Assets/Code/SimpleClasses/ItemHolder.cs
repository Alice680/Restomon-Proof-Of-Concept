using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/ItemHolder")]
public class ItemHolder : ScriptableObject
{
    [SerializeField] private ItemData[] items;

    private static ItemData[] static_items;

    public void SetList()
    {
        static_items = new ItemData[items.Length];

        for (int i = 0; i < items.Length; ++i)
            static_items[i] = items[i];
    }

    public static ItemData GetItem(int index)
    {
        return static_items[index];
    }
}