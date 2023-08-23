using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Keeps track of the turn order.
 * It dose so by keeping track of a current placement. Incrementing it by the units speed until at least one hits 10.
 * When a new turn start the current leading unit placement gose down by 10
 *  
 * Notes:
 * Equations are a bith rough right now.
 * Speed it to important, maybe add something into it.
 * Position in the array should not determin turn order. Make a tie breaker.
 */
//TODO revist equation for turn changing once proper speeds are in play
//TODO add a random element to who gose if both players are tied for placement
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