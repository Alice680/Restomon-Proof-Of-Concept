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
// TODO add values for score
public class PermDataHolder : MonoBehaviour
{
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

    [SerializeField] private DungeonLayout[] dungeons;
    private int current_dungeon;

    [SerializeField] private HumanClass[] classes;
    private int[] current_player;

    [SerializeField] private RestomonBase[] restomon;
    private RestomonData[] current_team;

    [SerializeField] private Catalyst[] catalysts;
    private int current_catalyst;

    public void Setup()
    {
        DontDestroyOnLoad(gameObject);

        current_dungeon = 0;

        current_player = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

        current_catalyst = 0;

        current_team = new RestomonData[4] { new RestomonData(), new RestomonData(), new RestomonData(), new RestomonData() };

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

    //Player
    public void SetPlayer(int class_i, int sub_i, int weapon_i, int armor_i, int trinket_i, int trait_a, int trait_b, int trait_c)
    {
        current_player = new int[8] { class_i, sub_i, weapon_i, armor_i, trinket_i, trait_a, trait_b, trait_c };
    }

    public int[] GetPlayerInt()
    {
        return current_player;
    }

    public Human GetPlayer()
    {
        return classes[current_player[0]].GetHuman(3, current_player[1], current_player[2], current_player[3], current_player[4], 0, new int[3] { current_player[5], current_player[6], current_player[7] }, "Player");
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
        return restomon[current_team[index].restomon_id].GetRestomon(3, new int[0], new int[0]);
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
        return restomon[index];
    }
}