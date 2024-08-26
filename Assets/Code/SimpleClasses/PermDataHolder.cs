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
    [Serializable]
    private class GenericUnlocks
    {
        public int lv, trait_slots;
        public bool[] traits, catalysts, researches;

        public GenericUnlocks(int research_length)
        {
            lv = 0;
            trait_slots = 2;

            traits = new bool[5];
            for (int i = 0; i < 5; ++i)
                traits[i] = false;

            catalysts = new bool[8];
            for (int i = 0; i < 8; ++i)
                catalysts[i] = false;

            catalysts[0] = true;

            researches = new bool[research_length];

            for (int i = 0; i < research_length; ++i)
                researches[i] = false;
        }
    }

    [Serializable]
    private class ClassUnlocks
    {
        public bool[] sub_classes, weapons, trinkets, loops, traits;

        public ClassUnlocks()
        {
            sub_classes = new bool[6];
            sub_classes[0] = true;
            for (int i = 1; i < 6; ++i)
                sub_classes[i] = false;

            weapons = new bool[24];
            weapons[0] = true;
            for (int i = 1; i < 24; ++i)
                weapons[i] = false;

            trinkets = new bool[12];
            trinkets[0] = true;
            for (int i = 1; i < 12; ++i)
                trinkets[i] = false;

            loops = new bool[1];
            for (int i = 0; i < 1; ++i)
                loops[i] = false;

            traits = new bool[5];
            for (int i = 0; i < 5; ++i)
                traits[i] = false;
        }
    }

    [Serializable]
    private class RestomonUnlocks
    {
        public bool unlocked;
        public int reforges, refinements;

        public bool[] basic_attacks, evolution, mutations;
        public List<bool[]> attacks, traits;

        public RestomonUnlocks(RestomonBase base_data)
        {
            unlocked = false;
            reforges = 0;
            refinements = 0;

            evolution = new bool[10];
            mutations = new bool[2];

            basic_attacks = new bool[base_data.GetBasicAttacks().Length];

            attacks = new List<bool[]>();
            traits = new List<bool[]>();

            for (int i = 0; i < 10; ++i)
            {
                attacks.Add(new bool[base_data.GetAttacks(i).Length]);
                traits.Add(new bool[base_data.GetTraits(i).Length]);
                attacks[i][0] = true;
                traits[i][0] = true;
            }

            for (int i = 0; i < 2; ++i)
                basic_attacks[i] = true;
        }
    }

    [Serializable]
    private class CurrentQuestData
    {
        public int[] main_quest_markers;
        public int[] side_quest_markers;
        public bool[] dungeon_unlocked;
        public bool[] dungeon_cleared;

        public CurrentQuestData()
        {
            main_quest_markers = new int[10];
            side_quest_markers = new int[10];
            dungeon_unlocked = new bool[10];
            dungeon_cleared = new bool[10];
        }
    }

    [Serializable]
    private class CurrenGenericData
    {
        public bool save_data;
        public string player_name;
        public int current_chapter;
        public int job;
        public int money, pack_upgrades;
        public List<int> inventory;
        public int[] storage;
        public int current_class, current_catalyst;

        public int corruption_count;
        public int[] core_damage;

        public int current_dungeon;
        public int current_overworld;
        public Vector2Int current_position;

        public CurrenGenericData()
        {
            save_data = false;
            player_name = "player";
            current_catalyst = 0;
            job = 0;
            money = 15;
            pack_upgrades = 0;
            inventory = new List<int>();
            storage = new int[5];
            current_class = 3;
            current_catalyst = 0;
            corruption_count = 0;
            core_damage = new int[8];
            current_dungeon = 0;
            current_overworld = 0;
            current_position = new Vector2Int(26, 25);
        }
    }

    [Serializable]
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
    private class CurrentRestomonData
    {
        public int current_mutation;
        public Vector2Int[] current_attacks;
        public int[] current_traits;
        public int[] current_refinements;

        public CurrentRestomonData()
        {
            current_mutation = 0;
            current_attacks = new Vector2Int[11];
            current_traits = new int[11];
            current_refinements = new int[7];

            for (int i = 0; i < current_attacks.Length; ++i)
            {
                current_attacks[i].x = 0;
                current_attacks[i].y = 1;
                current_traits[i] = 0;
            }
        }
    }

    [Serializable]
    private class CurrentTeamData
    {
        public int size;
        public int[] restomon_id;

        public CurrentTeamData(Catalyst team_catalyst)
        {
            size = team_catalyst.GetTeamSize();
            restomon_id = new int[size];

            for (int i = 0; i < size; ++i)
                restomon_id[i] = -1;

            //TODO remove
            restomon_id[0] = 0;
        }
    }


    //Console Data
    [SerializeField] private HumanClass[] classes;
    [SerializeField] private RestomonBase[] restomon_bases;
    [SerializeField] private Catalyst[] catalysts;
    [SerializeField] private DungeonLayout[] dungeons;
    [SerializeField] private OverworldLayout[] overworlds;

    [SerializeField] private ResearchData[] research_data;
    [SerializeField] private GearData[] weapon_data;
    [SerializeField] private GearData[] trinket_data;

    [SerializeField] private Trait[] corruption_effect;

    [SerializeField] private ItemHolder item_holder;

    [SerializeField] private UpgradeCost[] RankCost;

    //Build Data
    private int current_save_file;

    private GenericUnlocks generic_unlocks;
    private ClassUnlocks[] class_unlocks;
    private RestomonUnlocks[] restomon_unlocks;

    private CurrentQuestData current_quest_data;
    private CurrenGenericData current_generic_data;
    private CurrentClassData[] current_class_data;
    private CurrentRestomonData[] current_restomon_data;
    private CurrentTeamData[] current_team_data;

    //Save Data
    public void SetSaveFile(int index)
    {
        current_save_file = Mathf.Clamp(index, 0, 2);
    }

    public void CreateSaveFile(int index, string name)
    {
        SetupData();
        SetSaveFile(index);
        current_generic_data.player_name = name;
        current_generic_data.save_data = true;
        SaveData();
    }

    public string[] GetFiles(out bool[] has_data)
    {
        string[] temp_files = new string[9];
        has_data = new bool[3];

        string[] chapter_names = new string[1] { "Demo" };
        string[] class_names = new string[4] { "", "", "", "Pesant" };

        for (int i = 0; i < 3; ++i)
        {
            current_save_file = i;
            LoadData();
            has_data[i] = current_generic_data.save_data;
            temp_files[i * 3] = current_generic_data.player_name;
            temp_files[i * 3 + 1] = chapter_names[current_generic_data.current_chapter];
            temp_files[i * 3 + 2] = class_names[current_generic_data.current_class];
        }

        return temp_files;
    }

    public void SetupData()
    {
        generic_unlocks = new GenericUnlocks(research_data.Length);
        class_unlocks = new ClassUnlocks[3];
        restomon_unlocks = new RestomonUnlocks[36];

        current_quest_data = new CurrentQuestData();
        current_generic_data = new CurrenGenericData();
        current_class_data = new CurrentClassData[3];
        current_restomon_data = new CurrentRestomonData[36];
        current_team_data = new CurrentTeamData[8];

        for (int i = 0; i < 3; ++i)
        {
            class_unlocks[i] = new ClassUnlocks();
            current_class_data[i] = new CurrentClassData();
        }

        for (int i = 0; i < 36; ++i)
        {
            restomon_unlocks[i] = new RestomonUnlocks(restomon_bases[i]);
            current_restomon_data[i] = new CurrentRestomonData();
        }

        restomon_unlocks[0].unlocked = true;

        for (int i = 0; i < 8; ++i)
        {
            current_team_data[i] = new CurrentTeamData(catalysts[i]);
        }

        item_holder.SetList();

        DontDestroyOnLoad(gameObject);

        Rest();
    }

    public void LoadData()
    {
        string file_path = "";
        string temp_save = "";

        try
        {
            file_path = Application.persistentDataPath + "/hasdata.jason";
            temp_save = System.IO.File.ReadAllText(file_path);
            string temp = temp_save;
        }
        catch (Exception e)
        {
            WipeData();
            Debug.Log(e);
        }

        try
        {
            file_path = Application.persistentDataPath + "/genericunlocks" + current_save_file + ".jason";
            temp_save = System.IO.File.ReadAllText(file_path);
            generic_unlocks = JsonUtility.FromJson<GenericUnlocks>(temp_save);

            for (int i = 0; i < 3; ++i)
            {
                file_path = Application.persistentDataPath + "/classunlocks" + i + "x" + current_save_file + ".jason";
                temp_save = System.IO.File.ReadAllText(file_path);
                class_unlocks[i] = JsonUtility.FromJson<ClassUnlocks>(temp_save);
            }

            for (int i = 0; i < 36; ++i)
            {
                file_path = Application.persistentDataPath + "/restomonunlocks" + i + "x" + current_save_file + ".jason";
                temp_save = System.IO.File.ReadAllText(file_path);
                restomon_unlocks[i] = JsonUtility.FromJson<RestomonUnlocks>(temp_save);
            }

            file_path = Application.persistentDataPath + "/currentquestdata" + current_save_file + ".jason";
            temp_save = System.IO.File.ReadAllText(file_path);
            current_quest_data = JsonUtility.FromJson<CurrentQuestData>(temp_save);

            file_path = Application.persistentDataPath + "/currentgenericdata" + current_save_file + ".jason";
            temp_save = System.IO.File.ReadAllText(file_path);
            current_generic_data = JsonUtility.FromJson<CurrenGenericData>(temp_save);

            for (int i = 0; i < 3; ++i)
            {
                file_path = Application.persistentDataPath + "/currentclassdata" + i + "x" + current_save_file + ".jason";
                temp_save = System.IO.File.ReadAllText(file_path);
                current_class_data[i] = JsonUtility.FromJson<CurrentClassData>(temp_save);
            }

            for (int i = 0; i < 36; ++i)
            {
                file_path = Application.persistentDataPath + "/currentrestomondata" + i + "x" + current_save_file + ".jason";
                temp_save = System.IO.File.ReadAllText(file_path);
                current_restomon_data[i] = JsonUtility.FromJson<CurrentRestomonData>(temp_save);
            }

            for (int i = 0; i < 8; ++i)
            {
                file_path = Application.persistentDataPath + "/currentteamdata" + i + "x" + current_save_file + ".jason";
                temp_save = System.IO.File.ReadAllText(file_path);
                current_team_data[i] = JsonUtility.FromJson<CurrentTeamData>(temp_save);
            }
        }
        catch (Exception e)
        {
            WipeData();
            LoadData();
            Debug.Log("Damaged Save");
            Debug.Log(e);
        }
    }

    public void SaveData()
    {
        string file_path;
        string temp_save;

        file_path = Application.persistentDataPath + "/hasdata.jason";
        temp_save = "Version 0.0.1";
        System.IO.File.WriteAllText(file_path, temp_save);

        file_path = Application.persistentDataPath + "/genericunlocks" + current_save_file + ".jason";
        temp_save = JsonUtility.ToJson(generic_unlocks);
        System.IO.File.WriteAllText(file_path, temp_save);

        for (int i = 0; i < 3; ++i)
        {
            file_path = Application.persistentDataPath + "/classunlocks" + i + "x" + current_save_file + ".jason";
            temp_save = JsonUtility.ToJson(class_unlocks[i]);
            System.IO.File.WriteAllText(file_path, temp_save);
        }

        for (int i = 0; i < 36; ++i)
        {
            file_path = Application.persistentDataPath + "/restomonunlocks" + i + "x" + current_save_file + ".jason";
            temp_save = JsonUtility.ToJson(restomon_unlocks[i]);
            System.IO.File.WriteAllText(file_path, temp_save);
        }

        file_path = Application.persistentDataPath + "/currentquestdata" + current_save_file + ".jason";
        temp_save = JsonUtility.ToJson(current_quest_data);
        System.IO.File.WriteAllText(file_path, temp_save);

        file_path = Application.persistentDataPath + "/currentgenericdata" + current_save_file + ".jason";
        temp_save = JsonUtility.ToJson(current_generic_data);
        System.IO.File.WriteAllText(file_path, temp_save);

        for (int i = 0; i < 3; ++i)
        {
            file_path = Application.persistentDataPath + "/currentclassdata" + i + "x" + current_save_file + ".jason";
            temp_save = JsonUtility.ToJson(current_class_data[i]);
            System.IO.File.WriteAllText(file_path, temp_save);
        }

        for (int i = 0; i < 36; ++i)
        {
            file_path = Application.persistentDataPath + "/currentrestomondata" + i + "x" + current_save_file + ".jason";
            temp_save = JsonUtility.ToJson(current_restomon_data[i]);
            System.IO.File.WriteAllText(file_path, temp_save);
        }

        for (int i = 0; i < 8; ++i)
        {
            file_path = Application.persistentDataPath + "/currentteamdata" + i + "x" + current_save_file + ".jason";
            temp_save = JsonUtility.ToJson(current_team_data[i]);
            System.IO.File.WriteAllText(file_path, temp_save);
        }
    }

    public void WipeData()
    {
        SetupData();
        for (int i = 0; i < 3; ++i)
        {
            current_save_file = i;
            SaveData();
        }
    }

    //Corruption
    public void Rest()
    {
        current_generic_data.corruption_count = 0;

        current_generic_data.core_damage = new int[8];
        for (int i = 0; i < 8; ++i)
            current_generic_data.core_damage[i] = 10;
    }

    public Trait GetCorruptionTrait()
    {
        return corruption_effect[0];
    }

    public int GetCorruption()
    {
        return current_generic_data.corruption_count;
    }

    public int GetCoreDamage(int index)
    {
        return current_generic_data.core_damage[index];
    }

    public void ModifyCorruption(int value)
    {
        current_generic_data.corruption_count = Mathf.Clamp(value + current_generic_data.corruption_count, 0, 100);
    }

    public int ModifyCoreDamage(int index, int value)
    {
        return current_generic_data.core_damage[index] = Mathf.Clamp(value + current_generic_data.core_damage[index], 0, 10);
    }

    //Class Data
    public int GetPlayerClass()
    {
        return current_generic_data.current_class;
    }

    public void SetPlayerClass(int new_class)
    {
        current_generic_data.current_class = new_class;
    }

    public int[] GetPlayerGear(out string[,] sub_string, out string[,] weapon_string, out string[,] trinket_string)
    {
        HumanClass temp_human = classes[current_generic_data.current_class];
        ClassUnlocks temp_unlocks = class_unlocks[current_generic_data.current_class];
        CurrentClassData temp_class = current_class_data[current_generic_data.current_class];

        sub_string = new string[6, 2];
        weapon_string = new string[24, 2];
        trinket_string = new string[12, 2];

        for (int i = 0; i < 6; ++i)
        {
            if (temp_unlocks.sub_classes[i])
            {
                sub_string[i, 0] = temp_human.GetSubclassName(i);
                sub_string[i, 1] = temp_human.GetSubclassDescription(i);
            }
            else
            {
                sub_string[i, 0] = "Locked";
                sub_string[i, 1] = "Find someone to train you in this subclass first";
            }
        }

        for (int i = 0; i < 24; ++i)
        {
            if (temp_unlocks.weapons[i])
            {
                weapon_string[i, 0] = temp_human.GetWeaponName(i);
                weapon_string[i, 1] = temp_human.GetWeaponDescription(i);
            }
            else
            {
                weapon_string[i, 0] = "Locked";
                weapon_string[i, 1] = "Find this weapon to be able to use it.";
            }
        }

        for (int i = 0; i < 12; ++i)
        {
            if (temp_unlocks.trinkets[i])
            {
                trinket_string[i, 0] = temp_human.GetTrinketName(i);
                trinket_string[i, 1] = temp_human.GetTrinketDescription(i);
            }
            else
            {
                trinket_string[i, 0] = "Locked";
                trinket_string[i, 1] = "Find this weapon to be able to use it.";
            }
        }

        return new int[4] { temp_class.current_sub, temp_class.current_weapon, temp_class.current_trinket_a, temp_class.current_trinket_b };
    }

    public void SetPlayerGear(int sub_value, int weapon_value, int trinket_a_value, int trinket_b_value)
    {
        ClassUnlocks temp_unlocks = class_unlocks[current_generic_data.current_class];
        CurrentClassData temp_class = current_class_data[current_generic_data.current_class];

        if (temp_class.current_sub != sub_value)
        {
            for (int i = 0; i < 5; ++i)
                if (current_class_data[current_generic_data.current_class].free_traits[i] >= 25)
                    current_class_data[current_generic_data.current_class].free_traits[i] = -1;
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
                temp_string[i, 0] = classes[current_generic_data.current_class].GetTraitName(i + 1);
                temp_string[i, 1] = classes[current_generic_data.current_class].GetTraitDescription(i + 1);
                unlocked[i] = true;
            }
            else
            {
                temp_string[i, 0] = "Locked";
                temp_string[i, 1] = "Find and complete the related quest to unlock this trait.";
                unlocked[i] = false;
            }

            if (class_unlocks[current_generic_data.current_class].traits[i])
            {
                temp_string[i + 5, 0] = classes[current_generic_data.current_class].GetTraitName(i + 6);
                temp_string[i + 5, 1] = classes[current_generic_data.current_class].GetTraitDescription(i + 6);
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
                temp_string[i + 10, 0] = classes[current_generic_data.current_class].GetTraitName(i + 10);
                temp_string[i + 10, 1] = classes[current_generic_data.current_class].GetTraitDescription(i + 10);
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
                    temp_string[i + 20, 0] = classes[current_generic_data.current_class].GetTraitName(20 + (i / 2));
                    temp_string[i + 20, 1] = classes[current_generic_data.current_class].GetTraitDescription(20 + (i / 2));
                }
                else
                {
                    temp_string[i + 20, 0] = classes[current_generic_data.current_class].GetSubTraitName(current_class_data[current_generic_data.current_class].current_sub, i / 2);
                    temp_string[i + 20, 1] = classes[current_generic_data.current_class].GetSubTraitDescription(current_class_data[current_generic_data.current_class].current_sub, i / 2);
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
            selected[i] = current_class_data[current_generic_data.current_class].free_traits[i];

        return temp_string;
    }

    public void SetPlayerTraits(int[] selected)
    {
        for (int i = 0; i < selected.Length; ++i)
        {
            if (i < generic_unlocks.trait_slots)
                current_class_data[current_generic_data.current_class].free_traits[i] = selected[i];
            else
                current_class_data[current_generic_data.current_class].free_traits[i] = -1;
        }
    }

    public Human GetPlayer()
    {
        if (current_generic_data.current_class != 3)
        {
            CurrentClassData temp_data = current_class_data[current_generic_data.current_class];
            return classes[current_generic_data.current_class].GetHuman(generic_unlocks.lv, temp_data.current_sub, temp_data.current_weapon, temp_data.current_trinket_a, temp_data.current_trinket_b, temp_data.free_traits, 0);
        }
        else
        {
            CurrentClassData temp_data = current_class_data[0];
            return classes[3].GetHuman(generic_unlocks.lv, temp_data.current_sub, temp_data.current_weapon, temp_data.current_trinket_a, temp_data.current_trinket_b, temp_data.free_traits, 0);
        }
    }

    public HumanClass GetDataHuman(int index)
    {
        return classes[index];
    }

    //Team Data
    public void SetCatalyst(int index)
    {
        if (index < 0)
            current_generic_data.current_catalyst = 0;
        else
            current_generic_data.current_catalyst = index;
    }

    public int GetCatalystInt()
    {
        return current_generic_data.current_catalyst;
    }

    public bool CatalystUnloked(int index)
    {
        if (index < 0 || index > 8)
            return false;

        return generic_unlocks.catalysts[index];
    }

    public Catalyst GetCatalyst(int index)
    {
        if (index < 0 || index > 8)
            return null;

        return catalysts[index];
    }

    public Catalyst GetCatalyst()
    {
        return catalysts[current_generic_data.current_catalyst];
    }

    //Restomon Data
    public void SetRestomon(int index, int value)
    {
        current_team_data[current_generic_data.current_catalyst].restomon_id[index] = value;
    }

    public bool GetRestomonUnlocked(int index)
    {
        return restomon_unlocks[index].unlocked;
    }

    public void GetRestomonUnlockInfo(int index, out int reforges, out int refinements, out bool[] mutations, out bool[] basic_attacks, out bool[] evolutions, out List<bool[]> attacks, out List<bool[]> traits)
    {
        reforges = restomon_unlocks[index].reforges;
        refinements = restomon_unlocks[index].refinements;
        mutations = restomon_unlocks[index].mutations;
        basic_attacks = restomon_unlocks[index].basic_attacks;
        evolutions = restomon_unlocks[index].evolution;
        attacks = restomon_unlocks[index].attacks;
        traits = restomon_unlocks[index].traits;
    }

    public void SetRestomonUnlockInfo(int index, int reforges, int refinements, bool[] mutations, bool[] basic_attacks, List<bool[]> attacks, List<bool[]> traits)
    {
        restomon_unlocks[index].reforges = reforges;
        restomon_unlocks[index].refinements = refinements;
        restomon_unlocks[index].mutations = mutations;
        restomon_unlocks[index].basic_attacks = basic_attacks;
        restomon_unlocks[index].attacks = attacks;
        restomon_unlocks[index].traits = traits;
    }

    public int GetRestomonInt(int index)
    {
        return current_team_data[current_generic_data.current_catalyst].restomon_id[index];
    }

    public void GetRestomonInfo(int index, out Vector2Int[] current_attacks, out int[] current_traits, out int[] current_refinements, out int total_refinements, out int reforges)
    {
        current_attacks = current_restomon_data[index].current_attacks;
        current_traits = current_restomon_data[index].current_traits;
        current_refinements = current_restomon_data[index].current_refinements;
        total_refinements = restomon_unlocks[index].refinements * 3;
        reforges = restomon_unlocks[index].reforges;
    }

    public void SetRestomonInfo(int index, Vector2Int[] current_attacks, int[] current_traits, int[] current_points)
    {
        current_restomon_data[index].current_attacks = current_attacks;
        current_restomon_data[index].current_traits = current_traits;
        current_restomon_data[index].current_refinements = current_points;
    }

    public bool EvolutionUnlocked(int index, RestomonEvolution current_evolution, int change)
    {
        int temp_evolution = 0;

        if (current_evolution == RestomonEvolution.None)
            temp_evolution = change + 1;
        else if (current_evolution == RestomonEvolution.FormA)
        {
            if (change == 0)
                temp_evolution = 6;
            else if (change == 1)
                temp_evolution = 7;
            else if (change == 2)
                temp_evolution = 4;
        }
        else if (current_evolution == RestomonEvolution.FormB)
        {
            if (change == 0)
                temp_evolution = 4;
            else if (change == 1)
                temp_evolution = 8;
            else if (change == 2)
                temp_evolution = 5;
        }
        else if (current_evolution == RestomonEvolution.FormC)
        {
            if (change == 0)
                temp_evolution = 5;
            else if (change == 1)
                temp_evolution = 9;
            else if (change == 2)
                temp_evolution = 6;
        }

        return restomon_unlocks[index].evolution[temp_evolution];
    }

    public RestomonBase GetRestomonData(int index)
    {
        return restomon_bases[index];
    }

    public Restomon GetTeam(int index)
    {
        if (index < 0 || index >= current_team_data[current_generic_data.current_catalyst].size || current_team_data[current_generic_data.current_catalyst].restomon_id[index] == -1)
            return null;

        int temp_restomon = current_team_data[current_generic_data.current_catalyst].restomon_id[index];

        CurrentRestomonData temp_data = current_restomon_data[temp_restomon];

        return restomon_bases[temp_restomon].GetRestomon(restomon_unlocks[temp_restomon].reforges, temp_data.current_refinements, 0, temp_data.current_attacks, temp_data.current_traits);
    }

    //Items Data
    public int GetMoney()
    {
        return current_generic_data.money;
    }

    public void ChangeMoney(int money)
    {
        current_generic_data.money += money;
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

    public string[] GetStorageData(out int[] ids, out string[] descriptions)
    {
        List<int> temp_ids = new List<int>();
        List<string> temp_descriptions = new List<string>();
        List<string> temp_names = new List<string>();

        for (int i = 0; i < current_generic_data.storage.Length; ++i)
            if (current_generic_data.storage[i] != 0)
            {
                temp_ids.Add(i);
                temp_names.Add(current_generic_data.storage[i] + "  " + ItemHolder.GetItem(i).GetInfo(out string temp_des));
                temp_descriptions.Add(temp_des);
            }

        ids = temp_ids.ToArray();
        descriptions = temp_descriptions.ToArray();
        return temp_names.ToArray();
    }

    public void ChangeStorage(int index, int value)
    {
        current_generic_data.storage[index] = Mathf.Max(0, current_generic_data.storage[index] + value);
    }

    public int GetNumberOfItem(int index)
    {
        int temp_value = current_generic_data.storage[index];

        for (int i = 0; i < GetInventoryCount(); ++i)
            if (current_generic_data.inventory[i] == index)
                ++temp_value;

        return temp_value;
    }

    public void AddItem(int index, int quantity)
    {
        while (quantity > 0 && GetInventoryCount() < GetInventorySize())
        {
            --quantity;

            AddInventory(index);
        }

        ChangeStorage(index, quantity);
    }

    public void RemoveItem(int index, int quantity)
    {
        while (quantity > 0 && GetInventoryCount() > 0)
        {
            bool remove = false;
            for (int i = 0; i < GetInventoryCount(); ++i)
            {
                if (GetInventorySlot(i) == index)
                {
                    --quantity;

                    RemoveInventory(i);

                    break;
                }
            }

            if (!remove)
                break;
        }

        ChangeStorage(index, -quantity);
    }

    public int CheckItem(int index)
    {
        int temp_value = current_generic_data.storage[index];
        Debug.Log(temp_value);
        for (int i = 0; i < GetInventoryCount(); ++i)
            if (GetInventorySlot(i) == index)
                ++temp_value;

        Debug.Log(temp_value);

        return temp_value;
    }

    //Town Data
    public string[] GetsellableItems(out string[] descriptions, out int[] index, out int[] values)
    {
        List<int> temp_indexs_l = new List<int>();
        List<int> temp_counts_l = new List<int>();
        List<int> temp_values_l = new List<int>();

        List<string> temp_names_l = new List<string>();
        List<string> temp_descriptions_l = new List<string>();

        for (int i = 0; i < current_generic_data.storage.Length; ++i)
        {
            int temp_value = ItemHolder.GetItem(i).GetValue(out bool temp_has_value);

            if (temp_has_value && current_generic_data.storage[i] > 0)
            {
                temp_indexs_l.Add(i);
                temp_counts_l.Add(current_generic_data.storage[i]);
                temp_names_l.Add(ItemHolder.GetItem(i).GetInfo(out string temp_description));
                temp_descriptions_l.Add(temp_description);
                temp_values_l.Add(temp_value);
            }
        }

        for (int i = 0; i < GetInventoryCount(); ++i)
        {
            bool temp_check = false;
            for (int e = 0; e < temp_indexs_l.Count; ++e)
            {
                if (GetInventorySlot(i) == temp_indexs_l[e])
                {
                    ++temp_counts_l[e];
                    temp_check = true;
                    break;
                }
            }

            if (!temp_check)
            {
                int temp_value = ItemHolder.GetItem(GetInventorySlot(i)).GetValue(out bool temp_has_value);

                if (temp_has_value)
                {
                    temp_indexs_l.Add(GetInventorySlot(i));
                    temp_counts_l.Add(1);
                    temp_names_l.Add(ItemHolder.GetItem(GetInventorySlot(i)).GetInfo(out string temp_description));
                    temp_descriptions_l.Add(temp_description);
                    temp_values_l.Add(temp_value);
                }
            }
        }

        for (int i = 0; i < temp_indexs_l.Count; ++i)
            temp_names_l[i] += "   " + temp_counts_l[i] + "   " + temp_values_l[i];

        if (temp_names_l.Count == 0)
        {
            values = new int[0];
            index = new int[0];
            descriptions = new string[0];
            return new string[0];
        }

        values = temp_values_l.ToArray();
        index = temp_indexs_l.ToArray();
        descriptions = temp_descriptions_l.ToArray();
        return temp_names_l.ToArray();
    }

    public void GetRearchUnlock(int[] indexs, out string[] names, out string[] descriptions, out string[] costs)
    {
        names = new string[indexs.Length];
        descriptions = new string[indexs.Length];
        costs = new string[indexs.Length];

        for (int i = 0; i < indexs.Length; ++i)
        {
            ResearchData temp_research = research_data[indexs[i]];

            names[i] = ItemHolder.GetItem(temp_research.GetItems(out int temp_quantity)).GetInfo(out string temp_description);
            names[i] += "   " + (generic_unlocks.researches[indexs[i]] ? "O" : "X");

            descriptions[i] = temp_description;
            costs[i] = "";

            Vector2Int[] temp_cost = temp_research.GetResearchCost();
            for (int e = 0; e < temp_cost.Length; ++e)
            {
                costs[i] += ItemHolder.GetItem(temp_cost[e].x).GetInfo(out string dead_info);
                costs[i] += "   " + temp_cost[e].y + "(" + GetNumberOfItem(temp_cost[e].x) + ")\n";
            }
        }
    }

    public void UnlockResearch(int index)
    {
        if (generic_unlocks.researches[index])
            return;

        Vector2Int[] temp_cost = research_data[index].GetResearchCost();

        for (int i = 0; i < temp_cost.Length; ++i)
        {
            if (temp_cost[i].y > GetNumberOfItem(temp_cost[i].x))
                return;
        }

        for (int i = 0; i < temp_cost.Length; ++i)
        {
            RemoveItem(temp_cost[i].x, temp_cost[i].y);
        }

        generic_unlocks.researches[index] = true;
    }

    public void GetRearchPurchase(out string[] names, out string[] descriptions, out string[] costs, out int[] indexes)
    {
        List<string> temp_names = new List<string>();
        List<string> temp_descriptions = new List<string>();
        List<string> temp_costs = new List<string>();
        List<int> temp_indexes = new List<int>();

        for (int i = 0; i < research_data.Length; ++i)
        {
            if (!generic_unlocks.researches[i])
                continue;

            ResearchData temp_research = research_data[i];
            Vector2Int[] temp_cost = temp_research.GetCraftCost();

            temp_indexes.Add(i);

            temp_names.Add(ItemHolder.GetItem(temp_research.GetItems(out int temp_quantity)).GetInfo(out string temp_description));
            temp_names[i] += "   " + GetNumberOfItem(i) + "   " + temp_quantity;

            temp_descriptions.Add(temp_description);

            temp_costs.Add("");
            for (int e = 0; e < temp_cost.Length; ++e)
            {
                temp_costs[i] += ItemHolder.GetItem(temp_cost[e].x).GetInfo(out string dead_info);
                temp_costs[i] += "   " + temp_cost[e].y + "(" + GetNumberOfItem(temp_cost[e].x) + ")\n";
            }
        }

        indexes = temp_indexes.ToArray();
        costs = temp_costs.ToArray();
        descriptions = temp_descriptions.ToArray();
        names = temp_names.ToArray();
    }

    public void PurchaseResearch(int index)
    {
        if (!generic_unlocks.researches[index])
            return;

        Vector2Int[] temp_cost = research_data[index].GetCraftCost();

        for (int i = 0; i < temp_cost.Length; ++i)
        {
            if (temp_cost[i].y > GetNumberOfItem(temp_cost[i].x))
                return;
        }

        for (int i = 0; i < temp_cost.Length; ++i)
        {
            RemoveItem(temp_cost[i].x, temp_cost[i].y);
        }

        AddItem(research_data[index].GetItems(out int quantity), quantity);
    }

    public void GetGearData(bool gear_type, int[] indexs, out string[] names, out string[] descriptions, out string[] costs)
    {
        names = new string[indexs.Length];
        descriptions = new string[indexs.Length];
        costs = new string[indexs.Length];

        for (int i = 0; i < indexs.Length; ++i)
        {
            GearData temp_gear;
            if (gear_type)
                temp_gear = weapon_data[indexs[i]];
            else
                temp_gear = trinket_data[indexs[i]];

            int temp_class = temp_gear.GetData(out int temp_slot);

            if (gear_type)
            {
                names[i] = classes[temp_class].GetWeaponName(temp_slot);
                names[i] += "   " + (class_unlocks[temp_class].weapons[temp_slot] ? "O" : "X");

                descriptions[i] = classes[temp_class].GetClassName() + "\n" + classes[temp_class].GetWeaponDescription(temp_slot);
            }
            else
            {
                names[i] = classes[temp_class].GetTraitName(temp_slot);
                names[i] += "   " + (class_unlocks[temp_class].trinkets[temp_slot] ? "O" : "X");

                descriptions[i] = classes[temp_class].GetClassName() + "\n" + classes[temp_class].GetTrinketDescription(temp_slot);
            }

            costs[i] = "";
            Vector2Int[] temp_cost = temp_gear.GetCost();
            for (int e = 0; e < temp_cost.Length; ++e)
            {
                costs[i] += ItemHolder.GetItem(temp_cost[e].x).GetInfo(out string dead_info);
                costs[i] += "   " + temp_cost[e].y + "(" + GetNumberOfItem(temp_cost[e].x) + ")\n";
            }
        }
    }

    public void PurchaseGear(bool gear_type, int index)
    {
        GearData temp_gear;
        if (gear_type)
            temp_gear = weapon_data[index];
        else
            temp_gear = trinket_data[index];

        int temp_class = temp_gear.GetData(out int temp_slot);

        Vector2Int[] temp_cost;
        if (gear_type)
        {
            if (class_unlocks[temp_class].weapons[temp_slot])
                return;

            temp_cost = weapon_data[index].GetCost();
        }
        else
        {
            if (class_unlocks[temp_class].trinkets[temp_slot])
                return;

            temp_cost = trinket_data[index].GetCost();
        }

        for (int i = 0; i < temp_cost.Length; ++i)
            if (temp_cost[i].y > GetNumberOfItem(temp_cost[i].x))
                return;
        for (int i = 0; i < temp_cost.Length; ++i)
            RemoveItem(temp_cost[i].x, temp_cost[i].y);

        if (gear_type)
            class_unlocks[temp_class].weapons[temp_slot] = true;
        else
            class_unlocks[temp_class].trinkets[temp_slot] = true;
    }

    //Upgrades
    public string GetRankUp(out bool can_get)
    {
        if (generic_unlocks.lv == 20)
        {
            can_get = false;
            return "Maxed";
        }

        can_get = true;
        string temp_string = "Rank " + generic_unlocks.lv + " -> " + (generic_unlocks.lv + 1) + "\n";
        Vector2Int[] temp_cost = RankCost[generic_unlocks.lv].GetCost();

        for (int i = 0; i < temp_cost.Length; ++i)
        {
            temp_string += "\n" + ItemHolder.GetItem(temp_cost[i].x).GetInfo(out string none) + " " + temp_cost[i].y + "(" + GetNumberOfItem(temp_cost[i].x) + ")";

            if (GetNumberOfItem(temp_cost[i].x) < temp_cost[i].y)
                can_get = false;
        }

        return temp_string;
    }

    public void PurchasRankUp()
    {
        if (generic_unlocks.lv == 20)
            return;

        Vector2Int[] temp_cost = RankCost[generic_unlocks.lv].GetCost();

        for (int i = 0; i < temp_cost.Length; ++i)
            if (GetNumberOfItem(temp_cost[i].x) < temp_cost[i].y)
                return;

        for (int i = 0; i < temp_cost.Length; ++i)
            RemoveItem(temp_cost[i].x, temp_cost[i].y);

        ++generic_unlocks.lv;
    }

    public string GetRefineUp(int index, out bool can_get)
    {
        if (restomon_unlocks[index].refinements == 15)
        {
            can_get = false;
            return "Maxed";
        }

        can_get = true;
        string temp_string = restomon_bases[index].GetName() + " " + restomon_unlocks[index].refinements + " -> " + (restomon_unlocks[index].refinements + 1) + "\n";
        Vector2Int[] temp_cost = restomon_bases[index].GetRefinementsCost(restomon_unlocks[index].refinements).GetCost();

        for (int i = 0; i < temp_cost.Length; ++i)
        {
            temp_string += "\n" + ItemHolder.GetItem(temp_cost[i].x).GetInfo(out string none) + " " + temp_cost[i].y + "(" + GetNumberOfItem(temp_cost[i].x) + ")";

            if (GetNumberOfItem(temp_cost[i].x) < temp_cost[i].y)
                can_get = false;
        }

        return temp_string;
    }

    public void PurchasRefineUp(int index)
    {
        if (restomon_unlocks[index].refinements == 15)
            return;

        Vector2Int[] temp_cost = restomon_bases[index].GetRefinementsCost(restomon_unlocks[index].refinements).GetCost();

        for (int i = 0; i < temp_cost.Length; ++i)
            if (GetNumberOfItem(temp_cost[i].x) < temp_cost[i].y)
                return;

        for (int i = 0; i < temp_cost.Length; ++i)
            RemoveItem(temp_cost[i].x, temp_cost[i].y);

        ++restomon_unlocks[index].refinements;
    }

    public string GetReforgeUp(int index, out bool can_get)
    {
        if (restomon_unlocks[index].reforges == 10)
        {
            can_get = false;
            return "Maxed";
        }

        can_get = true;
        string temp_string = restomon_bases[index].GetName() + " " + restomon_unlocks[index].reforges + " -> " + (restomon_unlocks[index].reforges + 1) + "\n";
        Vector2Int[] temp_cost = restomon_bases[index].GetReforgeCost(restomon_unlocks[index].reforges).GetCost();

        for (int i = 0; i < temp_cost.Length; ++i)
        {
            temp_string += "\n" + ItemHolder.GetItem(temp_cost[i].x).GetInfo(out string none) + " " + temp_cost[i].y + "(" + GetNumberOfItem(temp_cost[i].x) + ")";

            if (GetNumberOfItem(temp_cost[i].x) < temp_cost[i].y)
                can_get = false;
        }

        return temp_string;
    }

    public void PurchasReforgeUp(int index)
    {
        if (restomon_unlocks[index].reforges == 10)
            return;

        Vector2Int[] temp_cost = restomon_bases[index].GetReforgeCost(restomon_unlocks[index].reforges).GetCost();

        for (int i = 0; i < temp_cost.Length; ++i)
            if (GetNumberOfItem(temp_cost[i].x) < temp_cost[i].y)
                return;

        for (int i = 0; i < temp_cost.Length; ++i)
            RemoveItem(temp_cost[i].x, temp_cost[i].y);

        ++restomon_unlocks[index].reforges;
    }

    public string[] GetMoveUnlocks(int index, out string[] names, out string[] description, out Vector2Int[] positions, out bool[] can_get)
    {
        List<string> temp_string = new List<string>();
        List<string> temp_names = new List<string>();
        List<string> temp_description = new List<string>();
        List<Vector2Int> temp_position = new List<Vector2Int>();
        List<bool> temp_get = new List<bool>();

        Attack[] temp_attacks = restomon_bases[index].GetBasicAttacks();
        bool[] temp_attacks_unlock = restomon_unlocks[index].basic_attacks;
        UpgradeCost[] temp_cost = restomon_bases[index].GetBasicAttacksCost();

        for (int i = 0; i < temp_attacks_unlock.Length; ++i)
        {
            if (temp_attacks_unlock[i])
                continue;

            temp_names.Add(temp_attacks[i].GetName());
            temp_description.Add(temp_attacks[i].GetDescription());
            temp_position.Add(new Vector2Int(-1, i));

            string cost_name_temp = "";
            bool cost_unlocked_temp = true;

            for (int I = 0; I < temp_cost[i].GetCost().Length; ++I)
            {
                cost_name_temp += ItemHolder.GetItem(temp_cost[i].GetCost()[I].x).GetInfo(out string none) + " " + temp_cost[i].GetCost()[I].y + " (" + GetNumberOfItem(temp_cost[i].GetCost()[I].x) + ")" + "\n";

                if (GetNumberOfItem(temp_cost[i].GetCost()[I].x) < temp_cost[i].GetCost()[I].y)
                    cost_unlocked_temp = false;
            }

            temp_string.Add(cost_name_temp);
            temp_get.Add(cost_unlocked_temp);
        }

        for (int e = 0; e < 10; ++e)
        {
            if (!restomon_unlocks[index].evolution[e])
                continue;

            temp_attacks = restomon_bases[index].GetAttacks(e);
            temp_attacks_unlock = restomon_unlocks[index].attacks[e];
            temp_cost = restomon_bases[index].GetAttacksCost(e);

            for (int i = 0; i < temp_attacks_unlock.Length; ++i)
            {
                if (temp_attacks_unlock[i])
                    continue;

                temp_names.Add(temp_attacks[i].GetName());
                temp_description.Add(temp_attacks[i].GetDescription());
                temp_position.Add(new Vector2Int(e, i));

                string cost_name_temp = "";
                bool cost_unlocked_temp = true;

                for (int I = 0; I < temp_cost[i].GetCost().Length; ++I)
                {
                    cost_name_temp += ItemHolder.GetItem(temp_cost[i].GetCost()[I].x).GetInfo(out string none) + " " + temp_cost[i].GetCost()[I].y + " (" + GetNumberOfItem(temp_cost[i].GetCost()[I].x) + ")" + "\n";

                    if (GetNumberOfItem(temp_cost[i].GetCost()[I].x) < temp_cost[i].GetCost()[I].y)
                        cost_unlocked_temp = false;
                }

                temp_string.Add(cost_name_temp);
                temp_get.Add(cost_unlocked_temp);
            }
        }

        can_get = temp_get.ToArray();
        positions = temp_position.ToArray();
        description = temp_description.ToArray();
        names = temp_names.ToArray();
        return temp_string.ToArray();
    }

    public void PurchasMove(int index, Vector2Int position)
    {
        Vector2Int[] temp_cost;

        if (position.x == -1)
            temp_cost = restomon_bases[index].GetBasicAttacksCost()[position.y].GetCost();
        else
            temp_cost = restomon_bases[index].GetAttacksCost(position.x)[position.y].GetCost();

        for (int i = 0; i < temp_cost.Length; ++i)
            if (GetNumberOfItem(temp_cost[i].x) < temp_cost[i].y)
                return;

        for (int i = 0; i < temp_cost.Length; ++i)
            RemoveItem(temp_cost[i].x, temp_cost[i].y);

        if (position.x == -1)
            restomon_unlocks[index].basic_attacks[position.y] = true;
        else
            (restomon_unlocks[index].attacks[position.x])[position.y] = true;
    }

    public string[] GetTraitUnlocks(int index, out string[] names, out string[] description, out Vector2Int[] positions, out bool[] can_get)
    {
        List<string> temp_string = new List<string>();
        List<string> temp_names = new List<string>();
        List<string> temp_description = new List<string>();
        List<Vector2Int> temp_position = new List<Vector2Int>();
        List<bool> temp_get = new List<bool>();

        for (int e = 0; e < 10; ++e)
        {
            if (!restomon_unlocks[index].evolution[e])
                continue;

            Trait[] temp_traits = restomon_bases[index].GetTraits(e);
            bool[] temp_traits_unlock = restomon_unlocks[index].traits[e];
            UpgradeCost[] temp_cost = restomon_bases[index].GetTraitsCost(e);

            for (int i = 0; i < temp_traits_unlock.Length; ++i)
            {
                if (temp_traits_unlock[i])
                    continue;

                temp_names.Add(temp_traits[i].GetName());
                temp_description.Add(temp_traits[i].GetDescription());
                temp_position.Add(new Vector2Int(e, i));

                string cost_name_temp = "";
                bool cost_unlocked_temp = true;

                for (int I = 0; I < temp_cost[i].GetCost().Length; ++I)
                {
                    cost_name_temp += ItemHolder.GetItem(temp_cost[i].GetCost()[I].x).GetInfo(out string none) + " " + temp_cost[i].GetCost()[I].y + " (" + GetNumberOfItem(temp_cost[i].GetCost()[I].x) + ")" + "\n";

                    if (GetNumberOfItem(temp_cost[i].GetCost()[I].x) < temp_cost[i].GetCost()[I].y)
                        cost_unlocked_temp = false;
                }

                temp_string.Add(cost_name_temp);
                temp_get.Add(cost_unlocked_temp);
            }
        }

        can_get = temp_get.ToArray();
        positions = temp_position.ToArray();
        description = temp_description.ToArray();
        names = temp_names.ToArray();
        return temp_string.ToArray();
    }

    public void PurchasTrait(int index, Vector2Int position)
    {
        Vector2Int[] temp_cost;

        temp_cost = restomon_bases[index].GetTraitsCost(position.x)[position.y].GetCost();

        for (int i = 0; i < temp_cost.Length; ++i)
            if (GetNumberOfItem(temp_cost[i].x) < temp_cost[i].y)
                return;

        for (int i = 0; i < temp_cost.Length; ++i)
            RemoveItem(temp_cost[i].x, temp_cost[i].y);

        restomon_unlocks[index].traits[position.x][position.y] = true;
    }

    public string[] GetEvolutionUnlocks(int index, out string[] names, out string[] description, out Vector2Int[] positions, out bool[] can_get)
    {
        List<string> temp_string = new List<string>();
        List<string> temp_names = new List<string>();
        List<string> temp_description = new List<string>();
        List<Vector2Int> temp_position = new List<Vector2Int>();
        List<bool> temp_get = new List<bool>();

        string[] temp_form_names = { "Base", "Evolution A", "Evolution B", "Evolution C", "Merge AB", "Merge BC", "Merge AC", "Peak A", "Peak B", "Peak C" };

        for (int e = 0; e < 10; ++e)
        {
            if (restomon_unlocks[index].evolution[e])
                continue;

            if (e > 0 && e < 4 && (!restomon_unlocks[index].evolution[0] || current_generic_data.current_chapter < 1))
                continue;

            if (e == 4 && (!restomon_unlocks[index].evolution[1] || !restomon_unlocks[index].evolution[2] || current_generic_data.current_chapter < 3))
                continue;

            if (e == 5 && (!restomon_unlocks[index].evolution[2] || !restomon_unlocks[index].evolution[3] || current_generic_data.current_chapter < 3))
                continue;

            if (e == 6 && (!restomon_unlocks[index].evolution[1] || !restomon_unlocks[index].evolution[3] || current_generic_data.current_chapter < 3))
                continue;

            if (e > 6 && (!restomon_unlocks[index].evolution[e - 6] || current_generic_data.current_chapter < 5))
                continue;

            temp_names.Add(temp_form_names[e]);
            temp_description.Add(restomon_bases[index].GetDescription(e));
            temp_position.Add(new Vector2Int(e, 0));

            bool can_get_temp = true;
            string temp_string_temp = "";
            Vector2Int[] temp_cost = restomon_bases[index].GetEvolutionUpgradeCost(e).GetCost();

            for (int i = 0; i < temp_cost.Length; ++i)
            {
                temp_string_temp += ItemHolder.GetItem(temp_cost[i].x).GetInfo(out string none) + " " + temp_cost[i].y + "(" + GetNumberOfItem(temp_cost[i].x) + ")" + "\n";

                if (GetNumberOfItem(temp_cost[i].x) < temp_cost[i].y)
                    can_get_temp = false;
            }

            temp_get.Add(can_get_temp);
            temp_string.Add(temp_string_temp);
        }

        can_get = temp_get.ToArray();
        positions = temp_position.ToArray();
        description = temp_description.ToArray();
        names = temp_names.ToArray();
        return temp_string.ToArray();
    }

    public void PurchasEvolution(int index, Vector2Int position)
    {
        if (restomon_unlocks[index].evolution[position.x])
            return;

        if (position.x > 0 && position.x < 4 && (!restomon_unlocks[index].evolution[0] || current_generic_data.current_chapter < 1))
            return;

        if (position.x == 4 && (!restomon_unlocks[index].evolution[1] || !restomon_unlocks[index].evolution[2] || current_generic_data.current_chapter < 3))
            return;

        if (position.x == 5 && (!restomon_unlocks[index].evolution[2] || !restomon_unlocks[index].evolution[3] || current_generic_data.current_chapter < 3))
            return;

        if (position.x == 6 && (!restomon_unlocks[index].evolution[1] || !restomon_unlocks[index].evolution[3] || current_generic_data.current_chapter < 3))
            return;

        if (position.x > 6 && (!restomon_unlocks[index].evolution[position.x - 6] || current_generic_data.current_chapter < 5))
            return;

        Vector2Int[] temp_cost = restomon_bases[index].GetEvolutionUpgradeCost(position.x).GetCost();

        for (int i = 0; i < temp_cost.Length; ++i)
            if (GetNumberOfItem(temp_cost[i].x) < temp_cost[i].y)
                return;

        for (int i = 0; i < temp_cost.Length; ++i)
            RemoveItem(temp_cost[i].x, temp_cost[i].y);

        restomon_unlocks[index].evolution[position.x] = true;
    }

    //Dungeon
    public void SetDungeon(int index)
    {
        current_generic_data.current_dungeon = index;
    }

    public int GetDungeonInt()
    {
        return current_generic_data.current_dungeon;
    }

    public DungeonLayout GetDungeon()
    {
        return dungeons[current_generic_data.current_dungeon];
    }

    //Overworld
    public void SetOverworld(int index, Vector2Int position)
    {
        current_generic_data.current_overworld = index;
        current_generic_data.current_position = position;
    }

    public int GetOverworldInt()
    {
        return current_generic_data.current_overworld;
    }

    public OverworldLayout GetOverworld(out Vector2Int position)
    {
        position = current_generic_data.current_position;
        return overworlds[current_generic_data.current_overworld];
    }

    //Quest Data
    public int GetEventData(EventDataType data_type, int index)
    {
        switch (data_type)
        {
            case EventDataType.MainQuest:
                return current_quest_data.main_quest_markers[index];
            case EventDataType.SideQuest:
                return current_quest_data.side_quest_markers[index];
        }

        return -1;
    }

    public void SetEventData(EventDataType data_type, int index, int value)
    {
        switch (data_type)
        {
            case EventDataType.MainQuest:
                current_quest_data.main_quest_markers[index] = value;
                return;
            case EventDataType.SideQuest:
                current_quest_data.side_quest_markers[index] = value;
                return;
        }
    }

    public bool GetDungeonData(DungeonDataType data_type, int index)
    {
        switch (data_type)
        {
            case DungeonDataType.DungeonUnlocked:
                return current_quest_data.dungeon_unlocked[index];
            case DungeonDataType.DungeonCleared:
                return current_quest_data.dungeon_cleared[index];
        }

        return false;
    }

    public void SetDungeonData(DungeonDataType data_type, int index, bool value)
    {
        switch (data_type)
        {
            case DungeonDataType.DungeonUnlocked:
                current_quest_data.dungeon_unlocked[index] = value;
                return;
            case DungeonDataType.DungeonCleared:
                current_quest_data.dungeon_cleared[index] = value;
                return;
        }
    }
}