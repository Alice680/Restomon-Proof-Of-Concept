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
    [SerializeField] private DungeonLayout[] dungeons;
    private int current_dungeon;

    [SerializeField] private HumanClass[] classes;
    private int[] current_player;

    [SerializeField] private RestomonBase[] restomon;
    private Restomon[] current_team;

    [SerializeField] private Catalyst[] catalysts;
    private int current_catalyst;

    public void Setup()
    {
        DontDestroyOnLoad(gameObject);

        current_player = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        current_team = new Restomon[3];

        current_dungeon = 0;
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

    public void SetCatalyst(int index)
    {
        if (index < 0)
            current_catalyst = 0;
        else
            current_catalyst = index;
    }

    public void SetRestomon(int index)
    {
        current_team[index] = null;
    }

    public void SetRestomon(int index, int restomon_id, int[] attack_id, int[] trait_id)
    {
        current_team[index] = restomon[restomon_id].GetRestomon(1, attack_id, trait_id);
    }

    public Restomon GetTeam(int index)
    {
        return current_team[index];
    }

    public Catalyst GetCatalyst()
    {
        return catalysts[current_catalyst];
    }

    public HumanClass GetDataHuman(int index)
    {
        return classes[index];
    }
}