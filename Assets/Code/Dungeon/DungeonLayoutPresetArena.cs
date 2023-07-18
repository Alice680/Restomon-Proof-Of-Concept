using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Arena", menuName = "ScriptableObjects/Dungeons/Arena")]
public class DungeonLayoutPresetArena : DungeonLayoutPreset
{
    [SerializeField] private MonsterSpawnInfo[] monster_spawns;
    [SerializeField] private int spawn_rate;

    public override DungeonMap GenerateDungeon()
    {
        return base.GenerateDungeon();
    }

    public override Vector3Int GetStartPosition()
    {
        return base.GetStartPosition();
    }

    public override Creature GetDungeonManager()
    {
        return new ArenaStats(spawn_rate);
    }

    public override Creature[] GetStartUnits()
    {
        return new Creature[0];
    }

    public override Creature GetRandomCreature()
    {
        List<CreatureSpawnInfo> list = new List<CreatureSpawnInfo>();

        foreach (CreatureSpawnInfo spawn_info in monster_spawns)
            list.Add(spawn_info);

        return list[Random.Range(0, list.Count)].GetCreature();
    }

    public override AIBase GetAI()
    {
        return base.GetAI();
    }

    public override DungeonType GetDungeonType()
    {
        return DungeonType.Arena;
    }
}