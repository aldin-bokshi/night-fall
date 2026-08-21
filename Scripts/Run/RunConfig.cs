namespace NightFall.Scripts.Run;

public sealed class RunConfig(
    string seedText,
    ulong seed,
    bool bloodMoon,
    bool glassCannon,
    bool hardNight,
    bool greed,
    bool fragile)
{
    public string SeedText { get; } = seedText;
    public ulong Seed { get; } = seed;

    public bool BloodMoon { get; } = bloodMoon;
    public bool GlassCannon { get; } = glassCannon;
    public bool HardNight { get; } = hardNight;
    public bool Greed { get; } = greed;
    public bool Fragile { get; } = fragile;
}