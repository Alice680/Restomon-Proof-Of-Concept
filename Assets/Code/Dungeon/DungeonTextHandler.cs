using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTextHandler : MonoBehaviour
{
    private static LinkedQueue<string> new_text_holder;
    private CircleArray<string> current_text;

    public static void AddText(string new_text)
    {
        new_text_holder.Add(new_text);
    }

    public void SetUp()
    {
        new_text_holder = new LinkedQueue<string>();
        current_text = new CircleArray<string>(5);
    }

    public void Run()
    {
        while (new_text_holder.GetSize() != 0)
            current_text.Add(new_text_holder.Pop());

        for (int i = 0; i < 5; ++i)
            Debug.Log(current_text.GetValue(i));
    }
}