
using System.Collections.Generic;
using UnityEngine;

public enum UnitOption : byte
{
    SHIP_STOP_ENGINE,
    SHIP_START_ENGINE,
    WS_FIRE_AT,
    WS_CEASE_FIRE
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

    public static List<UnitOptionInput> GetInputs(this UnitOption option)
    {
        if (UnitOptionGO.Instance == null)
        {
            return null;
        }
        else
        {
            var op = UnitOptionGO.Instance.Data[option];
            return op == null ? null : op.Inputs;
        }
    }
}