using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonDialogue : MonoBehaviour
{
    [SerializeField] private GameObject text_box, choice_box;
    [SerializeField] private Text text_name, text_body, text_choice;

    private Inputer inputer;
    private DialogueTree dialogue_tree;
    private int current_node;
    private bool making_choice, current_choice;

    public void Activate(DialogueTree dialogue_tree)
    {
        this.dialogue_tree = dialogue_tree;
        current_node = 0;

        inputer = new Inputer();

        text_box.SetActive(true);

        text_body.text = dialogue_tree.GetData(current_node, out string speaker_name, out string choice_name);
        text_name.text = speaker_name;
    }

    public bool ChangeDialogue()
    {
        inputer.Run();

        if (inputer.GetEnter())
        {
            bool is_choice;

            if (making_choice)
                current_node = dialogue_tree.NextNode(current_node, ChangeChoice(inputer, out bool deactive), out is_choice);
            else
                current_node = dialogue_tree.NextNode(current_node, true, out is_choice);

            if (current_node < 0)
            {
                DeactivateDialogue();
                return true;
            }

            text_body.text = dialogue_tree.GetData(current_node, out string speaker_name, out string choice_name);
            text_name.text = speaker_name;

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
   
    private void DeactivateDialogue()
    {
        DeactivateChoice();

        dialogue_tree = null;
        current_node = -1;

        text_box.SetActive(false);

        text_body.text = "";
        text_name.text = "";
    }

    private void ActivateChoice(string line_a, string line_b)
    {
        choice_box.SetActive(true);

        current_choice = true;

        text_choice.text = line_a + "\n" + line_b + "\n" + "Yes" + (current_choice ? "*" : "") + "\nNo" + (current_choice ? "" : "*");
    }

    private void DeactivateChoice()
    {
        choice_box.SetActive(false);
        text_choice.text = "";
    }

    private bool ChangeChoice(Inputer inputer, out bool deactive)
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
            text_choice.text = text_choice + "Yes" + (current_choice ? "*" : "") + "\nNo" + (current_choice ? "" : "*");
            return false;
        }

        return false;
    }
}