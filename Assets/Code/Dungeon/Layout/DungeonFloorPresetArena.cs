using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Takes layout code from its base class and handles spawning enemies and win conditions in its place
 * 
 * Notes:
 * May merge with base class
 */

// TODO once working on dungeons agian, see if there is any need to have this and a boss fight class
[CreateAssetMenu(fileName = "Arena", menuName = "ScriptableObjects/Dungeons/Arena")]
public class DungeonFloorPresetArena : DungeonFloorPreset
{
    [System.Serializable]
    protected class MonsterChance
    {
        public int chance;
        public MonsterStats monster;
    }

    [SerializeField] private MonsterChance[] monsters;
    [SerializeField] private int spawn_rate;
    [SerializeField] private int total_waves;

    public override DungeonMap GenerateDungeon(DungeonWeatherManager weather_manager, out Vector3Int start_location)
    {
        return base.GenerateDungeon(weather_manager, out start_location);
    }

    public override DungeonMap GenerateDungeon(DungeonWeatherManager weather_manager, out Vector3Int start_location, out Creature[] enemies, out Vector3Int[] positions)
    {
        return base.GenerateDungeon(weather_manager, out start_location, out enemies, out positions);
    }

    public override Vector3Int GetStartPosition()
    {
        return base.GetStartPosition();
    }

    public override Creature GetDungeonManager()
    {
        return new ArenaStats(spawn_rate, total_waves);
    }

    public override Creature[] GetStartUnits()
    {
        return new Creature[0];
    }

    public override Creature GetRandomCreature()
    {
        Creature creature = null;

        int temp_random = Random.Range(0, 100);

        foreach (MonsterChance option in monsters)
            if (option.chance > temp_random)
            {
                creature = option.monster.GetMonster();
                break;
            }

        return creature;
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