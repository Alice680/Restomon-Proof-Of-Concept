using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour
{
    public GameObject[] action_marker;
    public Text move_marker;

    public GameObject cam;

    private DungeonMap current_map;

    public void Reset(DungeonMap map)
    {
        current_map = map;
    }

    public void UpdateUI(int moves, int actions)
    {
        move_marker.text = "" + moves;

        for (int i = 0; i < action_marker.Length; ++i)
        {
            if (actions > i)
                action_marker[i].SetActive(true);
            else
                action_marker[i].SetActive(false);
        }
    }

    public void UpdateCam(Vector3Int vec)
    {
        Vector3 limites = current_map.GetSize();

        vec.x = (int)Mathf.Clamp(vec.x, 8, limites.x - 9);
        vec.y = (int)Mathf.Clamp(vec.y, 8, limites.y - 9);
        vec.z = -10;

        cam.transform.position = vec;
    }
}