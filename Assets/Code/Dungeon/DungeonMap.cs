using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A collection of nodes that serves as the map for any dugeon.
 * Most actions actions with this in some way.
 * 
 * Notes:
 * Nodes track what unit is in them, if they are groud or wall, the texture they have.
 * The map tracks the current length and width.
 */
// TODO Finish Tile Conditions
// TODO Traversability beyound just wall or ground. Water and Lava at least.
public enum DungeonTileType { Empty, Ground, Wall };
public class DungeonMap
{
    private class Node
    {
        public DungeonTileType type;
        public GameObject model;
        public GameObject marker;
        public GameObject trait_model;
        public GameObject weather_model;
        public Unit current_unit;
        public int tile_trait;
        public int model_num;

        public int x, y;

        public Node()
        {
            type = DungeonTileType.Empty;
            model = null;
            current_unit = null;
            tile_trait = 0;
        }

        public Node(DungeonTileType type, GameObject model, int model_num, int x, int y)
        {
            this.x = x;
            this.y = y;
            this.type = type;
            this.model = GameObject.Instantiate(model, new Vector3(x, y, 0), new Quaternion());
            this.model_num = model_num;
            current_unit = null;
            tile_trait = 0;
        }

        public void SetMarker(int id)
        {
            ClearMarker();
            GameObject obj;

            if (id == 0)
                obj = Resources.Load<GameObject>("white");
            else if (id == 1)
                obj = Resources.Load<GameObject>("blue");
            else if (id == 2)
                obj = Resources.Load<GameObject>("green");
            else
                obj = Resources.Load<GameObject>("red");
            marker = GameObject.Instantiate(obj);
            marker.transform.position = new Vector3Int(x, y, 0);
        }

        public void ClearMarker()
        {
            if (marker != null)
                GameObject.Destroy(marker);
        }

        public void SetTrait(int id, GameObject obj)
        {
            ClearTrait();
            tile_trait = id;
            trait_model = GameObject.Instantiate(obj, new Vector3Int(x, y, 0), new Quaternion());
        }

        public void ClearTrait()
        {
            tile_trait = 0;

            if (trait_model != null)
                GameObject.Destroy(trait_model);
        }

        public void SetWeather(GameObject weather)
        {
            weather_model = GameObject.Instantiate(weather, new Vector3Int(x, y, 0), new Quaternion());
        }

        public void ClearWeather()
        {
            if (weather_model != null)
                GameObject.Destroy(weather_model);
        }

        public void Clear()
        {
            ClearMarker();
            ClearWeather();
            ClearTrait();

            if (model != null)
                GameObject.Destroy(model);
        }
    }

    private int x_size, y_size;
    private Node[,] nodes;

    private TileConditionHolder tile_conditions;

    //Node and Map editing
    public DungeonMap(int x, int y)
    {
        x_size = x;
        y_size = y;

        nodes = new Node[x_size, y_size];

        for (int i = 0; i < x_size; ++i)
            for (int e = 0; e < y_size; ++e)
                nodes[i, e] = new Node();

        tile_conditions = (TileConditionHolder)Resources.Load("TileConditionHolder");
    }

    public void SetNode(int x, int y, DungeonTileType type, GameObject model, int model_num)
    {
        if (!IsInMap(x, y))
            return;

        nodes[x, y].Clear();
        nodes[x, y] = new Node(type, model, model_num, x, y);
    }

    public DungeonTileType GetTileType(Vector3Int vec)
    {
        return GetTileType(vec.x, vec.y);
    }
    public DungeonTileType GetTileType(int x, int y)
    {
        return nodes[x, y].type;
    }

    public int GetTileModelNum(Vector3Int vec)
    {
        return GetTileModelNum(vec.x, vec.y);
    }
    public int GetTileModelNum(int x, int y)
    {
        return nodes[x, y].model_num;
    }

    public void Clear()
    {
        for (int i = 0; i < x_size; ++i)
            for (int e = 0; e < y_size; ++e)
                nodes[i, e].Clear();
    }

    //Units
    public void MoveUnit(Vector3Int vec, Unit unit) { MoveUnit(vec.x, vec.y, unit); }
    public void MoveUnit(int x, int y, Unit unit)
    {
        if (!IsInMap(x, y))
            return;

        RemoveUnit(unit);

        nodes[x, y].current_unit = unit;
        unit.SetPosition(new Vector3Int(x, y, 0));
    }

    public Unit GetUnit(Vector3Int vec)
    {
        return GetUnit(vec.x, vec.y);
    }
    public Unit GetUnit(int x, int y)
    {
        return nodes[x, y].current_unit;
    }

    public void RemoveUnit(Unit unit)
    {
        Vector3Int vec = unit.GetPosition();

        if (!IsInMap(vec))
            return;

        nodes[vec.x, vec.y].current_unit = null;

        unit.SetPosition(new Vector3Int(-1, -1, 0));
    }

    //Markers
    public void SetMarker(int x, int y, int index)
    {
        if (!IsInMap(x, y))
            return;

        nodes[x, y].SetMarker(index);
    }

    public void RemoveMarker(int x, int y)
    {
        if (!IsInMap(x, y))
            return;

        nodes[x, y].ClearMarker();
    }

    public void RemoveAllMarker()
    {
        for (int i = 0; i < x_size; ++i)
            for (int e = 0; e < y_size; ++e)
                RemoveMarker(i, e);
    }

    //Tile Conditions
    public Trait GetTileTrait(Vector3Int vec)
    {
        return GetTileTrait(vec.x, vec.y);
    }
    public Trait GetTileTrait(int x, int y)
    {
        return tile_conditions.GetEffect(nodes[x, y].tile_trait);
    }

    public void SetTileTrait(Vector3Int vec, int id)
    {
        SetTileTrait(vec.x, vec.y, id);
    }
    public void SetTileTrait(int x, int y, int id)
    {
        if (!IsInMap(x, y))
            return;

        nodes[x, y].SetTrait(id, tile_conditions.GetModel(id));
    }

    //Checks
    public bool IsInMap(Vector3Int vec)
    {
        return IsInMap(vec.x, vec.y);
    }
    public bool IsInMap(int x, int y)
    {
        if (x < 0 || x >= x_size || y < 0 || y >= y_size)
            return false;

        return true;
    }

    public bool IsValidMove(Unit unit, Vector3Int vec)
    {
        return IsValidMove(unit, vec.x, vec.y);
    }
    public bool IsValidMove(Unit unit, int x, int y)
    {
        if (!IsInMap(x, y))
            return false;

        if (nodes[x, y].current_unit != null)
            return false;

        if (nodes[x, y].type == DungeonTileType.Wall)
            return false;

        return true;
    }

    public Vector3Int GetSize()
    {
        return new Vector3Int(x_size, y_size, 0);
    }
}