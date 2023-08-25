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
    public Text[] status_text_row;
    public GameObject[] status_icon_row;

    public GameObject cam;

    public StatusConditions condition_list;

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

        int[] temp_b;
        int[] temp = player.GetAilments(out temp_b);

        for (int i = 0; i < 4; ++i)
        {
            status_text_row[i].text = "";
            Destroy(status_icon_row[i]);
        }

        for (int i = 0; i < temp.Length; ++i)
        {
            status_text_row[i].text = "" + temp_b[i];
            status_icon_row[i] = Instantiate(condition_list.GetAilmentModel(temp[i]));
            status_icon_row[i].transform.parent = cam.transform;
            status_icon_row[i].transform.localPosition = new Vector3(2.45f + (0.4f*i), 3.75f, 10);
        }

    }

    public void Reset(DungeonMap map)
    {
        current_map = map;
    }
}