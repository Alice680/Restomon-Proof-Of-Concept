using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MenuBasic
{
    [SerializeField] protected GameObject menu;

    [SerializeField] protected int x_value, y_value, x_size, y_size;

    public virtual void Activate()
    {
        menu.SetActive(true);
    }

    public virtual void UpdateMenu(Direction dir)
    {
        GetInputValue(x_value, y_value, x_size, y_size, dir, out x_value, out y_value);
    }

    public virtual void DeActivate()
    {
        menu.SetActive(false);
    }

    public virtual void GetValues(out int x, out int y)
    {
        x = x_value;
        y = y_value;
    }

    protected void GetInputValue(int x, int y, int x_max, int y_max, Direction dir, out int new_x, out int new_y)
    {
        new_x = x;
        new_y = y;

        if (dir == Direction.Down)
        {
            ++new_y;
            if (new_y == y_max)
                new_y = 0;
        }
        else if (dir == Direction.Up)
        {
            --new_y;
            if (new_y == -1)
                new_y = y_max - 1;
        }
        else if (dir == Direction.Right)
        {
            ++new_x;
            if (new_x == x_max)
                new_x = 0;
        }
        else if (dir == Direction.Left)
        {
            --new_x;
            if (new_x == -1)
                new_x = x_max - 1;
        }
    }
}