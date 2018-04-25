
using UnityEngine;

public enum UnitOption : byte
{
    STOP_ENGINE,
    START_ENGINE
}

public static class UnitOptionUtils
{
    public static string GetString(this UnitOption option)
    {
        if(UnitOptionGO.Instance == null)
        {
            return "<ERROR>";
        }
        else
        {
            var op = UnitOptionGO.Instance.Data[option];
            return op == null ? "<mssing_name>" : op.Name;
        }
    }

    public static KeyCode GetInputKey(this UnitOption option, int index)
    {
        if (UnitOptionGO.Instance == null)
        {
            return KeyCode.None;
        }
        else
        {
            var op = UnitOptionGO.Instance.Data[option];
            return op == null ? KeyCode.None : op.DefaultKey;
        }
    }

    public static Sprite GetIcon(this UnitOption option)
    {
        if (UnitOptionGO.Instance == null)
        {
            return null;
        }
        else
        {
            var op = UnitOptionGO.Instance.Data[option];
            return op == null ? null : op.Icon;
        }
    }
}