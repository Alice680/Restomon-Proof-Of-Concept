using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss", menuName = "ScriptableObjects/Dungeons/Boss")]
public class DungeonFloorPresetBoss : DungeonFloorPreset
{
    [SerializeField] private MonsterSpawnInfo boss;
    [SerializeField] Vector3Int boss_spawn;

    public override DungeonMap GenerateDungeon(DungeonWeatherManager weather_manager, out Vector3Int start_location)
    {
        return base.GenerateDungeon(weather_manager, out start_location);
    }

    public override DungeonMap GenerateDungeon(DungeonWeatherManager weather_manager, PermDataHolder data_holder, out Vector3Int start_location, out Creature[] enemies, out Vector3Int[] positions)
    {
        enemies = new Creature[1] { boss.GetCreature() };
        positions = new Vector3Int[1] { boss_spawn };
        return base.GenerateDungeon(weather_manager,data_holder, out start_location, out Creature[] nothing_a, out Vector3Int[] nothing_b);
    }
}