using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private enum State { core, dungeon, armory, team, achievements, options }

    private PermDataHolder data_holder;

    [SerializeField] MenuMovingIcon core_menu;
    [SerializeField] MenuSwapIcon dungeon_menu;
    [SerializeField] MenuBasic armory_menu;
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

        //Temp
        data_holder.SetPlayer(human_classes[2].GetHuman(1, 0, 0, 0, 0, 0, new int[0], new int[0]));
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
                case 4:
                    dungeon_menu.Activate();
                    state = State.dungeon;
                    break;
                case 3:
                    armory_menu.Activate();
                    state = State.armory;
                    break;
                case 2:
                    team_menu.Activate();
                    state = State.team;
                    break;
                case 1:
                    achievements_menu.Activate();
                    state = State.achievements;
                    break;
                case 0:
                    options_menu.Activate();
                    state = State.options;
                    break;
            }
        }
        else if (inputer.GetBack())
        {

        }
    }

    public void DungeonMenu()
    {
        if (inputer.GetDir() != Direction.None)
        {
            dungeon_menu.UpdateMenu(inputer.GetDir());
        }
        else if (inputer.GetEnter())
        {
            int x, y;

            dungeon_menu.GetValues(out x, out y);

            //Enter Dungeon
            if (y == 0)
            {
                data_holder.SetDungeon(dungeon_layouts[1]);
                SceneManager.LoadScene(2);
            }
            else if (y == 3)
            {
                data_holder.SetDungeon(dungeon_layouts[0]);
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

    public void ArmoryMenu()
    {
        if (inputer.GetDir() != Direction.None)
        {
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

    public void TeamMenu()
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

    public void AchievementsMenu()
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

    public void OptionsMenu()
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
}