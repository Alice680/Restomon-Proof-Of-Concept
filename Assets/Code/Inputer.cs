using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputer
{
    private Direction dir;
    private bool enter;
    private bool back;
    private bool action_one;

    private float last_input;

    public void Run()
    {
        dir = Direction.None;
        enter = false;
        back = false;
        action_one = false;

        if (Time.time - last_input <= 0.1F)
            return;

        if (Input.GetKey(KeyCode.W))
            dir = Direction.Up;
        if (Input.GetKey(KeyCode.D))
            dir = Direction.Right;
        if (Input.GetKey(KeyCode.S))
            dir = Direction.Down;
        if (Input.GetKey(KeyCode.A))
            dir = Direction.Left;

        if (Input.GetKeyDown(KeyCode.Return) && Time.time - last_input > 0.15)
            enter = true;

        if (Input.GetKeyDown(KeyCode.Backspace) && Time.time - last_input > 0.15)
            back = true;

        if (Input.GetKeyDown(KeyCode.E) && Time.time - last_input > 0.15)
            action_one = true;

        if (dir != Direction.None || enter || back || action_one)
            last_input = Time.time;
    }

    public Direction GetDir()
    {
        return dir;
    }

    public bool GetEnter()
    {
        return enter;
    }

    public bool GetBack()
    {
        return back;
    }

    public bool GetActionOne()
    {
        return action_one;
    }
}