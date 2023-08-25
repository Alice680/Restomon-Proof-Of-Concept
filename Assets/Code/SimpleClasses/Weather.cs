using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weather", menuName = "ScriptableObjects/Weather")]
public class Weather : ScriptableObject
{
    [SerializeField] private string weather_name;
    [SerializeField] private GameObject tile_marker;
    [SerializeField] private Trait effect;
    [SerializeField] private int[] element_multi = new int[12];

    public string GetName()
    {
        return weather_name;
    }

    public GameObject GetModel()
    {
        return tile_marker;
    }

    public Trait GetWeatherCondition()
    {
        return effect;
    }

    public int GetDamageMulti(Element element)
    {
        return element_multi[((int)element) - 1];
    }
}