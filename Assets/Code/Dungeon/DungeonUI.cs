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

    public Text condition_name;
    private GameObject[] condition_icons;

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

        for (int i = 0; i < 4; ++i)
        {
            status_text_row[i].text = "";
        }
    }

    public void UpdateUnitStatus(Unit target)
    {
        int index = -1, rank = -1;

        for (int i = 0; i < 12; ++i)
            if (condition_icons[i] != null)
                Destroy(condition_icons[i]);

        condition_name.text = "";

        if (target == null)
            return;

        for (int i = 0; i < target.GetNumConditions(); ++i)
        {
            index = target.GetCondition(i, out rank);

            condition_icons[i] = Instantiate(condition_list.GetModel(index, rank));
            condition_icons[i].transform.parent = cam.transform;
            condition_icons[i].transform.localPosition = new Vector3(2.25f + (0.5f * (i % 4)), 0.25f - (0.5f * (i / 4)), 10f);
        }

        // TODO add get name
        condition_name.text = "Name";
    }

    public void Reset(DungeonMap map)
    {
        current_map = map;

        condition_icons = new GameObject[12];
    }
}