using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class OverworldManager : MonoBehaviour
{
    private enum State { idle, enter_dungeon, dialogue, town, menu };

    private OverworldEntity player_entity;

    private Inputer inputer;
    private OverworldMap map;

    private GameObject cam;
    private OverworldUI overworld_ui;
    private PermDataHolder data_holder;

    [SerializeField] private MenuOverworld menu_overworld;

    private State current_state;

    private void Start()
    {
        inputer = new Inputer();

        cam = GameObject.Find("Main Camera");

        data_holder = GameObject.Find("DataHolder").GetComponent<PermDataHolder>();

        overworld_ui = GameObject.Find("Overworld UI").GetComponent<OverworldUI>();

        OverworldLayout temp_layout = data_holder.GetOverworld(out Vector2Int position);

        player_entity = new OverworldEntity(data_holder.GetPlayer().GetModel());

        map = temp_layout.GetMap(data_holder, out DialogueTree dialogue);
        map.Move(player_entity, (Vector3Int)position);
        temp_layout.SetUIValues(overworld_ui);

        UpdateCamera();
        overworld_ui.Startup(data_holder);

        menu_overworld.SetData(data_holder, cam);

        if (dialogue != null)
        {
            overworld_ui.ActivateDialogue(dialogue);
            current_state = State.dialogue;
        }

        data_holder.SaveData();
    }

    private void Update()
    {
        Steamworks.SteamClient.RunCallbacks();

        inputer.Run();

        switch (current_state)
        {
            case State.idle:
                Idle();
                return;
            case State.enter_dungeon:
                EnterDungeon();
                return;
            case State.dialogue:
                Dialogue();
                break;
            case State.town:
                Town();
                break;
            case State.menu:
                Menu();
                break;
        }
    }

    private void Idle()
    {
        if (inputer.GetActionOne())
        {
            menu_overworld.Activate();
            current_state = State.menu;
        }
        else if (inputer.GetEnter())
            Interact();
        else if (inputer.GetDir() != Direction.None)
            MovePlayer(inputer.GetDir());
    }

    private void EnterDungeon()
    {
        if (overworld_ui.ChangeChoice(inputer, out bool deactive))
        {
            data_holder.SaveData();
            SceneManager.LoadScene(2);
        }
        else if (deactive)
            current_state = State.idle;
    }

    private void Dialogue()
    {
        if (overworld_ui.ChangeDialogue(inputer))
        {
            data_holder.SaveData();
            current_state = State.idle;
        }

    }

    private void Town()
    {
        if (overworld_ui.ChangeTown(inputer))
        {
            Vector3Int temp_pos = player_entity.GetPosition();

            player_entity.Remove();
            map.Clear();

            map = data_holder.GetOverworld(out Vector2Int position).GetMap(data_holder, out DialogueTree dialogue);
            player_entity = new OverworldEntity(data_holder.GetPlayer().GetModel());
            map.Move(player_entity, temp_pos);

            data_holder.SaveData();
            current_state = State.idle;
        }
    }

    private void Menu()
    {
        if (menu_overworld.Run(inputer, (Vector2Int)player_entity.GetPosition()))
        {
            menu_overworld.DeActivate();
            current_state = State.idle;
        }
    }

    private void UpdateCamera()
    {
        Vector3 temp_positon = new Vector3(0, 0, -10);

        map.GetSize(out int x_limit, out int y_limit);

        temp_positon.x = Mathf.Clamp(player_entity.GetPosition().x, 6, x_limit - 7);
        temp_positon.y = Mathf.Clamp(player_entity.GetPosition().y, 5, y_limit - 6);

        cam.transform.position = temp_positon;
    }

    private void Interact()
    {
        map.Interact(player_entity.GetPosition().x, player_entity.GetPosition().y, out int dungeon_layout, out OverworldTown town);

        if (dungeon_layout != -1)
        {
            data_holder.SetDungeon(dungeon_layout);

            overworld_ui.ActivateChoice("Enter", data_holder.GetDungeon() + "?");

            current_state = State.enter_dungeon;
        }
        else if (town != null)
        {
            overworld_ui.ActivateTown(town);

            current_state = State.town;
        }
    }

    private void MovePlayer(Direction dir)
    {
        map.Move(player_entity, player_entity.GetPosition() + DirectionMath.GetVectorChange(dir));

        UpdateCamera();
    }
}