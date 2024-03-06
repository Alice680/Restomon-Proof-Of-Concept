using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MenuAtelier : MenuSwapIcon
{
    private PermDataHolder data_holder;

    public void Startup(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;
    }

    public override void Activate()
    {
        base.Activate();
    }

    public bool Change(Inputer inputer)
    {
        return true;
    }
}