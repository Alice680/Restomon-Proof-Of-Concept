using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnKeeper
{
    private class Node
    {
        private Unit unit;
        private int placement;

        public Node(Unit u)
        {
            unit = u;
            placement = -15;
        }

        public Node(Unit u, int i)
        {
            unit = u;
            placement = i;
        }

        public void Increment()
        {
            placement += unit.GetStat(6);
        }

        public void Reduce()
        {
            placement -= 10;
        }

        public Unit GetUnit()
        {
            return unit;
        }

        public int GetPlacement()
        {
            return placement;
        }

        public Node Copy(Node old_node)
        {
            return new Node(old_node.GetUnit(), old_node.GetPlacement());
        }
    }

    private List<Node> nodes;

    private Node next_unit;

    public TurnKeeper()
    {
        nodes = new List<Node>();
    }

    public void NextTurn()
    {
        if (next_unit != null)
            next_unit.Reduce();

        next_unit = null;

        do
            foreach (Node node in nodes)
            {
                node.Increment();

                if (next_unit == null || next_unit.GetPlacement() < node.GetPlacement())
                    next_unit = node;
            }
        while (next_unit.GetPlacement() < 10);
    }

    public void AddUnit(Unit unit)
    {
        foreach (Node node in nodes)
            if (node.GetUnit().GetID() == unit.GetID())
                return;

        nodes.Add(new Node(unit));
    }

    public void RemoveUnit(Unit unit)
    {
        Node temp_node = null;

        foreach (Node node in nodes)
            if (node.GetUnit().GetID() == unit.GetID())
                temp_node = node;

        if (temp_node != null)
            nodes.Remove(temp_node);
    }

    public Unit Peak()
    {
        if (next_unit == null)
            return null;

        return next_unit.GetUnit();
    }
}