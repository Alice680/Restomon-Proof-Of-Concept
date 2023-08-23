using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A basic menu type, it displays it elements by turning a list of game objects on and off.
 * Notes:
 */
[Serializable]
public class MenuSwapIcon : MenuBasic
{
    [SerializeField] protected GameObject[] icons;

    public override void Activate()
    {
        base.Activate();

        UpdateMenu(Direction.None);
    }

    public override void UpdateMenu(Direction dir)
    {
        base.UpdateMenu(dir);

        for (int i = 0; i < icons.Length; ++i)
            icons[i].SetActive(false);

        icons[x_value + (y_value * x_size)].SetActive(true);
    }

    public override void DeActivate()
    {
        base.DeActivate();
    }

    public override void GetValues(out int x, out int y)
    {
        menu.SetActive(true);
        base.GetValues(out x, out y);
    }
}