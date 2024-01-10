using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldConstructor : MonoBehaviour
{
    public bool active;

    public OverworldLayout layout;
    public bool new_layout;
    public int x_size;
    public int y_size;

    public bool traversable;
    public GameObject current_model;

    private OverworldMap current_map;

    private float last_input;

    private void Start()
    {
        if (!active)
            return;

#pragma warning disable CS0618 // Type or member is obsolete
        layout.SetDirty();
#pragma warning restore CS0618 // Type or member is obsolete

        if (new_layout)
            layout.Setup(x_size, y_size, current_model);

        current_map = layout.GetMap();
    }

    private void Update()
    {
        if (!active)
            return;

        if (Input.GetMouseButton(0) && Time.time - last_input > 0.025F)
        {
            var v3 = Input.mousePosition;
            v3.z = 10.0f;
            v3 = Camera.main.ScreenToWorldPoint(v3);
            v3 += new Vector3(0.5f, 0.5f, 0);
            Vector2Int vec = new Vector2Int((int)v3.x, (int)v3.y);

            if (vec.x < 0 || vec.y < 0 || vec.x >= x_size || vec.y >= y_size)
                return;

            Debug.Log(vec);

            layout.SetTile(vec.x, vec.y, current_model, traversable);

            current_map.Clear();
            current_map = layout.GetMap();

            last_input = Time.time;
        }
    }
}