public class CircleArray<T>
{
    private int size;

    private int pointer;

    private static T[] current_value;

    public CircleArray(int size)
    {
        this.size = size;
        pointer = 0;

        current_value = new T[size];

        for (int i = 0; i < size; ++i)
            current_value[i] = default(T);
    }

    public void Add(T value)
    {
        current_value[pointer] = value;

        ++pointer;

        if (pointer == size)
            pointer = 0;
    }

    public T GetValue(int index)
    {
        if (index < 0 || index >= size)
            return default(T);

        if (index + pointer < size)
            return current_value[index + pointer];
        else
            return current_value[index + pointer - size];
    }
}