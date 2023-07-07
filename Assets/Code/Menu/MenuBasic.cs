using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MenuBasic
{
    [SerializeField] protected GameObject menu;


    [SerializeField] protected int x_value, y_value, x_size, y_size;

    public virtual void UpdateMenu(Direction dir)
    {
        if (dir == Direction.Up)
        {
            ++y_value;
            if (y_value == y_size)
                y_value = 0;
        }
        else if (dir == Direction.Down)
        {
            --y_value;
            if (y_value == -1)
                y_value = y_size - 1;
        }
        else if (dir == Direction.Right)
        {
            ++x_value;
            if (x_value == x_size)
                x_value = 0;
        }
        else if (dir == Direction.Left)
        {
            --x_value;
            if (x_value == -1)
                x_value = x_size - 1;
        }
    }

    public virtual void Activate()
    {
        menu.SetActive(true);
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
}