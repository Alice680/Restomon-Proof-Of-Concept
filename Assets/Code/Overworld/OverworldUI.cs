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

    private DialogueTree dialogue_tree;
    private bool making_choice;
    private int current_node;

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

    public void ActivateDialogue(DialogueTree dialogue_tree, int start_node)
    {
        this.dialogue_tree = dialogue_tree;
        current_node = start_node;

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

    public bool ChangeDialogue(Inputer inputer)
    {
        if (inputer.GetEnter())
        {
            bool is_choice;

            if (making_choice)
                current_node = dialogue_tree.NextNode(current_node, ChangeChoice(inputer, out bool deactive), out is_choice);
            else
                current_node = dialogue_tree.NextNode(current_node, true, out is_choice);

            if (current_node == -1)
            {
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
}