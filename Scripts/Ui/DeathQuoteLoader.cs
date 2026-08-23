using System.Text.Json;
using Godot;
using NightFall.Scripts.Core;

namespace NightFall.Scripts.Ui;

public static class DeathQuoteLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static string GetRandomQuote()
    {
        string json = FileAccess.GetFileAsString(GamePaths.DeathQuotes);

        DeathQuoteData? data = JsonSerializer.Deserialize<DeathQuoteData>(
            json,
            JsonOptions);

        if (data?.Quotes is not { Length: > 0 }) return "You died.";

        return SelectWeightedQuote(data.Quotes).Quote ?? "You died.";
    }

    private static DeathQuoteEntry SelectWeightedQuote(
        DeathQuoteEntry[] quotes)
    {
        float totalWeight = 0f;

        foreach (DeathQuoteEntry quote in quotes)
        {
            totalWeight += GetWeight(quote);
        }

        float roll = GD.Randf() * totalWeight;

        foreach (DeathQuoteEntry quote in quotes)
        {
            roll -= GetWeight(quote);

            if (roll <= 0f) return quote;
        }

        return quotes[^1];
    }

    private static float GetWeight(DeathQuoteEntry quote)
    {
        string rarity = quote.Rarity?.ToUpperInvariant() ?? string.Empty;

        return rarity switch
        {
            "COMMON" => 2f,
            "UNCOMMON" => 1f,
            "RARE" => 0.5f,
            "LEGENDARY" => 0.1f,
            _ => 1f
        };
    }

    // System.Text.Json creates these through reflection.
    // CA1812 incorrectly considers them unused.
#pragma warning disable CA1812

    private sealed class DeathQuoteData
    {
        public DeathQuoteEntry[] Quotes { get; init; } = [];
    }

    private sealed class DeathQuoteEntry(string? quote, string? rarity)
    {
        public string? Quote { get; } = quote;
        public string? Rarity { get; } = rarity;
    }
}