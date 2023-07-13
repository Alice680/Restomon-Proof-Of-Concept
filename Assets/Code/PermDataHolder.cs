using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermDataHolder : MonoBehaviour
{
    private DungeonLayout dungeon;

    private Human player;

    private Restomon[] team;

    public void Setup()
    {
        DontDestroyOnLoad(gameObject);

        team = new Restomon[3];

        dungeon = null;
        player = null;
        team[0] = null;
        team[1] = null;
        team[2] = null;
    }

    public void SetDungeon(DungeonLayout dungeon)
    {
        this.dungeon = dungeon;
    }

    public DungeonLayout GetDungeon()
    {
        return dungeon;
    }

    public void SetPlayer(Human player)
    {
        this.player = player;
    }

    public Human GetPlayer()
    {
        return player;
    }

    public void SetTeam(Restomon mon, int index)
    {
        if (index < 0 || index > 3)
            return;

        team[index] = mon;
    }
}