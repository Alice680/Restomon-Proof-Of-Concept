using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableAttack", order = 2)]
public class Attack : ScriptableObject
{
    [SerializeField] ShapeData area;
    [SerializeField] ShapeData target;

    public void ApplyEffect(Unit user, Unit target)
    {
        if (target == null)
            return;

        target.ChangeHp(-1);
        Debug.Log(target.GetHp());
    }

    public Vector3Int[] GetArea(Direction dir)
    {
        return area.GetArea(dir);
    }

    public Vector3Int[] GetTarget(Direction dir)
    {
        return target.GetArea(dir);
    }
}