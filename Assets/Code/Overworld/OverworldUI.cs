using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverworldUI : MonoBehaviour
{
    [SerializeField] private GameObject text_box, choice_box, town_box;

    [SerializeField] private Text dialogue_name, dialogue, choice;

    [SerializeField] private Text[] town_options;

    private PermDataHolder data_holder;

    private bool current_choice;
    private string text_choice;

    private DialogueTree dialogue_tree;
    private bool making_choice;
    private int current_node;

    private OverworldTown current_town;
    private int current_selection;
    private int current_area;

    public void Startup(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;

        text_box.SetActive(false);
        choice_box.SetActive(false);
        town_box.SetActive(false);

        dialogue_name.text = "";
        dialogue.text = "";
        choice.text = "";

        for (int i = 0; i < 8; ++i)
            town_options[i].text = "";
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
        else if ((inputer.GetDir() == Direction.Up || inputer.GetDir() == Direction.Down) && inputer.GetMoveKeyUp())
        {
            current_choice = !current_choice;
            choice.text = text_choice + "Yes" + (current_choice ? "*" : "") + "\nNo" + (current_choice ? "" : "*");
            return false;
        }

        return false;
    }

    public void ActivateDialogue(DialogueTree dialogue_tree)
    {
        this.dialogue_tree = dialogue_tree;
        current_node = 0;

        text_box.SetActive(true);

        dialogue.text = dialogue_tree.GetData(current_node, out string speaker_name, out string choice_name);
        dialogue_name.text = speaker_name;
    }

    public void DeactivateDialogue()
    {
        DeactivateChoice();

        dialogue_tree = null;
        current_node = -1;

        text_box.SetActive(false);

        dialogue.text = "";
        dialogue_name.text = "";
    }

    public bool ChangeDialogue(Inputer inputer, PermDataHolder data_holder)
    {
        if (inputer.GetEnter())
        {
            bool is_choice;

            if (making_choice)
                current_node = dialogue_tree.NextNode(current_node, ChangeChoice(inputer, out bool deactive), out is_choice);
            else
                current_node = dialogue_tree.NextNode(current_node, true, out is_choice);

            if (current_node < 0)
            {
                dialogue_tree.RunEvent(data_holder, current_node);
                DeactivateDialogue();
                return true;
            }

            dialogue.text = dialogue_tree.GetData(current_node, out string speaker_name, out string choice_name);
            dialogue_name.text = speaker_name;

            if (is_choice)
            {
                ActivateChoice("", choice_name);
                making_choice = true;
            }
            else
            {
                DeactivateChoice();
                making_choice = false;
            }

            return false;
        }
        else if (!inputer.GetBack() && making_choice)
        {
            ChangeChoice(inputer, out bool deactive);
        }

        return false;
    }

    public void ActivateTown(OverworldTown town)
    {
        text_box.SetActive(true);
        town_box.SetActive(true);

        current_area = 0;

        current_town = town;

        ChangeTown(null);
    }

    public void ChangeTown(Inputer inputer)
    {
        if (inputer != null)
        {
            if (inputer.GetDir() != Direction.None && inputer.GetMoveKeyUp())
                current_selection = DirectionMath.GetMenuChange(current_selection, inputer.GetDir(), 4, 2);

            if(inputer.GetEnter())
            {
                int temp_selection = current_town.GetSelection(current_area, current_selection);
            }
        }

        dialogue.text = current_town.GetBodyText(current_area, data_holder, out string area_name);
        dialogue_name.text = area_name;

        for (int i = 0; i < 8; ++i)
            town_options[i].text = (current_selection == i ? "*" : "") + (current_town.GetChoiceText(current_area, data_holder)[i]);
    }
}