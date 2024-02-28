using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A place to hold data between scenes.
 * It is also temporarly holding refrences to the dungeons and base classes
 * 
 * Notes:
 * Mose scenes load data from this, if you wan't to test in iscolation, make sure to overide.
 */
public class PermDataHolder : MonoBehaviour
{
    //Data classes
    private class GenericUnlocks
    {
        public int lv, trait_slots;
        public bool[] traits, catalysts;

        public GenericUnlocks()
        {
            lv = 20;
            trait_slots = 2;

            traits = new bool[5];
            for (int i = 0; i < 5; ++i)
                traits[i] = false;

            catalysts = new bool[3];
            for (int i = 0; i < 3; ++i)
                catalysts[i] = false;
        }
    }

    private class ClassUnlocks
    {
        public bool[] sub_classes, weapons, trinkets, loops, traits;

        public ClassUnlocks()
        {
            sub_classes = new bool[5];
            sub_classes[0] = true;
            for (int i = 1; i < 5; ++i)
                sub_classes[i] = false;

            weapons = new bool[5];
            weapons[0] = true;
            for (int i = 1; i < 5; ++i)
                weapons[i] = false;

            trinkets = new bool[6];
            trinkets[0] = true;
            for (int i = 1; i < 6; ++i)
                trinkets[i] = false;

            loops = new bool[1];
            for (int i = 0; i < 1; ++i)
                loops[i] = false;

            traits = new bool[5];
            for (int i = 0; i < 5; ++i)
                traits[i] = false;
        }
    }

    private class CurrenGenericData
    {
        public int job;
        public int money, pack_upgrades;
        public List<int> inventory;

        public CurrenGenericData()
        {
            job = 0;
            money = 15;
            pack_upgrades = 0;
            inventory = new List<int>();
        }
    }

    private class CurrentClassData
    {
        public int current_sub, current_weapon, current_trinket_a, current_trinket_b;
        public int[] free_traits;

        public CurrentClassData()
        {
            current_sub = 0;
            current_weapon = 0;
            current_trinket_a = 0;
            current_trinket_b = 0;

            free_traits = new int[2] { -1, -1 };
        }
    }

    [Serializable]
    private class RestomonData
    {
        public int restomon_id;
        public int[] form_value;

        public RestomonData()
        {
            restomon_id = 0;
            form_value = new int[4] { 0, 0, 0, 0 };
        }

        public RestomonData(int restomon_id, int[] form_value)
        {
            this.restomon_id = restomon_id;
            this.form_value = form_value;
        }
    }

    [Serializable]
    private class Form
    {
        public int[] attacks;
        public int trait;
    }

    [Serializable]
    private class RestomonBuild
    {
        public RestomonBase restomon_base;
        public Form[] base_form;
        public Form[] form_a;
        public Form[] form_b;
        public Form[] form_c;

        public Restomon GetBuild(int[] form_values)
        {
            int[] temp_attack = new int[10];

            for (int i = 0; i < 4; ++i)
                temp_attack[i] = base_form[form_values[0]].attacks[i];

            for (int i = 0; i < 2; ++i)
            {
                temp_attack[4 + i] = form_a[form_values[1]].attacks[i];
                temp_attack[6 + i] = form_a[form_values[2]].attacks[i];
                temp_attack[8 + i] = form_a[form_values[3]].attacks[i];
            }

            int[] temp_trait = new int[4] { base_form[form_values[0]].trait, form_a[form_values[1]].trait, form_b[form_values[1]].trait, form_c[form_values[1]].trait };

            return restomon_base.GetRestomon(3, temp_attack, temp_trait);
        }
    }

    //Console Data
    [SerializeField] private HumanClass[] classes;
    [SerializeField] private ItemHolder item_holder;

    //Private Data
    private GenericUnlocks generic_unlocks;

    private int current_class;
    private ClassUnlocks[] class_unlocks;
    private CurrenGenericData current_generic_data;
    private CurrentClassData[] current_class_data;

    [SerializeField] private DungeonLayout[] dungeons;
    private int current_dungeon;

    [SerializeField] private OverworldLayout[] overworlds;
    private int current_overworld;
    private Vector2Int current_position;

    [SerializeField] private Catalyst[] catalysts;
    private int current_catalyst;

    [SerializeField] private RestomonBuild[] restomon_builds;
    private RestomonData[] current_team;

    private int[] main_quest_markers;
    private int[] side_quest_markers;
    private bool[] dungeon_unlocked;
    private bool[] dungeon_cleared;

    //Methods
    public void SetupData()
    {
        generic_unlocks = new GenericUnlocks();
        current_generic_data = new CurrenGenericData();

        class_unlocks = new ClassUnlocks[3];
        current_class_data = new CurrentClassData[3];

        for (int i = 0; i < 3; ++i)
        {
            class_unlocks[i] = new ClassUnlocks();
            current_class_data[i] = new CurrentClassData();
        }

        item_holder.SetList();

        DontDestroyOnLoad(gameObject);

        current_dungeon = 0;

        current_overworld = 0;

        current_position = new Vector2Int(7, 4);


        current_catalyst = 0;

        main_quest_markers = new int[2];
        side_quest_markers = new int[3];
        dungeon_unlocked = new bool[2];
        dungeon_cleared = new bool[2];

        dungeon_unlocked[0] = true;

        current_team = new RestomonData[4] { new RestomonData(), new RestomonData(), new RestomonData(), new RestomonData() };

    }

    public void LoadData()
    {

    }

    public void SaveData()
    {

    }

    //Class Data
    public int GetPlayerClass()
    {
        return current_class;
    }

    public void SetPlayerClass(int new_class)
    {
        current_class = new_class;
    }

    public int[] GetPlayerGear(out string[,] sub_string, out string[,] weapon_string, out string[,] trinket_string)
    {
        HumanClass temp_human = classes[current_class];
        ClassUnlocks temp_unlocks = class_unlocks[current_class];
        CurrentClassData temp_class = current_class_data[current_class];

        sub_string = new string[5, 2];
        weapon_string = new string[6, 2];
        trinket_string = new string[6, 2];

        for (int i = 0; i < 5; ++i)
        {
            if (temp_unlocks.sub_classes[i])
            {
                sub_string[i, 0] = temp_human.GetSubclassName(i);
                sub_string[i, 1] = temp_human.GetSubclassDescription(i);
            }
            else if (temp_unlocks.sub_classes[0])
            {
                sub_string[i, 0] = "Locked.";
                sub_string[i, 1] = "Find someone to train you in this subclass first";
            }
        }

        for (int i = 0; i < 5; ++i)
        {
            if (temp_unlocks.weapons[i])
            {
                weapon_string[i, 0] = temp_human.GetWeaponName(i);
                weapon_string[i, 1] = temp_human.GetWeaponDescription(i);
            }
            else
            {
                weapon_string[i, 0] = "Locked.";
                weapon_string[i, 1] = "Find this weapon to be able to use it.";
            }
        }

        for (int i = 0; i < 6; ++i)
        {
            if (temp_unlocks.trinkets[i])
            {
                trinket_string[i, 0] = temp_human.GetTrinketName(i);
                trinket_string[i, 1] = temp_human.GetTrinketDescription(i);
            }
            else
            {
                trinket_string[i, 0] = "Locked.";
                trinket_string[i, 1] = "Find this weapon to be able to use it.";
            }
        }

        return new int[4] { temp_class.current_sub, temp_class.current_weapon, temp_class.current_trinket_a, temp_class.current_trinket_b };
    }

    public void SetPlayerGear(int sub_value, int weapon_value, int trinket_a_value, int trinket_b_value)
    {
        ClassUnlocks temp_unlocks = class_unlocks[current_class];
        CurrentClassData temp_class = current_class_data[current_class];

        if (temp_class.current_sub != sub_value)
        {
            for (int i = 0; i < 5; ++i)
                if (current_class_data[current_class].free_traits[i] >= 25)
                    current_class_data[current_class].free_traits[i] = -1;
        }

        if (temp_unlocks.sub_classes[sub_value])
            temp_class.current_sub = sub_value;
        else
            temp_class.current_sub = 0;

        if (temp_unlocks.weapons[weapon_value])
            temp_class.current_weapon = weapon_value;
        else
            temp_class.current_weapon = 0;

        if (temp_unlocks.trinkets[trinket_a_value])
            temp_class.current_trinket_a = trinket_a_value;
        else
            temp_class.current_trinket_a = 0;

        if (temp_unlocks.trinkets[trinket_b_value] && trinket_a_value != trinket_b_value)
            temp_class.current_trinket_b = trinket_b_value;
        else
            temp_class.current_trinket_b = 0;
    }

    public string[,] GetPlayerTraits(out bool[] unlocked, out int[] selected)
    {
        string[,] temp_string = new string[30, 2];
        unlocked = new bool[30];

        for (int i = 0; i < 5; ++i)
        {
            if (generic_unlocks.traits[i])
            {
                temp_string[i, 0] = classes[current_class].GetTraitName(i + 1);
                temp_string[i, 1] = classes[current_class].GetTraitDescription(i + 1);
                unlocked[i] = true;
            }
            else
            {
                temp_string[i, 0] = "Locked";
                temp_string[i, 1] = "Find and complete the related quest to unlock this trait.";
                unlocked[i] = false;
            }

            if (class_unlocks[current_class].traits[i])
            {
                temp_string[i + 5, 0] = classes[current_class].GetTraitName(i + 6);
                temp_string[i + 5, 1] = classes[current_class].GetTraitDescription(i + 6);
                unlocked[i + 5] = true;
            }
            else
            {
                temp_string[i + 5, 0] = "Locked";
                temp_string[i + 5, 1] = "Find and complete the related quest to unlock this trait.";
                unlocked[i + 5] = false;
            }
        }

        for (int i = 0; i < 10; ++i)
        {
            if (generic_unlocks.lv >= 1 + (i * 2))
            {
                temp_string[i + 10, 0] = classes[current_class].GetTraitName(i + 10);
                temp_string[i + 10, 1] = classes[current_class].GetTraitDescription(i + 10);
                unlocked[i + 10] = true;
            }
            else
            {
                temp_string[i + 10, 0] = "Locked";
                temp_string[i + 10, 1] = "Reach level " + (1 + (i * 2)) + " to unlock this trait.";
                unlocked[i + 10] = false;
            }

            if (generic_unlocks.lv >= (i + 1 * 2))
            {
                if (i % 2 == 0)
                {
                    temp_string[i + 20, 0] = classes[current_class].GetTraitName(20 + (i/2));
                    temp_string[i + 20, 1] = classes[current_class].GetTraitDescription(20 + (i / 2));
                }
                else
                {
                    temp_string[i + 20, 0] = classes[current_class].GetSubTraitName(current_class_data[current_class].current_sub, i / 2);
                    temp_string[i + 20, 1] = classes[current_class].GetSubTraitDescription(current_class_data[current_class].current_sub, i / 2);
                }
                unlocked[i + 20] = true;
            }
            else
            {
                temp_string[i + 20, 0] = "Locked";
                temp_string[i + 20, 1] = "Reach level " + ((i + 1) * 2) + " to unlock this trait.";
                unlocked[i + 20] = false;
            }
        }

        selected = new int[generic_unlocks.trait_slots];

        for (int i = 0; i < selected.Length; ++i)
            selected[i] = current_class_data[current_class].free_traits[i];

        return temp_string;
    }

    public void SetPlayerTraits(int[] selected)
    {
        for (int i = 0; i < selected.Length; ++i)
        {
            if (i < generic_unlocks.trait_slots)
                current_class_data[current_class].free_traits[i] = selected[i];
            else
                current_class_data[current_class].free_traits[i] = -1;
        }
    }

    public Human GetPlayer()
    {
        CurrentClassData temp_data = current_class_data[current_class];
        return classes[current_class].GetHuman(generic_unlocks.lv, temp_data.current_sub, temp_data.current_weapon, temp_data.current_trinket_a, temp_data.current_trinket_b, temp_data.free_traits, 0);
    }

    //Inventory
    public int GetMoney()
    {
        return current_generic_data.money;
    }

    public void AddMoney(int money)
    {
        current_generic_data.money = money;
    }

    public int GetInventorySize()
    {
        return 4 + (current_generic_data.pack_upgrades * 2);
    }

    public int GetInventoryCount()
    {
        return current_generic_data.inventory.Count;
    }

    public int GetInventorySlot(int index)
    {
        if (index < GetInventoryCount())
            return current_generic_data.inventory[index];

        return 0;
    }

    public void AddInventory(int index)
    {
        if (GetInventoryCount() < GetInventorySize())
            current_generic_data.inventory.Add(index);
    }

    public void RemoveInventory(int index)
    {
        if (index < GetInventoryCount())
            current_generic_data.inventory.RemoveAt(index);
    }

    public void UpgradeInventory()
    {
        ++current_generic_data.pack_upgrades;
    }

    //Dungeon
    public void SetDungeon(int index)
    {
        current_dungeon = index;
    }

    public int GetDungeonInt()
    {
        return current_dungeon;
    }

    public DungeonLayout GetDungeon()
    {
        return dungeons[current_dungeon];
    }

    //Overworld
    public void SetOverworld(int index, Vector2Int position)
    {
        current_overworld = index;
        current_position = position;
    }

    public void SetPosition(Vector2Int position)
    {
        current_position = position;
    }

    public int GetOverworldInt()
    {
        return current_overworld;
    }

    public OverworldLayout GetOverworld(out Vector2Int position)
    {
        position = current_position;
        return overworlds[current_overworld];
    }

    //Team
    public void SetCatalyst(int index)
    {
        if (index < 0)
            current_catalyst = 0;
        else
            current_catalyst = index;
    }

    public int GetCatalystInt()
    {
        return current_catalyst;
    }

    public Catalyst GetCatalyst()
    {
        return catalysts[current_catalyst];
    }

    public void SetRestomon(int index)
    {
        current_team[index] = new RestomonData();
    }

    public void SetRestomon(int index, int restomon_id, int[] form_value)
    {
        current_team[index] = new RestomonData(restomon_id, form_value);
    }

    public int GetRestomonInt(int index, out int[] form_value)
    {
        form_value = current_team[index].form_value;

        return current_team[index].restomon_id;
    }

    public Restomon GetTeam(int index)
    {
        return restomon_builds[current_team[index].restomon_id].GetBuild(current_team[index].form_value);
    }

    //Data
    public int GetEventData(EventDataType data_type, int index)
    {
        switch (data_type)
        {
            case EventDataType.MainQuest:
                return main_quest_markers[index];
            case EventDataType.SideQuest:
                return side_quest_markers[index];
        }

        return -1;
    }

    public void SetEventData(EventDataType data_type, int index, int value)
    {
        switch (data_type)
        {
            case EventDataType.MainQuest:
                main_quest_markers[index] = value;
                return;
            case EventDataType.SideQuest:
                side_quest_markers[index] = value;
                return;
        }
    }

    public bool GetDungeonData(DungeonDataType data_type, int index)
    {
        switch (data_type)
        {
            case DungeonDataType.DungeonUnlocked:
                return dungeon_unlocked[index];
            case DungeonDataType.DungeonCleared:
                return dungeon_cleared[index];
        }

        return false;
    }

    public void SetDungeonData(DungeonDataType data_type, int index, bool value)
    {
        switch (data_type)
        {
            case DungeonDataType.DungeonUnlocked:
                dungeon_unlocked[index] = value;
                return;
            case DungeonDataType.DungeonCleared:
                dungeon_cleared[index] = value;
                return;
        }
    }

    //Get Data
    public HumanClass GetDataHuman(int index)
    {
        return classes[index];
    }

    public Catalyst GetDataCatalyst(int index)
    {
        return catalysts[index];
    }

    public RestomonBase GetDataRestomon(int index)
    {
        return restomon_builds[index].restomon_base;
    }
}