using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileConditionHolder", menuName = "ScriptableObjects/TileConditionHolder")]
public class TileConditionHolder : ScriptableObject
{
    [SerializeField] TileCondition[] tile_conditions;

    public string GetName(int index)
    {
        if (index < 0 || index > +tile_conditions.Length)
            return null;

        return tile_conditions[index].GetName();
    }

    public GameObject GetModel(int index)
    {
        if (index < 0 || index > +tile_conditions.Length)
            return null;

        return tile_conditions[index].GetModel();
    }

    public Trait GetEffect(int index)
    {
        if (index < 0 || index > +tile_conditions.Length)
            return null;

        return tile_conditions[index].GetEffect();
    }
}