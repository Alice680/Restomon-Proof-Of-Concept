using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MenuDungeon : MenuSwapIcon
{
    [SerializeField] private Text dungeon_text_box, difficulty_text_box;
    [SerializeField] private String[] dungeon_text, difficulty_text;
    [SerializeField] private int dungeon_int, difficulty_int;

    public override void Activate()
    {
        base.Activate();

        dungeon_text_box.text = dungeon_text[dungeon_int];
        difficulty_text_box.text = difficulty_text[difficulty_int];
    }

    public override void UpdateMenu(Direction dir)
    {
        if (dir == Direction.Up || dir == Direction.Down)
        {
            base.UpdateMenu(dir);
            return;
        }

        if(y_value == 0)
        {
            GetInputValue(dungeon_int, 0, 6, 1, dir, out dungeon_int, out int dead_variable);

            dungeon_text_box.text = dungeon_text[dungeon_int];
        }

        else if(y_value == 1)
        {
            GetInputValue(difficulty_int, 0, 6, 1, dir, out difficulty_int, out int dead_variable);

            difficulty_text_box.text = difficulty_text[difficulty_int];
        }
    }

    public void SetData(PermDataHolder data)
    {
        data.SetDungeon(dungeon_int);
    }
}