using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The base method for creatures. This class dose not do anything on it's own but instead serves as a template for 
 * all other creature types to use.
 * 
 * 
 * Notes:
 * Do not create instances of this class unless it is for testing.
 * 
 * Stats are broken down into 4 groups. Energy, offensive, defensive, and speed.
 * Energy has Health points in all casses and mave have more for certain creature types.
 * Offensive are used to deal damage or aplly effects and have attack for physical, magic for magical, and force for status condtions.
 * Defensive are used to prevent damage or aplly effects and have attack for defence, shield for magical, and will for status condtions.
 * Speed is for how often you can do things. Speed determins how often you gain turns, movement determins how many spaces you can move, and actions are used to perform actions.
 * 
 * Traits are passive abilites that trigger under certain conditions. Look at the trait class for more infomation.
 * 
 * Attacks are active abilites you use during your turn. Look at the attack class for more infomration.
 * 
 * Elements give certain benifits and demerits situatly. Such as bonus damage for using an attack of the same element you have.
 * The base form and both evolutions after it each grant one element. With each after the first granting slighly less potency.
 * The elements are: Ather, Fire, Nature, Earth, Metal, Water, Light, Dark, Wind, Lightning, Ice, Cosmic
 * 
 * Notes where not made for methods in this class as it would just be the name and saying it return nothing.
 */

public enum CreatureType { None, Human, Restomon, Monster, Floor }

public class Creature
{
    public virtual CreatureType GetCreatureType()
    {
        return CreatureType.None;
    }

    public Element GetElement(int index)
    {
        if (index == 0)
            return Element.None;
        else if (index == 1)
            return Element.None;
        else if (index == 2)
            return Element.None;
        else
            return Element.None;
    }

    public virtual int GetID()
    {
        return -1;
    }

    public virtual int GetLV()
    {
        return -1;
    }

    public virtual int GetHp()
    {
        return -1;
    }

    public virtual int GetStat(int index)
    {
        return -1;
    }

    public virtual Attack GetAttack(int index)
    {
        return null;
    }

    public virtual Trait[] GetTraits()
    {
        return null;
    }

    public virtual GameObject GetModel()
    {
        return null;
    }

    public override string ToString()
    {
        return "Error, using creature";
    }
}