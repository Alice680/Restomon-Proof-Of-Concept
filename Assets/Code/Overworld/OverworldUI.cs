using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverworldUI : MonoBehaviour
{
    private class ShopInfo
    {
        public Vector2Int[] values;

        public ShopInfo(Vector2Int[] values)
        {
            this.values = values;
        }
    }

    private class SmithInfo
    {
        public int[] weapon_values;
        public int[] accessory_value;

        public SmithInfo(int[] weapon_values, int[] accessory_value)
        {
            this.weapon_values = weapon_values;
            this.accessory_value = accessory_value;
        }
    }

    private class AtelierInfo
    {

        public int[] values;

        public AtelierInfo(int[] values)
        {
            this.values = values;
        }
    }
    [SerializeField] private GameObject[] town_icon_ref;

    [SerializeField] private GameObject text_box, choice_box, town_box, cam;

    [SerializeField] private Text dialogue_name, dialogue, choice;

    [SerializeField] private Text town_name, town_description, town_current_option;
    [SerializeField] private GameObject[] town_bars, town_icons;
    [SerializeField] private Text[] town_options;

    [SerializeField] private ManagerMenuHome menu_home;
    [SerializeField] private MenuShop menu_shop;
    [SerializeField] private MenuSmith menu_smith;
    [SerializeField] private MenuAtelier menu_atelier;
    [SerializeField] private MenuWorkshop menu_workshop;

    private PermDataHolder data_holder;

    private ShopInfo[] shop_info;
    private SmithInfo[] smith_info;
    private AtelierInfo[] atelier_info;

    private bool current_choice;
    private string text_choice;

    private DialogueTree dialogue_tree;
    private bool making_choice;
    private int current_node;

    private OverworldTown current_town;
    private int current_selection;
    private int current_area;
    private GameObject[] town_icon_objects;
    private bool town_dialouge;
    private TownFeatureType current_feature;

    public void Startup(PermDataHolder data_holder)
    {
        this.data_holder = data_holder;

        text_box.SetActive(false);
        choice_box.SetActive(false);
        town_box.SetActive(false);

        dialogue_name.text = "";
        dialogue.text = "";
        choice.text = "";

        current_feature = TownFeatureType.None;

        for (int i = 0; i < 8; ++i)
            town_options[i].text = "";

        town_icon_objects = new GameObject[8];

        menu_home.SetData(data_holder);
        menu_shop.Startup(data_holder);
        menu_smith.Startup(data_holder);
        menu_atelier.Startup(data_holder);
        menu_workshop.Startup(data_holder);
    }

    public void SetShop(List<Vector2Int[]> values)
    {
        shop_info = new ShopInfo[values.Count];

        for (int i = 0; i < shop_info.Length; ++i)
            shop_info[i] = new ShopInfo(values[i]);
    }

    public void SetSmith(List<int[]> weapon_values, List<int[]> accessory_value)
    {
        smith_info = new SmithInfo[weapon_values.Count];

        for (int i = 0; i < smith_info.Length; ++i)
            smith_info[i] = new SmithInfo(weapon_values[i], accessory_value[i]);
    }

    public void Set_Atelier(List<int[]> values)
    {
        atelier_info = new AtelierInfo[values.Count];

        for (int i = 0; i < atelier_info.Length; ++i)
            atelier_info[i] = new AtelierInfo(values[i]);
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

    public bool ChangeDialogue(Inputer inputer)
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
        town_box.SetActive(true);

        current_area = 0;

        current_town = town;

        ChangeTown(null);
    }

    public void DeactivateTown()
    {
        town_box.SetActive(false);

        town_name.text = "";
        town_description.text = "";
        town_current_option.text = "";

        for (int i = 0; i < 8; ++i)
        {
            town_bars[i].SetActive(false);

            town_options[i].text = "";

            if (town_icon_objects[i] != null)
                Destroy(town_icon_objects[i]);
        }
    }

    public bool ChangeTown(Inputer inputer)
    {
        if (town_dialouge)
        {
            if (ChangeDialogue(inputer))
            {
                town_dialouge = false;

                int temp_int = current_area;
                ActivateTown(current_town);
                current_area = temp_int;

                ChangeTown(null);
            }

            return false;
        }

        if (current_feature != TownFeatureType.None)
        {
            switch (current_feature)
            {
                case TownFeatureType.Home:
                    if (menu_home.Change(inputer))
                        current_feature = TownFeatureType.None;
                    else
                        return false;
                    break;

                case TownFeatureType.Shop:
                    if (menu_shop.Change(inputer))
                        current_feature = TownFeatureType.None;
                    else
                        return false;
                    break;
                case TownFeatureType.Smith:
                    if (menu_smith.Change(inputer))
                        current_feature = TownFeatureType.None;
                    else
                        return false;
                    break;
                case TownFeatureType.Atelier:
                    if (menu_atelier.Change(inputer))
                        current_feature = TownFeatureType.None;
                    else
                        return false;
                    break;
                case TownFeatureType.Workshop:
                    if (menu_workshop.Change(inputer))
                        current_feature = TownFeatureType.None;
                    else
                        return false;
                    break;
            }

            int temp_int = current_area;
            ActivateTown(current_town);
            current_area = temp_int;

            ChangeTown(null);

            return false;
        }

        if (inputer != null)
        {
            if (inputer.GetDir() != Direction.None && inputer.GetMoveKeyUp())
                current_selection = DirectionMath.GetMenuChange(current_selection, inputer.GetDir(), 2, 4);

            else if (inputer.GetEnter())
            {
                int temp_selection = current_town.GetSelection(data_holder, current_area, current_selection, out DialogueTree temp_dialogue, out TownFeatureType feature_type, out int feature_int);

                if (temp_selection == -1)
                {
                    DeactivateTown();
                    return true;
                }

                if (temp_selection != -2)
                {
                    current_area = temp_selection;
                    current_selection = 0;

                    if (temp_dialogue != null)
                    {
                        town_dialouge = true;
                        //DeactivateTown();
                        ActivateDialogue(temp_dialogue);
                        return false;
                    }

                    if (feature_type != TownFeatureType.None)
                    {
                        current_feature = feature_type;

                        DeactivateTown();

                        switch (current_feature)
                        {
                            case TownFeatureType.Home:
                                menu_home.Activate();
                                return false;
                            case TownFeatureType.Shop:
                                menu_shop.ActivateEX(shop_info[feature_int].values);
                                return false;
                            case TownFeatureType.Smith:
                                menu_smith.ActivateEX(smith_info[feature_int].weapon_values, smith_info[feature_int].accessory_value);
                                return false;
                            case TownFeatureType.Atelier:
                                menu_atelier.ActivateEX(atelier_info[feature_int].values);
                                return false;
                            case TownFeatureType.Workshop:
                                menu_workshop.Activate();
                                return false;
                        }

                        return false;
                    }
                }
            }
        
            else if (inputer.GetBack())
            {
                current_area = current_town.GetSelection(data_holder, current_area, 7, out DialogueTree temp_dialogue, out TownFeatureType feature_type, out int feature_int);
                current_selection = 0;

                if (current_area == -1)
                {
                    DeactivateTown();
                    return true;
                }
            }
        }

        town_description.text= current_town.GetBodyText(current_area, out string area_name);
        town_name.text = area_name;

        for (int i = 0; i < 8; ++i)
        {
            town_bars[i].SetActive(false);
            town_options[i].text = current_town.GetChoiceText(current_area, data_holder,out int[] temp_icon, out string[] temp_descriptions)[i];

            if (town_icon_objects[i] != null)
                Destroy(town_icon_objects[i]);

            if (i == current_selection)
                town_current_option.text = temp_descriptions[i];

            if (temp_icon[i] != 0)
            {
                town_icon_objects[i] = Instantiate(town_icon_ref[temp_icon[i]-1]);
                town_icon_objects[i].transform.parent = cam.transform;
                town_icon_objects[i].transform.position = town_icons[i].transform.position;
            }
        }

        town_bars[current_selection].SetActive(true);

        return false;
    }
}