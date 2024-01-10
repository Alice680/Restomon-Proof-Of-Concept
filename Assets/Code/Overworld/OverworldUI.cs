using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverworldUI : MonoBehaviour
{
    [SerializeField] private GameObject text_box;
    [SerializeField] private GameObject choice_box;

    [SerializeField] private Text dialogue_name;
    [SerializeField] private Text dialogue;
    [SerializeField] private Text choice;

    private bool current_choice;
    private string text_choice;

    public void Startup()
    {
        text_box.SetActive(false);
        choice_box.SetActive(false);

        dialogue_name.text = "";
        dialogue.text = "";
        choice.text = "";
    }

    public void ActivateChoice(string name_a, string name_b)
    {
        choice_box.SetActive(true);

        current_choice = true;

        text_choice = name_a + "\n" + name_b + "\n";

        choice.text = text_choice + "Yes" + (current_choice ? "*" : "") + "\nNo" + (current_choice ? "" : "*");
    }

    public void DeactivateChoice()
    {
        choice_box.SetActive(false);
        choice.text = "";
    }

    public bool ChangeChoice(Inputer inputer, out bool deactive)
    {
        deactive = false;
        if (inputer.GetEnter())
        {
            if (current_choice == true)
            {
                DeactivateChoice();
                deactive = true;
                return true;
            }
            else
            {
                DeactivateChoice();
                deactive = true;
                return false;
            }
        }
        else if (inputer.GetBack())
        {
            DeactivateChoice();
            deactive = true;
            return false;
        }
        else if (inputer.GetDir() == Direction.Up || inputer.GetDir() == Direction.Down)
        {
            current_choice = !current_choice;
            choice.text = text_choice + "Yes" + (current_choice ? "*" : "") + "\nNo" + (current_choice ? "" : "*");
            return false;
        }

        return false;
    }

}