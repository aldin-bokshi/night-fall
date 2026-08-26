using System;

namespace NightFall.Scripts.Shop;

public sealed class RerolledEventArgs(int rerollsRemaining)
    : EventArgs
{
    public int RerollsRemaining { get; } = rerollsRemaining;
}