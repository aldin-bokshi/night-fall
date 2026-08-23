using Godot;

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

    public static RunConfig Create(
        string seedText,
        bool bloodMoon,
        bool glassCannon,
        bool hardNight,
        bool greed,
        bool fragile)
    {
        ulong seed = SeedTranslator.ToNumericSeed(seedText);

        GD.Print($"Seed text: {seedText}");
        GD.Print($"Numeric seed: {seed}");

        return new RunConfig(
            seedText,
            seed,
            bloodMoon,
            glassCannon,
            hardNight,
            greed,
            fragile);
    }
}