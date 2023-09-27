using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedQueue<T>
{
    private class Node
    {
        public T value;
        public Node next;

        public Node(T value)
        {
            this.value = value;
        }
    }

    private Node head;

    public void Add(T value)
    {
        if (head == null)
        {
            head = new Node(value);
            return;
        }

        Node temp = head;

        while (temp.next != null)
            temp = temp.next;
    }

    public T Pop()
    {
        if (head == null)
            return default(T);

        T value = default(T);

        value = head.value;
        head = head.next;

        return value;
    }

    public int GetSize()
    {
        if (head == null)
            return 0;

        Node temp = head;
        int size = 0;

        while (temp != null)
        {
            temp = temp.next;
            ++size;
        }

        return size;
    }

    public void Clear()
    {
        head = null;
    }
}