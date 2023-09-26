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
// TODO remove dungeons and classes and add them into the main menu
// TODO add values for score, win condtion, and Restomon
public class PermDataHolder : MonoBehaviour
{
    [SerializeField] private DungeonLayout[] dungeons;
    private int current_dungeon;

    [SerializeField] private HumanClass[] classes;
    private Human current_player;

    private Restomon[] team;

    public void Setup()
    {
        DontDestroyOnLoad(gameObject);

        team = new Restomon[3];

        current_dungeon = 0;
        current_player = null;
        team[0] = null;
        team[1] = null;
        team[2] = null;
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

    public Human GetPlayer()
    {
        return current_player;
    }

    public void SetTeam(Restomon mon, int index)
    {
        if (index < 0 || index > 3)
            return;

        team[index] = mon;
    }
}