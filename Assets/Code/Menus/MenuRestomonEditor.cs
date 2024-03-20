using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MenuRestomonEditor : MenuSwapIcon
{
    private enum State { Initial, Core, Sub, Choice }

    [SerializeField] private Text name_text;
    [SerializeField] private Text[] core_text, sub_text, choice_text;
    [SerializeField] private GameObject core_menu, sub_menu, choice_menu;
    [SerializeField] private GameObject[] core_icons, sub_icons, choice_icons;

    private int restomon_index;

    private PermDataHolder data_holder;

    private State current_state;

    public void SetData(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;
        current_state = State.Initial;
    }

    public void Activate(int value)
    {
        base.Activate();

        CloseMenus();

        restomon_index = value;

        name_text.text = data_holder.GetRestomonData(value).GetName();

        current_state = State.Initial;
    }

    public override void DeActivate()
    {
        CloseMenus();

        base.DeActivate();
        name_text.text = "";
    }


    private void CloseMenus()
    {
        core_menu.SetActive(false);
        sub_menu.SetActive(false);

        for(int i = 0;i<17;++i)
        {
            core_text[i].text = "";
            core_icons[i].SetActive(false);
        }

        CloseChoice();
    }

    private void OpenCore()
    {
        core_menu.SetActive(true);
    }

    private void OpenSub()
    {

    }

    private void CloseChoice()
    {

    }

    private void OpenChoice()
    {

    }

    public bool Run(Inputer input)
    {
        if (input.GetBack() && current_state == State.Initial)
        {
            return true;
        }

        switch (current_state)
        {
            case State.Initial:
                Initial(input);
                break;
            case State.Core:
                Core(input);
                break;
            case State.Sub:
                Sub(input);
                break;
            case State.Choice:
                Choice(input);
                break;

        }

        return false;
    }

    private void Initial(Inputer input)
    {
        if (input.GetDir() != Direction.None)
        {
            base.UpdateMenu(input.GetDir());

            if (y_value == 0)
                OpenCore();
            else
                Debug.Log("Finish Menu");
        }
        else if (input.GetEnter())
        {

        }
        else if (input.GetBack())
        {

        }
    }

    private void Core(Inputer input)
    {
        if (input.GetDir() != Direction.None)
        {

        }
        else if (input.GetEnter())
        {

        }
        else if (input.GetBack())
        {

        }
    }

    private void Sub(Inputer input)
    {
        if (input.GetDir() != Direction.None)
        {

        }
        else if (input.GetEnter())
        {

        }
        else if (input.GetBack())
        {

        }
    }

    private void Choice(Inputer input)
    {
        if (input.GetDir() != Direction.None)
        {

        }
        else if (input.GetEnter())
        {

        }
        else if (input.GetBack())
        {

        }
    }
}