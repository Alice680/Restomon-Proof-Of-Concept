using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;

[Serializable]
public class MenuOverworld : MenuSwapIcon
{
    private enum State { Core, Stats, Inventory, Quit };

    [SerializeField] private Text[] core_text, stats_text, item_text;
    [SerializeField] private Text dialogue, page_num;
    [SerializeField] private GameObject[] stats_positions, item_marker;
    [SerializeField] private GameObject core_menu, item_menu;

    private string[] item_names, item_descriptions;

    private PermDataHolder data_holder;

    private GameObject cam;

    private GameObject[] stats_icons;

    private State current_state;

    public void SetData(PermDataHolder data_holder, GameObject cam)
    {
        this.data_holder = data_holder;
        this.cam = cam;

        stats_icons = new GameObject[9];
    }

    public override void Activate()
    {
        x_value = 0;
        y_value = 0;

        menu.SetActive(true);

        OpenCore();
    }

    public override void DeActivate()
    {
        x_value = 0;
        y_value = 0;

        CloseMenus();

        menu.SetActive(false);
    }

    public bool Run(Inputer inputer)
    {
        switch (current_state)
        {
            case State.Core:
                return Core(inputer);
            case State.Stats:
                Stats(inputer);
                break;
            case State.Inventory:
                Inventory(inputer);
                break;
            case State.Quit:
                Quit(inputer);
                break;
        }

        return false;
    }

    public void CloseMenus()
    {
        core_menu.SetActive(false);
        item_menu.SetActive(false);

        dialogue.text = "";
        page_num.text = "";

        for (int i = 0; i < 3; i++)
        {
            core_text[i].text = "";
            icons[i].SetActive(false);
        }

        for (int i = 0; i < 9; i++)
        {
            stats_text[i].text = "";

            if (stats_icons[i] != null)
                GameObject.Destroy(stats_icons[i]);
        }

        for (int i = 0; i < 8; i++)
        {
            item_text[i].text = "";
            item_marker[i].SetActive(false);
        }
    }

    public void OpenCore()
    {
        core_menu.SetActive(true);

        core_text[0].text = "Stats";
        core_text[1].text = "Inventory";
        core_text[2].text = "Quit";

        for (int i = 0; i < 3; i++)
            icons[i].SetActive(false);

        icons[y_value].SetActive(true);
    }

    public void OpenStats()
    {
        stats_text[0].text = data_holder.GetPlayer().ToString() + "   " + data_holder.GetCorruption() + "/100";

        stats_icons[0] = data_holder.GetPlayer().GetModel();
        stats_icons[0].transform.parent = cam.transform;
        stats_icons[0].transform.localPosition = stats_positions[0].transform.localPosition;
        stats_icons[0].transform.localPosition -= new Vector3Int(0, 0, -10);
        stats_icons[0].GetComponent<Renderer>().sortingLayerName = "UI";
        stats_icons[0].GetComponent<Renderer>().sortingOrder = 2;

        for (int i = 0; i < 8; i++)
        {
            if (data_holder.GetTeam(i) != null)
            {
                stats_text[i + 1].text = data_holder.GetTeam(i).GetName() + "   " + data_holder.GetCoreDamage(i) + "/10";

                stats_icons[i + 1] = data_holder.GetTeam(i).GetModel(RestomonEvolution.None);
                stats_icons[i + 1].transform.parent = cam.transform;
                stats_icons[i + 1].transform.localPosition = stats_positions[i + 1].transform.localPosition;
                stats_icons[i + 1].transform.localPosition -= new Vector3Int(0, 0, -10);
                stats_icons[i + 1].GetComponent<Renderer>().sortingLayerName = "UI";
                stats_icons[i + 1].GetComponent<Renderer>().sortingOrder = 2;
            }
        }
    }

    public void OpenInventory()
    {
        item_menu.SetActive(true);

        item_names = new string[data_holder.GetInventoryCount()];
        item_descriptions = new string[data_holder.GetInventoryCount()];

        for (int i = 0; i < 8; i++)
        {
            item_marker[i].SetActive(false);

            if (i + (x_value * 8) < item_names.Length)
                item_text[i].text = item_names[i + (x_value * 8)];
            else
                item_text[i].text = "";
        }

        page_num.text = (x_value + 1) + "/" + (item_names.Length + 1);

        if (y_value + (x_value * 8) < item_names.Length)
            dialogue.text = item_descriptions[y_value + (x_value * 8)];
        else
            dialogue.text = "";
    }

    public void OpenQuit()
    {
        core_menu.SetActive(true);

        core_text[0].text = "Quit Game";
        core_text[1].text = "Main Menu";
        core_text[2].text = "Resume";

        for (int i = 0; i < 3; i++)
            icons[i].SetActive(false);

        icons[y_value].SetActive(true);
    }

    private bool Core(Inputer inputer)
    {
        if (inputer.GetDir() != Direction.None && inputer.GetMoveKeyUp())
        {
            GetInputValue(x_value, y_value, 1, 3, inputer.GetDir(), out x_value, out y_value);

            for (int i = 0; i < 3; i++)
                icons[i].SetActive(false);

            icons[y_value].SetActive(true);
        }
        else if (inputer.GetEnter())
        {
            CloseMenus();

            switch (y_value)
            {
                case 0:
                    OpenStats();
                    current_state = State.Stats;
                    break;
                case 1:
                    y_value = 0;
                    OpenInventory();
                    current_state = State.Inventory;
                    break;
                case 2:
                    OpenQuit();
                    current_state = State.Quit;
                    break;
            }

            return false;
        }
        else if (inputer.GetBack())
        {
            return true;
        }

        return false;
    }

    private void Stats(Inputer inputer)
    {
        if (inputer.GetDir() != Direction.None && inputer.GetMoveKeyUp())
        {

        }
        else if (inputer.GetEnter())
        {

        }
        else if (inputer.GetBack())
        {
            y_value = 0;

            CloseMenus();
            OpenCore();
            current_state = State.Core;
        }
    }

    private void Inventory(Inputer inputer)
    {
        if (inputer.GetDir() != Direction.None && inputer.GetMoveKeyUp())
        {
            GetInputValue(x_value, y_value, item_names.Length / 8 + 1, 8, inputer.GetDir(), out x_value, out y_value);

            for (int i = 0; i < 8; i++)
            {
                item_marker[i].SetActive(false);

                if (i + (x_value * 8) < item_names.Length)
                    item_text[i].text = item_names[i + (x_value * 8)];
                else
                    item_text[i].text = "";
            }

            page_num.text = (x_value + 1) + "/" + (item_names.Length + 1);

            if (y_value + (x_value * 8) < item_names.Length)
                dialogue.text = item_descriptions[y_value + (x_value * 8)];
            else
                dialogue.text = "";
        }
        else if (inputer.GetEnter())
        {

        }
        else if (inputer.GetBack())
        {
            y_value = 1;

            CloseMenus();
            OpenCore();
            current_state = State.Core;
        }
    }

    private void Quit(Inputer inputer)
    {
        if (inputer.GetDir() != Direction.None && inputer.GetMoveKeyUp())
        {
            GetInputValue(x_value, y_value, 1, 3, inputer.GetDir(), out x_value, out y_value);

            for (int i = 0; i < 3; i++)
                icons[i].SetActive(false);

            icons[y_value].SetActive(true);
        }
        else if (inputer.GetEnter())
        {
            switch (y_value)
            {
                case 0:
                    Steamworks.SteamClient.Shutdown();
                    Application.Quit();
                    Debug.Log("Close");
                    break;
                case 1:
                    Debug.Log("Main Menu");
                    break;
                case 2:
                    CloseMenus();
                    OpenCore();
                    current_state = State.Core;
                    break;
            }
        }
        else if (inputer.GetBack())
        {
            y_value = 2;

            CloseMenus();
            OpenCore();
            current_state = State.Core;
        }
    }
}