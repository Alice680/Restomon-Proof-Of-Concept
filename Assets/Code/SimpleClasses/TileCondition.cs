using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileCondition", menuName = "ScriptableObjects/TileCondition")]
public class TileCondition : ScriptableObject
{
    public string condition_name;
    public GameObject model;
    public Trait effect;

    public string GetName()
    {
        return condition_name;
    }

    public GameObject GetModel()
    {
        return model;
    }

    public Trait GetEffect()
    {
        return effect;
    }
}