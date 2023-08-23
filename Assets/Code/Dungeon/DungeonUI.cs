using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Handles persitant UI in the dungeon.
 * 
 * Notes:
 * Shows hp, mp, and sp of all player controlled characters.
 * Handles the camera.
 * Shows the current amount of actions and movement if the active actor is the player
 */
// TODO move tile markers into this class. Maybe?
public class DungeonUI : MonoBehaviour
{
    public GameObject[] action_marker;
    public Text move_marker;

    public Text ap_text;
    public Text[] hp_text, sp_text, mp_text;
    public Text[] text_row_a, text_row_b, text_row_c, text_row_d;
    public GameObject[] icon_row_a, icon_row_b, icon_row_c, icon_row_d;

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
        ap_text.text = player.GetHp() + "/" + player.GetMaxHP();
        for (int i = 0; i < 4; ++i)
            text_row_a[i].text = "";
    }

    public void Reset(DungeonMap map)
    {
        current_map = map;
    }
}