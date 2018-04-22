
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
}