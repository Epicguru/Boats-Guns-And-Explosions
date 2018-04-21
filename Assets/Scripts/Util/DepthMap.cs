
using UnityEngine;

public class FloatMap
{
    private float[] values;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public bool Created { get; private set; }

    public FloatMap(int width, int height)
    {
        if(width <= 0 || height <= 0)
        {
            Debug.LogError("Width ({0}) and height ({1}) must both be 1 or more!".Form(width, height));
            return;
        }
        Width = width;
        Height = height;
        int length = width * height;
        values = new float[length];
        Created = true;
    }

    public bool IsInBounds(int x, int y)
    {
        if (!Created)
            return false;

        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public int GetIndexOf(int x, int y)
    {
        if (!Created)
            return -1;

        return x + (y * Width);
    }

    public void SetValue(int x, int y, float value)
    {
        if (!IsInBounds(x, y))
            return;

        values[GetIndexOf(x, y)] = value;
    }

    public float GetValue(int x, int y)
    {
        if (!Created)
        {
            return 0f;
        }

        if(IsInBounds(x, y))
        {
            return values[GetIndexOf(x, y)];
        }
        else
        {
            return 0f;
        }
    }

    public float[] GetArray()
    {
        if (!Created)
        {
            return null;
        }

        return values;
    }
}