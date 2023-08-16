using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private enum State { core, dungeon, armory, team, achievements, options }

    private PermDataHolder data_holder;

    [SerializeField] MenuSwapIcon core_menu;
    [SerializeField] MenuDungeon dungeon_menu;
    [SerializeField] MenuArmory armory_menu;
    [SerializeField] MenuBasic team_menu;
    [SerializeField] MenuBasic achievements_menu;
    [SerializeField] MenuBasic options_menu;

    private Inputer inputer;

    private State state;

    [SerializeField] private DungeonLayout[] dungeon_layouts;
    [SerializeField] private HumanClass[] human_classes;
    [SerializeField] private RestomonBase[] restomon_types;

    private void Start()
    {
        data_holder = GameObject.Find("DataHolder").GetComponent<PermDataHolder>();

        inputer = new Inputer();
        state = State.core;

        core_menu.Activate();
    }

    private void Update()
    {
        inputer.Run();

        switch (state)
        {
            case State.core:
                CoreMenu();
                break;
            case State.dungeon:
                DungeonMenu();
                break;
            case State.armory:
                ArmoryMenu();
                break;
            case State.team:
                TeamMenu();
                break;
            case State.achievements:
                AchievementsMenu();
                break;
            case State.options:
                OptionsMenu();
                break;
        }
    }

    //States
    private void CoreMenu()
    {
        if (inputer.GetDir() != Direction.None)
        {
            core_menu.UpdateMenu(inputer.GetDir());
        }
        else if (inputer.GetEnter())
        {
            int x, y;

            core_menu.GetValues(out x, out y);

            core_menu.DeActivate();

            switch (y)
            {
                case 0:
                    dungeon_menu.Activate();
                    state = State.dungeon;
                    break;
                case 1:
                    armory_menu.Activate();
                    state = State.armory;
                    break;
                case 2:
                    team_menu.Activate();
                    state = State.team;
                    break;
                case 3:
                    achievements_menu.Activate();
                    state = State.achievements;
                    break;
                case 4:
                    options_menu.Activate();
                    state = State.options;
                    break;
            }
        }
        else if (inputer.GetBack())
        {

        }
    }

    private void DungeonMenu()
    {
        if (inputer.GetDir() != Direction.None)
        {
            dungeon_menu.UpdateMenu(inputer.GetDir());
        }
        else if (inputer.GetEnter())
        {
            int x, y;

            dungeon_menu.GetValues(out x, out y);

            if (y == 2)
            {
                UpdateData();
                SceneManager.LoadScene(2);
            }
        }
        else if (inputer.GetBack())
        {
            dungeon_menu.DeActivate();
            core_menu.Activate();
            state = State.core;
        }
    }

    private void ArmoryMenu()
    {
        if (inputer.GetDir() != Direction.None)
        {
            armory_menu.UpdateMenu(inputer.GetDir());
        }
        else if (inputer.GetEnter())
        {

        }
        else if (inputer.GetBack())
        {
            armory_menu.DeActivate();
            core_menu.Activate();
            state = State.core;
        }
    }

    private void TeamMenu()
    {
        if (inputer.GetDir() != Direction.None)
        {
        }
        else if (inputer.GetEnter())
        {

        }
        else if (inputer.GetBack())
        {
            team_menu.DeActivate();
            core_menu.Activate();
            state = State.core;
        }
    }

    private void AchievementsMenu()
    {
        if (inputer.GetDir() != Direction.None)
        {
        }
        else if (inputer.GetEnter())
        {

        }
        else if (inputer.GetBack())
        {
            achievements_menu.DeActivate();
            core_menu.Activate();
            state = State.core;
        }
    }

    private void OptionsMenu()
    {
        if (inputer.GetDir() != Direction.None)
        {
        }
        else if (inputer.GetEnter())
        {

        }
        else if (inputer.GetBack())
        {
            options_menu.DeActivate();
            core_menu.Activate();
            state = State.core;
        }
    }

    //Internal calls
    private void UpdateData()
    {
        dungeon_menu.SetData(data_holder);
        armory_menu.SetData(data_holder);
    }
}