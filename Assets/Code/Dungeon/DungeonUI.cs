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
    public GameObject cam;

    public GameObject[] move_normal;
    public GameObject[] move_end;
    public GameObject[] action_marker;
    public GameObject[] unit_icons;

    public Text player_name_text, ap_text;
    public Text[] name_text, hp_text, mp_text;

    public GameObject condition_icon;
    public GameObject condition_mp_stat;
    public GameObject[] condition_transforms;

    private GameObject condition_model;
    private GameObject[] condition_icons;

    public Text condition_name;
    public Text condition_cost;
    public Text[] condition_text;

    public StatusConditions condition_list;

    private DungeonMap current_map;

    public void UpdateActions(int moves, int actions)
    {
        for (int i = 0; i < 10; ++i)
        {
            if (moves > i + 1)
            {
                move_normal[i].SetActive(true);
                move_end[i].SetActive(false);
            }
            else if (moves == i + 1)
            {
                move_normal[i].SetActive(false);
                if (i == 0)
                    move_normal[i].SetActive(true);
                move_end[i].SetActive(true);
            }
            else
            {
                move_normal[i].SetActive(false);
                move_end[i].SetActive(false);
            }
        }

        for (int i = 0; i < 4; ++i)
        {
            if (actions > i)
                action_marker[i].SetActive(true);
            else
                action_marker[i].SetActive(false);
        }
    }

    public void UpdateCam(Vector3Int vec)
    {
        vec += new Vector3Int(1, 0, 0);

        Vector3 limites = current_map.GetSize();

        vec.x = (int)Mathf.Clamp(vec.x, 5, limites.x - 4);
        vec.y = (int)Mathf.Clamp(vec.y, 4, limites.y - 5);
        vec.z = -10;

        cam.transform.position = vec + new Vector3(0.5f, 0, 0);
    }

    public void UpdatePlayerStats(Unit player, List<Unit> player_units)
    {
        ap_text.text = player.GetHp() + "/" + player.GetMaxHP();
        player_name_text.text = "Player";

        for (int i = 1; i < 9; ++i)
        {
            if (player_units.Count > i)
            {
                name_text[i - 1].text = player_units[i].ToString();
                hp_text[i - 1].text = player_units[i].GetHp() + "/" + player_units[i].GetMaxHP();
                mp_text[i - 1].text = player_units[i].GetMp() + "/" + player_units[i].GetMaxMP();
                unit_icons[i - 1].SetActive(true);
            }
            else
            {
                name_text[i - 1].text = "";
                hp_text[i - 1].text = "";
                mp_text[i - 1].text = "";
                unit_icons[i - 1].SetActive(false);
            }
        }
    }

    public void UpdateUnitStatus(Unit target , int show_cost)
    {
        if (condition_model != null)
            Destroy(condition_model);

        int index = -1, rank = -1;

        condition_icon.SetActive(false);
        condition_name.text = "";
        condition_cost.text = "";

        for (int i = 0; i < 10; ++i)
            if (condition_icons[i] != null)
                Destroy(condition_icons[i]);

        if (target == null)
            return;

        condition_icon.SetActive(true);

        if(show_cost >= 0)
            condition_cost.text = "Cost: " + show_cost;

        condition_model = target.GetModel();
        condition_model.transform.parent = cam.transform;
        condition_model.transform.localPosition = new Vector3(5.4f ,-0.8f,10);
        condition_model.GetComponent<Renderer>().sortingLayerName = "UI";
        condition_model.GetComponent<Renderer>().sortingOrder = 15;

        if (target.GetCreatureType() == CreatureType.Restomon)
        {
            condition_mp_stat.SetActive(true);
            condition_text[1].text = "" + target.GetMaxMP();
        }
        else
        {
            condition_mp_stat.SetActive(false);
            condition_text[1].text = "";
        }

        condition_text[0].text = "" + target.GetMaxHP();

        for (int i = 0; i < 9; ++i)
            condition_text[i+2].text = "" + target.GetStat(i);

        condition_name.text = target.ToString();
        for (int i = 0; i < target.GetNumConditions(); ++i)
        {
            index = target.GetCondition(i, out rank);
            condition_icons[i] = Instantiate(condition_list.GetModel(index, rank));
            condition_icons[i].transform.parent = cam.transform;
            condition_icons[i].transform.localPosition = new Vector3(3.25f + (0.5f * (i % 4)), 1.25f - (0.5f * (i / 4)), 10f);
        }
    }

    public void Reset(DungeonMap map)
    {
        current_map = map;

        condition_icons = new GameObject[10];
        for (int i = 0; i < 10; ++i)
            if (condition_icons[i] != null)
                Destroy(condition_icons[i]);
    }
}