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
    private Human current_player;

    [SerializeField] private RestomonBase[] restomon;
    private Restomon[] current_team;

    [SerializeField] private Catalyst[] catalysts;
    private int current_catalyst;

    public void Setup()
    {
        DontDestroyOnLoad(gameObject);

        current_team = new Restomon[3];

        current_dungeon = 0;
        current_player = null;
        current_team[0] = null;
        current_team[1] = null;
        current_team[2] = null;
    }

    public void SetDungeon(int index)
    {
        current_dungeon = index;
    }

    public DungeonLayout GetDungeon()
    {
        return dungeons[current_dungeon];
    }

    public void SetPlayer(int class_i, int sub_i, int weapon_i, int armor_i, int trinket_i, int trait_a, int trait_b, int trait_c)
    {
        current_player = classes[class_i].GetHuman(3, sub_i, weapon_i, armor_i, trinket_i, 0, new int[3] { trait_a, trait_b, trait_c }, "Player");
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

    public Human GetPlayer()
    {
        return current_player;
    }

    public Restomon GetTeam(int index)
    {
        return current_team[index];
    }

    public Catalyst GetCatalyst()
    {
        return catalysts[current_catalyst];
    }
}