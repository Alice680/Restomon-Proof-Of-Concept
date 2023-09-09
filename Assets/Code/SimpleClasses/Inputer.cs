using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A class used to help standerdise inputs across my game. 
 * It takes inputs from the unity input system and returns a couple variables.
 * 
 * Notes:
 * The buttons for input are fixed atm, but will be dynamic once I begin setting up the options menu
 */
//TODO add alternate controll options
public class Inputer
{
    private Direction dir;
    private bool enter;
    private bool back;
    private bool action_one;

    private float last_input;

    public void Run()
    {
        Clear();

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

    public void Clear()
    {
        dir = Direction.None;
        enter = false;
        back = false;
        action_one = false;
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