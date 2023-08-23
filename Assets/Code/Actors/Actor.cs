using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A method that servs as the inputter for players and ai alike.
 * Each one represents one of the two active entities in any given battle.
 * They all have the same permisions and can only do actions by trying to run them by DungeonManager
 * 
 * Notes:
 * Do not make instances of this class unless it is for testing.
 */
public class Actor
{
    public virtual void Run()
    {

    }
}