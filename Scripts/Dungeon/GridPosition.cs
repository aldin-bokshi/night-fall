using System.Runtime.InteropServices;

namespace NightFall.Scripts.Dungeon;

[StructLayout(LayoutKind.Auto)]
public readonly record struct GridPosition(int X, int Y)
{
    public static GridPosition operator +(
        GridPosition a,
        GridPosition b)
    {
        return new(
            a.X + b.X,
            a.Y + b.Y);
    }

    public static GridPosition operator -(
        GridPosition a,
        GridPosition b)
    {
        return new(
            a.X - b.X,
            a.Y - b.Y);
    }

    public static GridPosition operator *(
        GridPosition position,
        int multiplier)
    {
        return new(
            position.X * multiplier,
            position.Y * multiplier);
    }
}