using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour
{
    public GameObject[] action_marker;
    public Text move_marker;

    public Text hp_text, ap_text;

    public GameObject cam;

    private DungeonMap current_map;

    public void UpdateActions(int moves, int actions)
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

        vec.x = (int)Mathf.Clamp(vec.x, 4, limites.x - 5);
        vec.y = (int)Mathf.Clamp(vec.y, 4, limites.y - 5);
        vec.z = -10;

        cam.transform.position = vec;
    }

    public void UpdatePlayerStats(Unit player, List<Unit> player_units)
    {
        hp_text.text = player.GetHp() + "/" + player.GetMaxHP();
        ap_text.text = 0 + "/" + 0;
    }

    public void Reset(DungeonMap map)
    {
        current_map = map;
    }
}