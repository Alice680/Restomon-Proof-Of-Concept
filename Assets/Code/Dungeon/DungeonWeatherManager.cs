using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonWeatherManager : MonoBehaviour
{
    [SerializeField] private Weather[] weather_list;

    private int current_weather, current_power;

    private DungeonManager manager_ref;
    private DungeonMap map;

    public void Setup(DungeonManager manager_ref, DungeonMap map, int current_weather, int current_power)
    {
        this.current_weather = current_weather;
        this.current_power = current_power;
        this.manager_ref = manager_ref;
        this.map = map;
    }

    public void TryNewWeather(int new_weather, int new_power)
    {
        if (new_weather < 0 || new_weather >= weather_list.Length)
            return;

        if(new_power < current_power)
        {
            current_power -= new_power / 2;
        }
        else
        {
            current_weather = new_weather;
            current_power = new_power;
        }
    }
    public string GetName()
    {
        return weather_list[current_weather].GetName();
    }

    public GameObject GetModel()
    {
        return weather_list[current_weather].GetModel();
    }

    public Trait GetWatherCondition()
    {
        return weather_list[current_weather].GetWeatherCondition();
    }

    public int GetDamageMulti(Element element)
    {
        return weather_list[current_weather].GetDamageMulti(element);
    }
}