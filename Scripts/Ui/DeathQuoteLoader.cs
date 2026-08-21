using Godot;
using System.Text.Json;

namespace NightFall.Scripts.Ui;

public static class DeathQuoteLoader
{
    private const string Path = "res://Data/DeathQuotes.json";

    public static string GetRandomQuote()
    {
        string json = FileAccess.GetFileAsString(Path);

        var data = JsonSerializer.Deserialize<DeathQuoteData>(
            json,
            new JsonSerializerOptions{PropertyNameCaseInsensitive = true}
        );

        if (data?.Quotes == null || data.Quotes.Length == 0) return "You died.";

        int index = GD.RandRange(0, data.Quotes.Length - 1);
        return data.Quotes[index].Quote ?? "You died.";
    }

    private sealed class DeathQuoteData
    {
        public DeathQuoteEntry[] Quotes { get; init; } = [];
    }

    private sealed class DeathQuoteEntry
    {
        public string? Quote { get; init; }
        public string? Rarity { get; init; }
    }
}