using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A simple method used by attacks to track their effects
 * Notes:
 */
// TODO add proper security
[Serializable]
public class AttackAffect
{
    public AttackEffect type;
    public Target target;
    public int[] variables;
}