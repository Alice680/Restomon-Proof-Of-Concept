using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Base script for all dungeon layouts.
 * 
 * Notes:
 * Just serves as a base for inheritance
 * 
 * Do not use at run time.
 */
public enum DungeonType { None, Arena, Boss, Lair }
public class DungeonFloor : ScriptableObject
{
    [SerializeField] protected AIBase ai;

    public virtual DungeonMap GenerateDungeon(DungeonWeatherManager weather_manager, out Vector3Int start_location)
    {
        start_location = new Vector3Int();
        return new DungeonMap(0, 0, null);
    }

    public virtual DungeonMap GenerateDungeon(DungeonWeatherManager weather_manager,out Vector3Int start_location, out Creature[] enemies, out Vector3Int[] positions)
    {
        start_location = new Vector3Int();
        enemies = null;
        positions = null;
        return new DungeonMap(0, 0, null);
    }

    public virtual Vector3Int GetStartPosition()
    {
        return new Vector3Int(-1, -1,0);
    }

    public virtual Creature GetDungeonManager()
    {
        return new Creature();
    }

    public virtual Creature[] GetStartUnits()
    {
        return new Creature[0];
    }

    public virtual Creature GetRandomCreature()
    {
        return null;
    }

    public virtual AIBase GetAI()
    {
        return null;
    }

    public virtual DungeonType GetDungeonType()
    {
        return DungeonType.None;
    }
}