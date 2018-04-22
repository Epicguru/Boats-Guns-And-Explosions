
using UnityEngine;

public enum Faction : byte
{
    RED,
    BLUE,
    GREEN,
    PURPLE
}

public static class FactionUtils
{
    public static Color GetColour(this Faction faction)
    {
        switch (faction)
        {
            case Faction.RED:
                return Color.red;
            case Faction.BLUE:
                return Color.blue;
            case Faction.GREEN:
                return Color.green;
            case Faction.PURPLE:
                return new Color((float)192 / 255, (float)10 / 255, (float)209 / 255, 1f);
            default:
                return Color.black;
        }
    }
}