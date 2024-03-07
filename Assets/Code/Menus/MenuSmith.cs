using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MenuSmith : MenuSwapIcon
{
    private PermDataHolder data_holder;

    public void Startup(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;
    }

    public void ActivateEX()
    {
        base.Activate();
    }

    public bool Change(Inputer inputer)
    {
        return true;
    }
}