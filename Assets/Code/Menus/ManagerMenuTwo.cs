using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagerMenuTwo : MonoBehaviour
{
    private enum State { Core, New, Load, Option }

    [SerializeField] private MenuSwapIcon core_menu, file_menu;
    [SerializeField] private Text[] file_text;

    private State current_sate;

    private PermDataHolder data_holder;
    private Inputer inputer;

    private void Start()
    {
        data_holder = GameObject.Find("DataHolder").GetComponent<PermDataHolder>();

        core_menu.Activate();
        core_menu.UpdateMenu(Direction.Down);

        inputer = new Inputer();

        current_sate = State.Core;
    }

    private void Update()
    {
        inputer.Run();

        switch (current_sate)
        {
            case State.Core:
                Core();
                break;
            case State.New:
            case State.Load:
                Slots();
                break;
            case State.Option:

                break;
        }
    }

    private void Core()
    {
        if (inputer.GetDir() != Direction.None && inputer.GetMoveKeyUp())
            core_menu.UpdateMenu(inputer.GetDir());

        if (inputer.GetEnter())
        {
            core_menu.GetValues(out int x, out int y);
            switch (y)
            {
                case 0:
                case 1:
                    core_menu.DeActivate();
                    file_menu.Activate();

                    string[] temp_text = data_holder.GetFiles(out bool[] has_save);

                    for (int i = 0; i < 3; ++i)
                    {
                        if (has_save[i])
                        {
                            file_text[(i * 3) + 0].text = temp_text[(i * 3) + 0];
                            file_text[(i * 3) + 1].text = temp_text[(i * 3) + 1];
                            file_text[(i * 3) + 2].text = temp_text[(i * 3) + 2];
                        }
                        else
                        {
                            file_text[(i * 3) + 0].text = "";
                            file_text[(i * 3) + 1].text = "";
                            file_text[(i * 3) + 2].text = "";
                        }
                    }

                    if (y == 0)
                        current_sate = State.New;
                    else
                        current_sate = State.Load;

                    break;
                case 2:
                    Debug.Log("Add next update");
                    break;
                case 3:
                    Steamworks.SteamClient.Shutdown();
                    Application.Quit();
                    Debug.Log("Close");
                    break;
            }
        }
    }

    private void Slots()
    {
        if (inputer.GetDir() != Direction.None && inputer.GetMoveKeyUp())
        {
            file_menu.UpdateMenu(inputer.GetDir());
        }
        else if (inputer.GetEnter())
        {
            file_menu.GetValues(out int x, out int y);
            if (current_sate == State.New)
            {
                data_holder.SetSaveFile(y);
                data_holder.CreateSaveFile(y, "Player");
                SceneManager.LoadScene(3);
            }
            else
            {
                string[] temp_text = data_holder.GetFiles(out bool[] has_save);
                if (has_save[y])
                {
                    data_holder.SetSaveFile(y);
                    data_holder.LoadData();
                    SceneManager.LoadScene(3);
                }
            }
        }
        else if (inputer.GetBack())
        {
            file_menu.DeActivate();
            core_menu.Activate();

            current_sate = State.Core;
        }
    }
}