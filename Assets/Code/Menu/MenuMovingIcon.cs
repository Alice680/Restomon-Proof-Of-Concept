using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MenuMovingIcon : MenuBasic
{
    [SerializeField] private GameObject selector;

    [SerializeField] private float x_scale, y_scale, x_offset, y_offset;

    public override void UpdateMenu(Direction dir)
    {
        base.UpdateMenu(dir);

        selector.transform.position = new Vector2(x_offset + (x_value * x_scale), y_offset + (y_value * y_scale));
    }

    public override void Activate()
    {
        base.Activate();
        selector.transform.position = new Vector2(x_offset + (x_value * x_scale), y_offset + (y_value * y_scale));
    }

    public override void DeActivate()
    {
        base.DeActivate();
        selector.transform.position = new Vector2(x_offset + (x_value * x_scale), y_offset + (y_value * y_scale));
    }

    public override void GetValues(out int x, out int y)
    {
        base.GetValues(out x, out y);
    }
}