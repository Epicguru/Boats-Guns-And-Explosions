
using UnityEngine;

public enum UnitOption : byte
{
    STOP_ENGINE
}

public static class UnitOptionUtils
{
    public static string GetString(this UnitOption option)
    {
        switch (option)
        {
            case UnitOption.STOP_ENGINE:
                return "Stop Engine";
            default:
                return option.ToString().Replace("_", " ").ToLower();
        }
    }

    public static KeyCode GetInputKey(this UnitOption option, int index)
    {
        return KeyCode.E;
    }

    public static Sprite GetIcon(this UnitOption option)
    {
        return null;
    }
}