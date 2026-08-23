using System;
using Godot;
using NightFall.Data.Shop;

namespace NightFall.Scripts.Shop;

public static class ShopItemFormatter
{
    public static string GetRarityText(ItemData item)
    {
        if (string.IsNullOrWhiteSpace(item.Rarity))
        {
            return "Common";
        }

        return item.Rarity;
    }

    public static string FormatRarity(string rarity)
    {
        return rarity.ToUpperInvariant();
    }

    public static Color GetRarityColor(string rarity)
    {
        switch (rarity.ToUpperInvariant())
        {
            case "COMMON":
                return new Color(0.55f, 0.5f, 0.62f);

            case "UNCOMMON":
                return new Color(0.35f, 0.75f, 0.55f);

            case "RARE":
                return new Color(0.35f, 0.55f, 0.95f);

            case "EPIC":
                return new Color(0.7f, 0.4f, 0.95f);

            case "LEGENDARY":
                return new Color(1f, 0.65f, 0.25f);

            default:
                return new Color(0.5f, 0.42f, 0.62f);
        }
    }

    public static string FormatStatName(string statName)
    {
        return statName
            .Replace("_", " ", StringComparison.Ordinal)
            .ToUpperInvariant();
    }

    public static string FormatStatValue(string statName, float value)
    {
        string upperName = statName.ToUpperInvariant();

        if (upperName.Contains("COOLDOWN", StringComparison.Ordinal) ||
            upperName.Contains("CHANCE", StringComparison.Ordinal) ||
            upperName.Contains("PERCENT", StringComparison.Ordinal))
        {
            return $"{value:+0;-0}%";
        }

        return $"{value:+0;-0}";
    }
}