using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Handles the making of pre made dungeon layouts.
 * 
 * 
 * Notes:
 * Script is only for running in editor. Will not save if used at run time.
 */
public class DungeonConstructor : MonoBehaviour
{
    public DungeonFloorPreset layout;
    public bool new_layout;
    public int x_size;
    public int y_size;

    public int current_tile;
    public DungeonMap current_map;

    private float last_input;

    public void Start()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        layout.SetDirty();
#pragma warning restore CS0618 // Type or member is obsolete

        if (new_layout)
            layout.Setup(x_size, y_size);

        current_map = layout.GenerateDungeon(null,out Vector3Int empty_variable);
    }

    public void Update()
    {
        if (Input.GetMouseButton(0) && Time.time - last_input > 0.025F)
        {
            var v3 = Input.mousePosition;
            v3.z = 10.0f;
            v3 = Camera.main.ScreenToWorldPoint(v3);
            v3 += new Vector3(0.5f, 0.5f, 0);
            Vector2Int vec = new Vector2Int((int)v3.x, (int)v3.y);

            if (vec.x < 0 || vec.y < 0 || vec.x >= x_size || vec.y >= y_size)
                return;

            layout.SetTile(vec.x,vec.y, current_tile);

            current_map.Clear();
            current_map = layout.GenerateDungeon(null,out Vector3Int empty_variable);

            last_input = Time.time;
        }
    }
}