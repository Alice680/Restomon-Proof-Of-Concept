using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldManager : MonoBehaviour
{
    private enum State { idle, enter_dungeon };

    private OverworldEntity player_entity;

    private Inputer inputer;
    private OverworldMap map;

    private GameObject cam;
    private OverworldUI overworld_ui;
    private PermDataHolder data_holder;

    private State current_state;

    private void Start()
    {
        inputer = new Inputer();

        cam = GameObject.Find("Main Camera");

        data_holder = GameObject.Find("DataHolder").GetComponent<PermDataHolder>();

        overworld_ui = GameObject.Find("Overworld UI").GetComponent<OverworldUI>();

        player_entity = new OverworldEntity(data_holder.GetPlayer().GetModel());
        map = data_holder.GetOverworld(out Vector2Int position).GetMap();
        map.Move(player_entity, (Vector3Int)position);

        UpdateCamera();
        overworld_ui.Startup();
    }

    private void Update()
    {
        inputer.Run();

        switch (current_state)
        {
            case State.idle:
                Idle();
                return;
            case State.enter_dungeon:
                EnterDungeon();
                return;
        }
    }

    private void Idle()
    {
        if (inputer.GetBack())
            Debug.Log("Menu");
        else if (inputer.GetEnter())
            Interact();
        else if (inputer.GetDir() != Direction.None)
            MovePlayer(inputer.GetDir());
    }

    private void EnterDungeon()
    {
        if (overworld_ui.ChangeChoice(inputer, out bool deactive))
            SceneManager.LoadScene(2);
        else if (deactive)
            current_state = State.idle;
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
        int dungeon_layout;

        map.Interact(player_entity.GetPosition().x, player_entity.GetPosition().y, out dungeon_layout);

        if (dungeon_layout != -1)
        {
            data_holder.SetDungeon(dungeon_layout);

            overworld_ui.ActivateChoice("Enter", data_holder.GetDungeon() + "?");

            current_state = State.enter_dungeon;
        }
    }

    private void MovePlayer(Direction dir)
    {
        map.Move(player_entity, player_entity.GetPosition() + DirectionMath.GetVectorChange(dir));

        UpdateCamera();
    }
}